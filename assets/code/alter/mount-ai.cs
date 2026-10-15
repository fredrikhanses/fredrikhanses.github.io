using UnityEngine;
using UnityEngine.AI;

[SelectionBase]
[RequireComponent(typeof(NavMeshAgent))]
public class CompanionFollow : MonoBehaviour
{
    [SerializeField, Range(0f, 5f), Tooltip("Interval in seconds to update target destination")] 
    private float m_UpdateInterval = 1f;
    [SerializeField] private bool m_GetDistracted = true;
    [SerializeField, Range(0f, 60f), Tooltip("Interval in seconds to distract companion")] 
    private float m_DistractionInterval = 10f;
    [SerializeField, Range(0f, 100f), Tooltip("Max distraction range")] 
    private float m_MaxDistractionRange = 50f;
    [SerializeField, Tooltip("NavMeshAgent for this scene")] 
    private NavMeshAgent m_Agent;
    [SerializeField, Tooltip("Target to follow")] 
    private GameObject m_Target;
    [SerializeField, Tooltip("Player character")] 
    private PlayerCharacter m_Player;
    
    public GameObject Target { get => m_Target; set => m_Target = value; }
    public NavMeshAgent Agent { get => m_Agent; }

    private float m_InitialUpdateInterval;
    private Vector3 m_CurrentTargetPosition;
    private float m_InitialDistractionInterval;
    private float m_StoppingDistance;

    private void Start()
    {
        if (Agent == null)
        {
            m_Agent = GetComponent<NavMeshAgent>();
        }
        if (m_Player == null)
        {
            m_Player = FindObjectOfType<PlayerCharacter>();
        }
        if (Target == null && m_Player)
        {
            Target = m_Player.gameObject;
        }
        m_CurrentTargetPosition = transform.position;
        m_InitialUpdateInterval = m_UpdateInterval;
        m_InitialDistractionInterval = m_DistractionInterval;
        if(Agent != null)
        {
            m_StoppingDistance = Agent.stoppingDistance;
        }
    }

    private void FixedUpdate()
    {
        if (m_GetDistracted)
        {
            m_DistractionInterval -= Time.fixedDeltaTime;
            if (m_DistractionInterval <= 0)
            {
                if (Target == null)
                {
                    Target = m_Player.gameObject;
                    Agent.stoppingDistance = m_StoppingDistance;
                    m_CurrentTargetPosition = Target.transform.position;
                    m_Agent.destination = m_CurrentTargetPosition;
                }
                else
                {
                    Target = null;
                    m_CurrentTargetPosition = Vector3.zero;
                    Agent.stoppingDistance = 0f;
                    Vector3 newDestination = new Vector3(transform.position.x 
                    + Random.Range(-m_MaxDistractionRange, m_MaxDistractionRange), 
                    transform.position.y + Random.Range(-m_MaxDistractionRange, m_MaxDistractionRange), 
                    transform.position.z + Random.Range(-m_MaxDistractionRange, m_MaxDistractionRange));
                    m_Agent.destination = newDestination;
                }
                m_DistractionInterval = m_InitialDistractionInterval;
            }
        }

        m_UpdateInterval -= Time.fixedDeltaTime;
        if (m_UpdateInterval <= 0)
        {
            if (Target != null && m_CurrentTargetPosition != Target.transform.position)
            {
                m_CurrentTargetPosition = Target.transform.position;
                m_Agent.destination = m_CurrentTargetPosition;
            }
            m_UpdateInterval = m_InitialUpdateInterval;
        }
    }
}

using UnityEngine;
using UnityEngine.Assertions;

[SelectionBase]
public class CompanionCharacter : MonoBehaviour, IEvent, IInteract
{
    [Header("Stats")]
    [Tooltip("Height where player should be mounted at.")]
    [SerializeField, Range(0f, 5f)] private float m_MountHeight = 2.6f;
    [Tooltip("Rotation speed of companion when mounted.")]
    [SerializeField, Range(0f, 0.5f)] private float m_RotationSpeed = 0.1f;
    [Tooltip("Max running speed of companion when mounted.")]
    [SerializeField, Range(0f, 50f)] private float m_MaxRunningSpeed = 20f;
    [Tooltip("Max walking speed of companion when mounted.")]
    [SerializeField, Range(0f, 25f)] private float m_MaxWalkingSpeed = 5f;
    [Tooltip("How high should the companion be able to jump?")]
    [SerializeField, Range(0f, 30f)] private float m_JumpForce = 10f;
    [Tooltip("How fast should the companion reach its max speed?")]
    [SerializeField, Range(0f, 0.5f)] private float m_Acceleration = 0.1f;
    [Tooltip("How fast should the momentum of the companion drop?")]
    [SerializeField, Range(0f, 2.5f)] private float m_MomentumDrop = 0.5f;
    [Tooltip("Center of the blue box bellow the companion, shows where the companion connects with the floor.")]
    [SerializeField] private Vector3 m_FloorColliderOffset;
    [Tooltip("Size of the blue box bellow the companion, shows where the companion connects with the floor.")]
    [SerializeField] private Vector3 m_FloorColliderSize;
    [Tooltip("Which layers can the floor collider interact with.")]
    [SerializeField] private LayerMask m_FloorChecks;
    [Header("Components")]
    [Tooltip("Player character component")]
    [SerializeField] private PlayerCharacter m_Player;
    [Tooltip("Camera movement component")]
    [SerializeField] private CameraMovement m_CameraMovement;
    [Tooltip("Companion normal body component")]
    [SerializeField] private GameObject m_Body;
    [Tooltip("Companion spirit body component")]
    [SerializeField] private GameObject m_SpiritBody;
    [Tooltip("Companion AI Follow component")]
    [SerializeField] private CompanionFollow m_CompanionFollow;
    [Tooltip("Companion rigidbody component")]
    [SerializeField] private Rigidbody m_CompanionRigidBody;
    [Tooltip("Companion input component")]
    [SerializeField] private CompanionInput m_CompanionInput;

    public bool Walking { get; set; }
    public bool Falling { get; set; }
    public bool Jumping { get; set; }
    public bool Dismounting { get; set; }
    public bool Grounded { get; set; }
    public bool Mounted { get; set; } = false;
    public bool OrbForm { get; set; } = false;
    public Vector3 DesiredMovement { get; set; }
    public CameraMovement CameraMovement { get => m_CameraMovement; }

    private Rigidbody m_PlayerRigidBody;
    private CapsuleCollider m_PlayerCollider;
    private PlayerInput m_PlayerInput;
    private Vector3 m_DesiredDirection;
    private float m_CurrentSpeed;
    private const string k_PlayerTag = "Player";
    private const string k_UntaggedTag = "Untagged";

    private void Start()
    {
        if (m_CompanionRigidBody == null)
        {
            m_CompanionRigidBody = GetComponent<Rigidbody>();
        }
        if (m_CompanionFollow == null)
        {
            m_CompanionFollow = GetComponent<CompanionFollow>();
        }
        if (m_CompanionInput == null)
        {
            m_CompanionInput = GetComponent<CompanionInput>();
        }
        if (m_Player == null)
        {
            m_Player = FindObjectOfType<PlayerCharacter>();
        }
        if (m_Player != null)
        {
            m_PlayerRigidBody = m_Player.PlayerRigidBody;
            m_PlayerCollider = m_Player.GetComponent<CapsuleCollider>();
            m_PlayerInput = m_Player.GetComponent<PlayerInput>();
        }
        if (m_CameraMovement == null)
        {
            m_CameraMovement = FindObjectOfType<CameraMovement>();
        }
        Assert.IsNotNull(m_Body, "Missing Body component");
        Assert.IsNotNull(m_SpiritBody, "Missing OrbBody component");
        Assert.IsNotNull(m_CameraMovement, "Missing CameraMovement component");
    }

    private void FixedUpdate()
    {
        GroundCheck();

        if (DesiredMovement != Vector3.zero)
        {
            Accelerate();
            CalculateDirection();

            if (Walking && (DesiredMovement == Vector3.back || DesiredMovement == Vector3.back + Vector3.left 
                || DesiredMovement == Vector3.back + Vector3.right))
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, 
                Quaternion.LookRotation(-m_DesiredDirection, transform.up), m_RotationSpeed);
            }
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, 
                Quaternion.LookRotation(m_DesiredDirection, transform.up), m_RotationSpeed);
            }

            if (Grounded)
            {
                if (Walking)
                {
                    SpeedDrop();
                }
                if (Walking && (DesiredMovement == Vector3.back || DesiredMovement == Vector3.back + Vector3.left 
                || DesiredMovement == Vector3.back + Vector3.right))
                {
                    m_CompanionRigidBody.velocity = (m_DesiredDirection * m_CurrentSpeed);
                }
                else
                {
                    m_CompanionRigidBody.velocity = (m_DesiredDirection * m_CurrentSpeed);  
                }
            }
        }
        else
        {
            SpeedDrop();
        }

        if(Jumping && Grounded && !Falling)
        {
            Jumping = false;
            if (DesiredMovement == Vector3.zero)
            {
                m_CompanionRigidBody.AddForce(transform.up * m_JumpForce, ForceMode.Impulse);
            }
            else
            {
                m_CompanionRigidBody.AddForce(transform.up * m_JumpForce * 1.5f, ForceMode.Impulse);
            }
        }

        if(Dismounting)
        {
            EnablePlayer();
            DismountPlayer();
            DisableCompanion();
        }
    }

    public void ImDoingMyThing()
    {
        if (!OrbForm && !Mounted)
        {
            DisablePlayer();
            MountPlayer();
            EnableCompanion();
        }
    }

    public void Event()
    {
        ToggleForm();
    }

    [ContextMenu("ToggleForm")]
    public void ToggleForm()
    {
        if (OrbForm)
        {
            m_SpiritBody.SetActive(false);
            m_Body.SetActive(true);
            OrbForm = false;
        }
        else if (!Mounted)
        {
            m_Body.SetActive(false);
            m_SpiritBody.SetActive(true);
            OrbForm = true;
        }
    }

    private void Accelerate()
    {
        if (m_CurrentSpeed < m_MaxRunningSpeed)
        {
            m_CurrentSpeed += m_Acceleration;
            if (m_CurrentSpeed >= m_MaxRunningSpeed)
            {
                m_CurrentSpeed = m_MaxRunningSpeed;
            }
        }
    }

    private void SpeedDrop()
    {
        if (m_CurrentSpeed > m_MaxWalkingSpeed)
        {
            m_CurrentSpeed -= m_MomentumDrop;
            if (m_CurrentSpeed <= m_MaxWalkingSpeed)
            {
                m_CurrentSpeed = m_MaxWalkingSpeed;
            }
        }
    }

    private void CalculateDirection()
    {
        Vector2 cameraDirectionForward = m_CameraMovement.DirectionForward;
        Vector2 cameraDirectionRight = m_CameraMovement.DirectionRight;
        Vector3 desiredForward = new Vector3(cameraDirectionForward.x, 0, cameraDirectionForward.y);
        Vector3 desiredRight = new Vector3(cameraDirectionRight.x, 0, cameraDirectionRight.y);
        m_DesiredDirection = (DesiredMovement.z * desiredForward + DesiredMovement.x * desiredRight).normalized;
    }

    private void EnablePlayer()
    {
        m_PlayerCollider.enabled = true;
        m_PlayerRigidBody.isKinematic = false;
        m_PlayerRigidBody.useGravity = true;
        m_PlayerInput.enabled = true;
    }

    private void EnableCompanion()
    {
        m_CompanionFollow.enabled = false;
        m_CompanionFollow.Agent.enabled = false;
        m_CompanionFollow.Target = m_Player.gameObject;
        gameObject.tag = k_PlayerTag;
        m_CompanionRigidBody.isKinematic = false;
        m_CompanionInput.enabled = true;
        Mounted = true;
        m_Player.m_Mounted = true;
    }

    private void DisablePlayer()
    {
        m_PlayerInput.enabled = false;
        m_PlayerRigidBody.useGravity = false;
        m_PlayerRigidBody.isKinematic = true;
        m_PlayerCollider.enabled = false;
        m_Player.m_Airborne = true;
    }

    private void DisableCompanion()
    {
        m_CompanionInput.enabled = false;
        DesiredMovement = Vector3.zero;
        m_CompanionRigidBody.isKinematic = true;
        m_CompanionFollow.Agent.enabled = true;
        m_CompanionFollow.enabled = true;
        gameObject.tag = k_UntaggedTag;
        Dismounting = false;
        Mounted = false;
        m_Player.m_Mounted = false;
    }

    private void MountPlayer()
    {
        Vector3 mountPosition = new Vector3
        {
            x = transform.position.x,
            y = transform.position.y + m_MountHeight,
            z = transform.position.z
        };
        m_Player.transform.SetParent(transform);
        m_Player.transform.position = mountPosition;
        m_Player.transform.rotation = transform.rotation;
    }

    private void DismountPlayer()
    {
        m_Player.transform.SetParent(null);
        if (DesiredMovement == Vector3.zero)
        {
            m_Player.PlayerRigidBody.AddForce((-transform.forward + transform.up) 
            * m_Player.CharacterMaximumJumpForce * 0.75f, ForceMode.Impulse);
        }
        else
        {
            m_Player.PlayerRigidBody.AddForce((m_Player.m_DesiredMovement + transform.up) 
            * m_Player.CharacterMaximumJumpForce * 0.5f, ForceMode.Impulse);
        }
    }

    private void GroundCheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit,
            0.5f, m_FloorChecks))
        {
            Vector3 projection = transform.forward - (Vector3.Dot(transform.forward, hit.normal)) * hit.normal;
            transform.rotation = Quaternion.Lerp(transform.rotation, 
            Quaternion.LookRotation(projection, hit.normal), m_RotationSpeed);
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
            Grounded = true;
            Falling = false;
        }
        else
        {
            Grounded = false;
            Falling = true;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(m_FloorColliderOffset + transform.position, m_FloorColliderSize);
        Gizmos.color = Color.white;
    }
#endif
}

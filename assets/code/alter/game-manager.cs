using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.VFX;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("LevelTesting")]
    [SerializeField] private bool m_NoStaticCamera;
    [Space]
    [SerializeField] private WorldStateData m_WorldStateData;
    [SerializeField] private PlayerSpawnPoint m_CurrentPlayerSpawnPoint;
    [SerializeField] private PlayerCharacter m_PlayerCharacter;
    [SerializeField] private CameraMovement m_CameraMovement;
    [SerializeField] private SoundManager m_SoundManager;
    [SerializeField] private PlayerInput m_PlayerInput;
    [SerializeField] private Rigidbody m_PlayerRigidbody;
    [SerializeField] private Animator m_PlayerAnimator;
    [Header("Snow")]
    [SerializeField] private Vector3 m_SnowSpeedMaxUncorrupted;
    [SerializeField] private Vector3 m_SnowSpeedMinUncorrupted;
    [SerializeField] private int m_SpawnAmountUncorrupted;
    [SerializeField] private Vector3 m_SnowSpeedMaxCorrupted;
    [SerializeField] private Vector3 m_SnowSpeedMinCorrupted;
    [SerializeField] private int m_SpawnAmountCorrupted;
    [SerializeField] private Vector3 m_SnowSpeedMaxFullCorrupted;
    [SerializeField] private Vector3 m_SnowSpeedMinFullCorrupted;
    [SerializeField] private int m_SpawnAmountFullCorrupted;
    [SerializeField] private VisualEffectAsset m_SnowAsset;
    [Space]
    [SerializeField] private PlayerSpawnPoint[] m_PlayerSpawnPoints;
    [SerializeField] private ShrineLight[] m_ShrineLights;
    [SerializeField] private ChangingStone[] m_ChangingStones;
    [SerializeField] private ButtonPrompts[] m_ButtonPrompts;

    public WorldStateData WorldStateData { get => m_WorldStateData; }
    public PlayerSpawnPoint CurrentPlayerSpawnPoint 
    { get => m_CurrentPlayerSpawnPoint; set => m_CurrentPlayerSpawnPoint = value; }
    public PlayerCharacter PlayerCharacter { get => m_PlayerCharacter; }
    public CameraMovement CameraMovement { get => m_CameraMovement; }
    public Rigidbody PlayerRigidbody { get => m_PlayerRigidbody; }
    public PlayerInput PlayerInput { get => m_PlayerInput; }
    public List<GameObject> DontDestroyOnLoadList { get => m_DontDestroyOnLoadList; }
    public Animator PlayerAnimator { get => m_PlayerAnimator; }
    public SoundManager SoundManager { get => m_SoundManager; }
    public Vector3 m_OriginalCameraPosition;
    public Quaternion m_OriginalCameraRotation;

    private ChangeMood m_MoodChanger;
    private Mother[] m_Mothers;
    private List<LoadSceneTrigger> m_Portals;
    private List<VisualEffect> m_VisualEffects;
    private List<GameObject> m_DontDestroyOnLoadList;
    private Vector3[] m_InitialPlayerSpawnPointPosition;

    private const string k_Peaceful = "Peaceful";
    private const string k_OverWorld = "OverWorld";
    private const string k_SpiritWorld = "SpiritWorld";
    private const string k_Wind = "Wind";
    private const string k_Transition = "Transition";
    private const string k_LevelLoaded = "LevelLoaded";
    private const string k_Rise = "Rise";
    private const string k_Respawn = "Respawn";
    private const string k_SnowSpeedMax = "VelocitySpeedMax";
    private const string k_SnowSpeedMin = "VelocitySpeedMin";
    private const string k_SpawnAmount = "SpawnAmount";

    #region DeveloperTools
#if UNITY_EDITOR
    private const string k_CyclePostProcessProfile = "CyclePostProcessProfile";
    private const string k_RestartLevel = "RestartLevel";
    private const string k_MainMenu = "MainMenu";
    private const string k_Spirit1 = "Spirit1";
    private const string k_Spirit2 = "Spirit2";
    private const string k_Ending = "Ending";
    private const string k_StartGame = "StartGame";
    private int m_CycleIndex = 0;
#endif
    #endregion DeveloperTools

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            m_DontDestroyOnLoadList = new List<GameObject> { gameObject };
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    private void Start()
    {
        if (m_SoundManager == null)
        {
            m_SoundManager = GetComponent<SoundManager>();
        }
        if (WorldStateData.CurrentLevel == WorldStateData.StartMenu && m_SoundManager != null)
        {
            m_SoundManager.PlayMusic(k_Wind, true);
        }
        if (m_NoStaticCamera == false)
        {
            m_CameraMovement.m_Static = true;
            m_PlayerInput.enabled = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
        m_InitialPlayerSpawnPointPosition = new Vector3[m_PlayerSpawnPoints.Length];
        for (int i = 0; i < m_PlayerSpawnPoints.Length; i++)
        {
            m_InitialPlayerSpawnPointPosition[i] = m_PlayerSpawnPoints[i].transform.position;
        }
        m_OriginalCameraPosition = m_CameraMovement.transform.position;
        m_OriginalCameraRotation = m_CameraMovement.transform.rotation;
    }

    #region DeveloperTools
#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetButtonDown(k_Respawn) && WorldStateData.CurrentLevel != WorldStateData.StartMenu)
        {
            RespawnPlayer();
        }
        if (Input.GetButtonDown(k_CyclePostProcessProfile) && WorldStateData.CurrentLevel == WorldStateData.OverWorld)
        {
            if (m_MoodChanger == null)
            {
                m_MoodChanger = FindObjectOfType<ChangeMood>();
            }
            if (m_MoodChanger != null)
            {
                if (m_CycleIndex > 2)
                {
                    m_CycleIndex = 0;
                }
                if (m_CycleIndex == 0)
                {
                    m_MoodChanger.SetCorrupted();
                }
                if (m_CycleIndex == 1)
                {
                    m_MoodChanger.SetFullCorrupted();
                }
                if (m_CycleIndex == 2)
                {
                    m_MoodChanger.SetUncorrupted();
                }
                m_CycleIndex++;
            }
        }
        if(Input.GetButtonDown(k_RestartLevel) && WorldStateData.CurrentLevel == WorldStateData.OverWorld)
        {
            ShrineCompleted();
            RestartLevel();
        }
        if(Input.GetButtonDown(k_MainMenu))
        {
            ResetToMainMenu();
        }
        if (Input.GetButtonDown(k_OverWorld))
        {
            LoadLevel(WorldStateData.OverWorld);
        }
        if (Input.GetButtonDown(k_Transition))
        {
            LoadLevel(WorldStateData.Transition);
        }
        if (Input.GetButtonDown(k_Spirit1))
        {
            LoadLevel(WorldStateData.SpiritWorldFirst, 0);
        }
        if (Input.GetButtonDown(k_Spirit2))
        {
            LoadLevel(WorldStateData.SpiritWorldSecond, 1);
        }
        if (Input.GetButtonDown(k_Ending))
        {
            LoadLevel(WorldStateData.SpiritWorldThird, 2);
        }
    }

    private void LoadLevel(int level , int shrinesCompleted = 0)
    {
        WorldStateData.ShrinesCompleted = shrinesCompleted;
        WorldStateData.CurrentLevel = level;
        WorldStateData.NextLevel = level;
        SceneManager.LoadSceneAsync(level);
        UIController.Instance.m_Canvas.GetComponent<Animator>().SetBool(k_StartGame, true);
    }
#endif
    #endregion DeveloperTools

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == WorldStateData.StartMenu)
        {
            m_CameraMovement.m_Static = true;
        }
        if (scene.buildIndex != WorldStateData.StartMenu)
        {
            WorldStateData.NextLevel = scene.buildIndex;
        }
    }

    private void OnSceneUnloaded(Scene scene)
    {
        WorldStateData.LastLevel = scene.buildIndex;
        m_CurrentPlayerSpawnPoint = m_PlayerSpawnPoints[WorldStateData.NextLevel];
        if (WorldStateData.CurrentLevel == WorldStateData.SpiritWorldFirst 
        || WorldStateData.CurrentLevel == WorldStateData.SpiritWorldSecond 
        || WorldStateData.CurrentLevel == WorldStateData.SpiritWorldThird)
        {
            MovePlayerToStart();
        }
    }

    private void OnActiveSceneChanged(Scene current, Scene next)
    {
        WorldStateData.CurrentLevel = next.buildIndex;
        if (WorldStateData.CurrentLevel == WorldStateData.OverWorld)
        {
            m_PlayerAnimator.SetBool(k_Rise, true);
            if (WorldStateData.ShrinesCompleted <= 0)
            {
                SetupUncorruptedPhase();
            }
            else if (WorldStateData.ShrinesCompleted == 1)
            {
                SetupCorruptedPhase();
            }
            else if (WorldStateData.ShrinesCompleted > 1)
            {
                SetupFullCorruptedPhase();
            }
        }
        else if (WorldStateData.CurrentLevel == WorldStateData.Transition)
        {
            m_Portals = new List<LoadSceneTrigger>(FindObjectsOfType<LoadSceneTrigger>());
            foreach (LoadSceneTrigger loadSceneTrigger in m_Portals)
            {
                if (WorldStateData.ShrinesCompleted == 0)
                {
                    loadSceneTrigger.LoadIndex = WorldStateData.SpiritWorldFirst;
                }
                if (WorldStateData.ShrinesCompleted == 1)
                {
                    loadSceneTrigger.LoadIndex = WorldStateData.SpiritWorldSecond;
                }
                if (WorldStateData.ShrinesCompleted > 1)
                {
                    loadSceneTrigger.LoadIndex = WorldStateData.SpiritWorldThird;
                }
            }
            m_CurrentPlayerSpawnPoint = m_PlayerSpawnPoints[WorldStateData.CurrentLevel];
            MovePlayerToStart();
            m_SoundManager.PlayMusic(k_Transition, true);
            SetupClosedLevel();
        }
        else if(WorldStateData.CurrentLevel == WorldStateData.SpiritWorldFirst 
        || WorldStateData.CurrentLevel == WorldStateData.SpiritWorldSecond)
        {
            if (WorldStateData.CurrentLevel == WorldStateData.SpiritWorldThird)
            {
                UIController.Instance.StartBox.GetComponent<UIStartDialogue>().m_Ending = true;
            }
            m_SoundManager.PlayMusic(k_SpiritWorld, true);
            SetupClosedLevel();
        }
    }

    private void SetupUncorruptedPhase()
    {
        m_SoundManager.PlayMusic(k_Peaceful, true);
        m_PlayerInput.enabled = true;
        m_CameraMovement.m_Static = false;
        m_CurrentPlayerSpawnPoint = m_PlayerSpawnPoints[WorldStateData.CurrentLevel];
        MovePlayerToStart();
        if (m_ShrineLights.Length >= 3)
        {
            m_ShrineLights[0].SetEnable(true);
            m_ShrineLights[1].SetEnable(false);
            m_ShrineLights[2].SetEnable(false);
        }
        if (m_ChangingStones.Length >= 3)
        {
            m_ChangingStones[0].SetEnable(false);
            m_ChangingStones[1].SetEnable(true);
            m_ChangingStones[2].SetEnable(true);
        }
        if (m_ButtonPrompts.Length >= 2)
        {
            m_ButtonPrompts[0].gameObject.SetActive(true);
            m_ButtonPrompts[1].gameObject.SetActive(true);
        }
        m_Portals = new List<LoadSceneTrigger>(FindObjectsOfType<LoadSceneTrigger>());
        foreach (LoadSceneTrigger loadSceneTrigger in m_Portals)
        {
            if (loadSceneTrigger.ShrineNumber == 2 || loadSceneTrigger.ShrineNumber == 3)
            {
                loadSceneTrigger.Deactivate();
            }
        }
        m_VisualEffects = new List<VisualEffect>(FindObjectsOfType<VisualEffect>());
        foreach (VisualEffect visualEffect in m_VisualEffects)
        {
            if (m_SnowAsset != null && visualEffect.visualEffectAsset.Equals(m_SnowAsset))
            {
                visualEffect.SetVector3(k_SnowSpeedMax, m_SnowSpeedMaxUncorrupted);
                visualEffect.SetVector3(k_SnowSpeedMin, m_SnowSpeedMinUncorrupted);
                visualEffect.SetInt(k_SpawnAmount, m_SpawnAmountUncorrupted);
            }
        }
    }

    private void SetupCorruptedPhase()
    {
        m_SoundManager.PlayMusic(k_OverWorld, true);
        m_SoundManager.PlayMusic(k_Wind, true);
        m_CameraMovement.m_Static = false;
        m_PlayerInput.enabled = true;
        m_CurrentPlayerSpawnPoint = m_PlayerSpawnPoints[WorldStateData.CurrentLevel];
        MovePlayerToStart();
        if (m_ShrineLights.Length >= 3)
        {
            m_ShrineLights[0].SetEnable(false);
            m_ShrineLights[1].SetEnable(true);
            m_ShrineLights[2].SetEnable(false);
        }
        if (m_ChangingStones.Length >= 3)
        {
            m_ChangingStones[0].SetEnable(true);
            m_ChangingStones[1].SetEnable(false);
            m_ChangingStones[2].SetEnable(true);
        }
        if (m_ButtonPrompts.Length >= 2)
        {
            m_ButtonPrompts[0].gameObject.SetActive(false);
            m_ButtonPrompts[1].gameObject.SetActive(false);
        }
        if (m_MoodChanger == null)
        {
            m_MoodChanger = FindObjectOfType<ChangeMood>();
        }
        if (m_MoodChanger != null)
        {
            m_MoodChanger.SetCorrupted();
        }
        m_Mothers = FindObjectsOfType<Mother>();
        if (m_Mothers != null && m_Mothers.Length > 0)
        {
            foreach (Mother mother in m_Mothers)
            {
                if (mother.FirstMother == true)
                {
                    mother.gameObject.SetActive(false);
                }
                else
                {
                    mother.SetCorrupted();
                }
            }
        }
        m_Portals = new List<LoadSceneTrigger>(FindObjectsOfType<LoadSceneTrigger>());
        foreach (LoadSceneTrigger loadSceneTrigger in m_Portals)
        {
            if (loadSceneTrigger.ShrineNumber == 1 || loadSceneTrigger.ShrineNumber == 3)
            {
                loadSceneTrigger.Deactivate();
            }
        }
        m_VisualEffects = new List<VisualEffect>(FindObjectsOfType<VisualEffect>());
        foreach (VisualEffect visualEffect in m_VisualEffects)
        {
            if (m_SnowAsset != null && visualEffect.visualEffectAsset.Equals(m_SnowAsset))
            {
                visualEffect.SetVector3(k_SnowSpeedMax, m_SnowSpeedMaxCorrupted);
                visualEffect.SetVector3(k_SnowSpeedMin, m_SnowSpeedMinCorrupted);
                visualEffect.SetInt(k_SpawnAmount, m_SpawnAmountCorrupted);
            }
        }
    }

    private void SetupFullCorruptedPhase()
    {
        m_SoundManager.PlayMusic(k_OverWorld, true);
        m_SoundManager.PlayMusic(k_Wind, true);
        m_CameraMovement.m_Static = false;
        m_PlayerInput.enabled = true;
        m_CurrentPlayerSpawnPoint = m_PlayerSpawnPoints[WorldStateData.CurrentLevel];
        MovePlayerToStart();
        if (m_ShrineLights.Length >= 3)
        {
            m_ShrineLights[0].SetEnable(false);
            m_ShrineLights[1].SetEnable(false);
            m_ShrineLights[2].SetEnable(true);
        }
        if (m_ChangingStones.Length >= 3)
        {
            m_ChangingStones[0].SetEnable(true);
            m_ChangingStones[1].SetEnable(false);
            m_ChangingStones[2].SetEnable(false);
        }
        if (m_ButtonPrompts.Length >= 2)
        {
            m_ButtonPrompts[0].gameObject.SetActive(false);
            m_ButtonPrompts[1].gameObject.SetActive(false);
        }
        if (m_MoodChanger == null)
        {
            m_MoodChanger = FindObjectOfType<ChangeMood>();
        }
        if (m_MoodChanger != null)
        {
            m_MoodChanger.SetFullCorrupted();
        }
        m_Mothers = FindObjectsOfType<Mother>();
        if (m_Mothers != null && m_Mothers.Length > 0)
        {
            foreach (Mother mother in m_Mothers)
            {
                if (mother.FirstMother == true)
                {
                    mother.gameObject.SetActive(false);
                }
                else
                {
                    mother.SetFullCorrupted();
                }
            }
        }
        m_Portals = new List<LoadSceneTrigger>(FindObjectsOfType<LoadSceneTrigger>());
        foreach (LoadSceneTrigger loadSceneTrigger in m_Portals)
        {
            if (loadSceneTrigger.ShrineNumber < 3)
            {
                loadSceneTrigger.Deactivate();
            }
        }
        m_VisualEffects = new List<VisualEffect>(FindObjectsOfType<VisualEffect>());
        foreach (VisualEffect visualEffect in m_VisualEffects)
        {
            if (m_SnowAsset != null && visualEffect.visualEffectAsset.Equals(m_SnowAsset))
            {
                visualEffect.SetVector3(k_SnowSpeedMax, m_SnowSpeedMaxFullCorrupted);
                visualEffect.SetVector3(k_SnowSpeedMin, m_SnowSpeedMinFullCorrupted);
                visualEffect.SetInt(k_SpawnAmount, m_SpawnAmountFullCorrupted);
            }
        }
    }

    private void SetupClosedLevel()
    {
        m_SoundManager.PlaySound(k_LevelLoaded);
        m_CameraMovement.m_Static = false;
        m_PlayerInput.enabled = true;
        if (m_ShrineLights.Length >= 3)
        {
            m_ShrineLights[0].SetEnable(false);
            m_ShrineLights[1].SetEnable(false);
            m_ShrineLights[2].SetEnable(false);
        }
        if (m_ChangingStones.Length >= 3)
        {
            m_ChangingStones[0].SetEnable(false);
            m_ChangingStones[1].SetEnable(false);
            m_ChangingStones[2].SetEnable(false);
        }
        if (m_ButtonPrompts.Length >= 2)
        {
            m_ButtonPrompts[0].gameObject.SetActive(false);
            m_ButtonPrompts[1].gameObject.SetActive(false);
        }  
    }

    public void RespawnPlayer()
    {
        m_SoundManager.PlaySound(k_LevelLoaded);
        m_PlayerCharacter.transform.position = m_CurrentPlayerSpawnPoint.GetPosition();
    }

    private void MovePlayerToStart()
    {
        m_PlayerCharacter.transform.position = m_CurrentPlayerSpawnPoint.GetPosition();
    }

    private void ResetGameState()
    {
        WorldStateData.ResetState();
        for (int i = 0; i < m_InitialPlayerSpawnPointPosition.Length; i++)
        {
            m_PlayerSpawnPoints[i].transform.position = m_InitialPlayerSpawnPointPosition[i];
        }
    }

    public void ResetGame()
    {
        ResetGameState();
        SceneManager.LoadSceneAsync(WorldStateData.OverWorld);
    }

    public void ResetToMainMenu()
    {
        ResetGameState();
        foreach (GameObject gameObject in m_DontDestroyOnLoadList)
        {
            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
        }
        SceneManager.LoadSceneAsync(WorldStateData.StartMenu);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void RestartLevel()
    {
        if (WorldStateData.CurrentLevel != WorldStateData.StartMenu)
        {
            SceneManager.LoadSceneAsync(WorldStateData.CurrentLevel);
        }
    }

    public void ShrineCompleted()
    {
        WorldStateData.ShrinesCompleted++;
        if (WorldStateData.ShrinesCompleted > 2)
        {
            WorldStateData.ShrinesCompleted = 0;
        }
    }
}

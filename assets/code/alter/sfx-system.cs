using UnityEngine;

public class CharacterSFX : MonoBehaviour
{
    [SerializeField] private PlayerCharacter m_PlayerCharacter;
    [SerializeField] private SoundManager m_PlayerSoundManager;
    [SerializeField] private Shader m_CliffShaderGraph;

    private TerrainReader m_TerrainReader = new TerrainReader();
    private float[] m_TerrainTextureValues;
    private GameObject m_CurrentChargeSFX;

    private const string k_TerrainTag = "Terrain";
    private const string k_SnowStep = "SnowStep";
    private const string k_SnowStep2 = "SnowStep2";
    private const string k_StoneStep = "StoneStep";
    private const string k_StoneStep2 = "StoneStep2";
    private const string k_DirtStep = "DirtStep";
    private const string k_DirtStep2 = "DirtStep2";
    private const string k_WoodStep = "WoodStep";
    private const string k_WoodStep2 = "WoodStep2";
    private const string k_LevelsProperty = "Vector1_5922E2C5"; // Level property on shader Graph_Cliff

    private const string k_Jump = "Jump";
    private const string k_DashRelease = "DashRelease";
    private const string k_DashCharge = "DashCharge";

    public void SFXFootStepEvent(int rightFoot)
    {
        RaycastHit m_FloorHit = m_PlayerCharacter.FloorCheck();
        if (m_FloorHit.collider != null)
        {
            bool snow = false;
            bool wood = false;
            bool stone = false;
            bool dirt = false;
            if (m_FloorHit.collider.gameObject.CompareTag(k_TerrainTag))
            {
                m_TerrainTextureValues = m_TerrainReader.GetTerrainTexture(Terrain.activeTerrain, transform.position);
            }
            else
            {
                MeshRenderer meshRenderer = m_FloorHit.collider.gameObject.GetComponent<MeshRenderer>();
                if (meshRenderer == null)
                {
                    foreach (Transform childTransform in m_FloorHit.collider.gameObject.transform)
                    {
                        meshRenderer = childTransform.GetComponent<MeshRenderer>();
                        if (meshRenderer != null)
                        {
                            break;
                        }
                    }
                }
                if (meshRenderer != null)
                {
                    Material currentMaterial = meshRenderer.sharedMaterial;
                    if (currentMaterial != null && currentMaterial.shader.Equals(m_CliffShaderGraph))
                    {
                        stone = true;
                        if (currentMaterial.GetFloat(k_LevelsProperty) == 0.1f)
                        {
                            dirt = true;
                        }
                        if (currentMaterial.GetFloat(k_LevelsProperty) > 0.1f)
                        {
                            snow = true;
                        }
                    }
                    else
                    {
                        wood = true;
                    }
                }
            }
            if (m_TerrainTextureValues != null && m_TerrainTextureValues.Length > 0)
            {
                // snow terrain
                if (m_TerrainTextureValues[0] > 0f)
                {
                    if (rightFoot > 0)
                    {
                        m_PlayerSoundManager.PlaySound(k_SnowStep, m_PlayerCharacter.transform.position, 
                        m_TerrainTextureValues[0], 1f);
                    }
                    else
                    {
                        m_PlayerSoundManager.PlaySound(k_SnowStep2, m_PlayerCharacter.transform.position, 
                        m_TerrainTextureValues[0], 1f);
                    }
                }
                // stone terrain
                if (m_TerrainTextureValues.Length > 1 && m_TerrainTextureValues[1] > 0f)
                {
                    if (rightFoot > 0)
                    {
                        m_PlayerSoundManager.PlaySound(k_StoneStep, m_PlayerCharacter.transform.position, 
                        m_TerrainTextureValues[1], 1f);
                    }
                    else
                    {
                        m_PlayerSoundManager.PlaySound(k_StoneStep2, m_PlayerCharacter.transform.position, 
                        m_TerrainTextureValues[1], 1f);
                    }
                }
                // dirt terrain
                if (m_TerrainTextureValues.Length > 2 && m_TerrainTextureValues[2] > 0f)
                {
                    if (rightFoot > 0)
                    {
                        m_PlayerSoundManager.PlaySound(k_DirtStep, m_PlayerCharacter.transform.position, 
                        m_TerrainTextureValues[2], 1f);
                    }
                    else
                    {
                        m_PlayerSoundManager.PlaySound(k_DirtStep2, m_PlayerCharacter.transform.position, 
                        m_TerrainTextureValues[2], 1f);
                    }
                }
            }
            if(wood == true)
            {
                if (rightFoot > 0)
                {
                    m_PlayerSoundManager.PlaySound(k_WoodStep, m_PlayerCharacter.transform.position);
                }
                else
                {
                    m_PlayerSoundManager.PlaySound(k_WoodStep2, m_PlayerCharacter.transform.position);
                }
            }
            if (snow == true)
            {
                if (rightFoot > 0)
                {
                    m_PlayerSoundManager.PlaySound(k_SnowStep, m_PlayerCharacter.transform.position);
                }
                else
                {
                    m_PlayerSoundManager.PlaySound(k_SnowStep2, m_PlayerCharacter.transform.position);
                }
            }
            if (stone == true)
            {
                if (rightFoot > 0)
                {
                    m_PlayerSoundManager.PlaySound(k_StoneStep, m_PlayerCharacter.transform.position);
                }
                else
                {
                    m_PlayerSoundManager.PlaySound(k_StoneStep2, m_PlayerCharacter.transform.position);
                }
            }
            if (dirt == true)
            {
                if (rightFoot > 0)
                {
                    m_PlayerSoundManager.PlaySound(k_DirtStep, m_PlayerCharacter.transform.position);
                }
                else
                {
                    m_PlayerSoundManager.PlaySound(k_DirtStep2, m_PlayerCharacter.transform.position);
                }
            }
            m_TerrainTextureValues = null;
        }
    }

    public void SFXJumpEvent()
    {
        if (m_PlayerSoundManager != null)
        {
            m_PlayerSoundManager.PlaySound(k_Jump);
        }
    }

    public void SFXChargeEvent()
    {
        if (m_PlayerSoundManager != null)
        {
            m_CurrentChargeSFX = m_PlayerSoundManager.PlaySound(k_DashCharge);
        }
    }

    public void SFXDestroyChargeEvent()
    {
        if (m_CurrentChargeSFX != null)
        {
            Destroy(m_CurrentChargeSFX);
        }
    }

    public void SFXDashEvent()
    {
        if (m_PlayerSoundManager != null)
        {
            m_PlayerSoundManager.PlaySound(k_DashRelease);
        }
    }
}

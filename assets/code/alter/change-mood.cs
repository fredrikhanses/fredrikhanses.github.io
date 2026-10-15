using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class ChangeMood : MonoBehaviour, IEvent
{
    [Header("Normal")]
    [SerializeField] private VolumeProfile m_NormalProfile;
    [SerializeField] private Color m_NormalColor;
    [Header("Corrupted")]
    [SerializeField] private VolumeProfile m_CorruptedProfile;
    [SerializeField] private Color m_CorruptedLightColor;
    [Header("Full Corrupted")]
    [SerializeField] private VolumeProfile m_FullCorruptedProfile;
    [SerializeField] private Color m_FullCorruptedLightColor;
    [Header("From Scene")]
    [SerializeField] private Light m_Light;
    [SerializeField] private Volume m_PostProcess;

    void Start()
    {
        if (m_PostProcess == null)
        {
            m_PostProcess = FindObjectOfType<Volume>();
        }
        if (m_Light == null)
        {
            m_Light = FindObjectOfType<Light>();
        }
    }

    public void Event()
    {
        SetFullCorrupted();
    }

    public void SetCorrupted()
    {
        m_PostProcess.profile = m_CorruptedProfile;
        m_Light.color = m_CorruptedLightColor;
    }

    public void SetFullCorrupted()
    {
        m_PostProcess.profile = m_FullCorruptedProfile;
        m_Light.color = m_FullCorruptedLightColor;
    }

    public void SetUncorrupted()
    {
        m_Light.color = m_NormalColor;
        m_PostProcess.profile = m_NormalProfile;
    }
}

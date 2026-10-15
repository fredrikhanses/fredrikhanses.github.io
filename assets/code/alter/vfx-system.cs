using System.Collections;
using UnityEditor;
using UnityEngine;

public class CharacterVFX : MonoBehaviour
{
    [Header("Foot Steps")]
    [SerializeField] private ObjectPooler m_DustPooler;
    [SerializeField] private Transform m_LeftFoot;
    [SerializeField] private Transform m_RightFoot;
    [SerializeField, Range(0f, 5f)] private float m_DustLifetime = 1f;
    [Header("Jump")]
    [SerializeField] private ObjectPooler m_JumpPooler;
    [SerializeField, Range(0f, 5f)] private float m_JumpLifetime = 1f;
    [Header("Charge")]
    [SerializeField] private Transform m_Chest;
    [SerializeField] private ObjectPooler m_ChargePooler;
    [SerializeField, Range(0f, 5f)] private float m_ChargeLifetime = 1f;
    [Header("Dash")]
    [SerializeField] private Transform m_Pelvis;
    [SerializeField] private ObjectPooler m_DashPooler;
    [SerializeField, Range(0f, 5f)] private float m_DashLifetime = 1f;
    [Header("Shadow")]
    [SerializeField] private Transform m_Shadow;
    [SerializeField] private PlayerCharacter m_PlayerCharacter;
    [SerializeField] private float ShadowDashUpOffset = 2.0f;
    [SerializeField] private float ShadowDashForwardOffset = 4.5f;
    [SerializeField] private float ShadowForwardOffset = 0.04f;
    private int m_LayerMask = 1 << 9;
    private int m_RaycastDistance = 200;

    private void FixedUpdate()
    {
        DrawShadow();
    }

    private void DrawShadow()
    {
        if (m_PlayerCharacter != null && m_PlayerCharacter.m_Airborne && !m_PlayerCharacter.m_OnLedge)
        {
            m_Shadow.gameObject.SetActive(true);
            RaycastHit surface = FloorCheck(transform.position - transform.up, -transform.up);
            if (surface.collider != null)
            {
                if (m_PlayerCharacter.m_PressingDashing)
                {
                    m_Shadow.position = surface.point + transform.up + transform.forward * ShadowDashForwardOffset
                    * Mathf.Clamp(Vector3.Distance(surface.point, transform.position - transform.up), 
                    0.0f, ShadowDashForwardOffset);
                    RaycastHit downSurface = FloorCheck(new Vector3(m_Shadow.position.x, transform.position.y 
                    + transform.up.y * ShadowDashUpOffset, m_Shadow.position.z), -transform.up);
                    if (downSurface.collider != null)
                    {
                        m_Shadow.position = downSurface.point + transform.up;
                    }
                }
                else
                {
                    m_Shadow.position = surface.point + transform.up + transform.forward * ShadowForwardOffset 
                    * new Vector3(m_PlayerCharacter.PlayerRigidBody.velocity.x, 
                    0.0f, m_PlayerCharacter.PlayerRigidBody.velocity.z).magnitude;
                }
            }
        }
        else if (m_Shadow.gameObject.activeSelf)
        {
            m_Shadow.gameObject.SetActive(false);
        }
    }

    private RaycastHit FloorCheck(Vector3 fromPosition, Vector3 direction)
    {
        Physics.Raycast(fromPosition, direction, out RaycastHit floorHit, m_RaycastDistance, ~m_LayerMask);
        return floorHit;
    }

    public void VFXFootStepEvent(int rightFoot)
    {
        GameObject dustEffect = m_DustPooler.GetPooledObject();
        if (dustEffect != null)
        {
            if (rightFoot > 0)
            {
                dustEffect.transform.position = m_RightFoot.position;
            }
            else
            {
                dustEffect.transform.position = m_LeftFoot.position;
            }
            dustEffect.transform.SetParent(null);
            dustEffect.SetActive(true);
            StartCoroutine(SetInactive(dustEffect, m_DustPooler, m_DustLifetime));
        }
    }

    public void VFXJumpEvent()
    {
        GameObject jumpEffect = m_JumpPooler.GetPooledObject();
        if (jumpEffect != null)
        {
            jumpEffect.transform.position = transform.position;
            jumpEffect.SetActive(true);
            StartCoroutine(SetInactive(jumpEffect, m_JumpPooler, m_JumpLifetime));
        }
    }

    public void VFXDashEvent()
    {
        GameObject dashEffect = m_DashPooler.GetPooledObject();
        if (dashEffect != null)
        {
            dashEffect.transform.position = m_Pelvis.position;
            dashEffect.SetActive(true);
            StartCoroutine(SetInactive(dashEffect, m_DashPooler, m_DashLifetime));
        }
    }

    public void VFXChargeEvent()
    {
        GameObject chargeEffect = m_ChargePooler.GetPooledObject();
        if (chargeEffect != null)
        {
            chargeEffect.transform.position = m_Chest.position;
            chargeEffect.SetActive(true);
            StartCoroutine(SetInactive(chargeEffect, m_ChargePooler, m_ChargeLifetime));
        }
    }

    private IEnumerator SetInactive(GameObject visualEffect, ObjectPooler objectPooler, float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        visualEffect.SetActive(false);
        visualEffect.transform.SetParent(objectPooler.transform);
    }
}

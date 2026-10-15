using UnityEngine;
using UnityEngine.Assertions;

#region Requirements

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(ReadPlayerInput))]

#endregion Requirements

public class Combat : MonoBehaviour
{
    #region Variables

    #region Unity Components

    private Animator animator;

    #endregion Unity Components

    #region Imported Classes

    private PlayerMovement playerMovement;
    private ReadPlayerInput readPlayerInput;

    #endregion Imported Classes

    #region Set In Editor

    [Header("Import from child object")]
    [SerializeField] private Transform firePoint;
    [Header("Import from Prefabs")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject chargedBulletPrefab;
    [Header("Timers")]
    [SerializeField] private float shotTimer = 0.5f;
    [Header("Distances")]
    [SerializeField] private float meleeReach = 0.5f;
    [Header("Damage")]
    [SerializeField] private int attackDamage = 5;
    [Header("Collider Tags")]
    [SerializeField] private string playerTag;

    #endregion Set In Editor

    #region Local

    private float currentShotTime = 0f;
    private bool attack;
    private bool shoot;
    private bool chargedShot;

    #endregion Local

    #endregion Variables

    #region Unity Functions

    private void Start()
    {
        Assert.IsNotNull(bulletPrefab, "Failed to find BulletPrefab GameObject.");
        
        Assert.IsNotNull(chargedBulletPrefab, "Failed to find ChargedBulletPrefab GameObject.");
        
        Assert.IsNotNull(firePoint, "Failed to find FirePoint component.");
        
        animator = GetComponent<Animator>();
        Assert.IsNotNull(animator, "Failed to find Animator component.");

        playerMovement = GetComponent<PlayerMovement>();
        Assert.IsNotNull(playerMovement, "Failed to find PlayerMovement script.");
              
        readPlayerInput = GetComponentInParent<ReadPlayerInput>();
        Assert.IsNotNull(readPlayerInput, "Failed to find ReadPlayerInput script.");
    }

    void Update()
    {
        if(currentShotTime > 0)
        {
            currentShotTime -= Time.deltaTime;
        }
        if (PlaySceneManager.Instance.RoundRunning)
        {
            InputHandler();
        }
    }

    private void FixedUpdate()
    {
        Attack();
        Shoot();
        ChargedShot();
    }

    #endregion Unity Functions

    #region Input

    private void InputHandler()
    {
        if (readPlayerInput.Shoot != Vector2.zero && currentShotTime <= 0)
        {
            shoot = true;
            currentShotTime = shotTimer;
        }

        if (readPlayerInput.AttackRequest)
        {
            attack = true;
        }
        readPlayerInput.AttackRequest = false;

        if (readPlayerInput.ChargedShotPressed && currentShotTime <= 0)
        {
            chargedShot = true;
            currentShotTime = shotTimer;
        }
        readPlayerInput.ChargedShotPressed = false;
    }

    #endregion Input

    #region Actions

    private void Attack()
    {
        if (attack)
        {
            // Todo: try attackPoint.
            animator.SetBool("Attack", true);
            bool facingRight = GetComponent<PlayerMovement>().FacingRight;
            Vector2 fromPosition = new Vector2(facingRight ? transform.position.x + 1 : transform.position.x - 1, 
                                               transform.position.y + 1);
            Vector2 direction = new Vector2(facingRight ? 1 : -1, 0);
            RaycastHit2D raycastHit = Physics2D.Raycast(fromPosition, direction, meleeReach);
            if(raycastHit)
            {
                if(raycastHit.transform.tag.Equals(playerTag) && raycastHit.transform != transform)
                {
                    raycastHit.transform.GetComponent<PlayerCharacter>().TakeDamage(attackDamage);
                }
            }
            attack = false;
        }
        else
        {
            animator.SetBool("Attack", false);
        }
    }

    private void Shoot()
    {
        if (shoot)
        {
            // Todo: Restrict to not shoot straight up or down.
            float yShootDirection = readPlayerInput.Shoot.y;
            playerMovement.CheckDirection();
            animator.SetBool("Shoot", true);
            animator.SetFloat("YShootDirection", yShootDirection);
            GameObject bullet = Instantiate(bulletPrefab, new Vector3(transform.localPosition.x 
                                            + readPlayerInput.Shoot.normalized.x * 2f, transform.localPosition.y + 1f 
                                            + readPlayerInput.Shoot.normalized.y * 2f, 0f), Quaternion.identity);
            bullet.GetComponent<PongmanBullet>().SetDirection(readPlayerInput.Shoot.normalized);
            shoot = false;
        }
        else
        {
            animator.SetBool("Shoot", false);
        }
    }

    private void ChargedShot()
    {
        if (chargedShot && playerMovement.Grounded)
        {
            animator.SetBool("ChargedShot", true);
            GameObject bullet = Instantiate(chargedBulletPrefab, firePoint.position, firePoint.rotation);
            bullet.GetComponent<Bullet>().SetDirection(Vector3.right);
            chargedShot = false;
        }
        else
        {
            animator.SetBool("ChargedShot", false);
        }
    }

    #endregion Actions
}

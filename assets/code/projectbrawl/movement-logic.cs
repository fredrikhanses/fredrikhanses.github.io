using UnityEngine;
using UnityEngine.Assertions;

#region Requirements

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(ReadPlayerInput))]
[RequireComponent(typeof(Rigidbody2D))]

#endregion Requirements

public class PlayerMovement : MonoBehaviour
{
    #region Variables

    #region Unity Components

    private Animator animator;
    private CapsuleCollider2D capsuleCollider;
    private Rigidbody2D playerRigidbody;
    private Transform playerTransform;

    #endregion Unity Components

    #region Imported Classes

    private ReadPlayerInput readPlayerInput;

    #endregion Imported Classes

    #region Set In Editor

    [Header("Choose groundLayers")]
    [SerializeField] private LayerMask groundedMask;
    [Header("Forces")]
    [SerializeField] private float airForce;    
    [SerializeField] private float jumpForce;
    [Header("Speed")]
    [SerializeField] private float speed;
    [SerializeField] private float dashSpeed;    
    [SerializeField] private float maxAirSpeed;    
    [Header("Timers")]    
    [SerializeField] private float dashDrop;
    [SerializeField] private float startDashCooldown;   
    [SerializeField] private float startDashTime;
    [SerializeField] private float startDropTimer;
    [Header("Abilities")]
    [SerializeField] private int startNumberOfDashes;

    #endregion Set In Editor

    #region Public

    public float FacingDirection { get => facingDirection; }
    public bool FacingRight { get => facingRight; }
    public bool Grounded { get => grounded; }
    public int NumberOfDashes { get => numberOfDashes; }

    private float facingDirection;
    private bool facingRight = true;
    private bool grounded;
    private int numberOfDashes;

    #endregion Public

    #region Local

    private float dashCooldown;
    private float dashTime;
    private float dropTimer;    
    private int lastGroundLayer;
    private bool dash;
    private bool drop;
    private bool falling;
    private bool jump;
    private float dashThreshold = 0.2f;
    private float dropThreshold = -0.9f;
    private float groundedBoxLength = 1.5f;
    private float groundedBoxDepth = 0.01f;
    private float rayCastLength = 0.2f;    
    private int groundedLayer = 11;

    #endregion Local

    #endregion Variables

    #region Unity Functions

    private void Start()
    {
        animator = GetComponent<Animator>();
        Assert.IsNotNull(animator, "Failed to find Animator component.");

        capsuleCollider = GetComponent<CapsuleCollider2D>();
        Assert.IsNotNull(capsuleCollider, "Failed to find CapsuleCollider2D component.");

        playerRigidbody = GetComponent<Rigidbody2D>();
        Assert.IsNotNull(playerRigidbody, "Failed to find Rigibody2D component.");

        playerTransform = GetComponent<Transform>();
        Assert.IsNotNull(playerTransform, "Failed to find Transform component.");

        readPlayerInput = GetComponentInParent<ReadPlayerInput>();
        Assert.IsNotNull(readPlayerInput, "Failed to find ReadPlayerInput script.");
        
        ResetDashCooldown();
        ResetDashes();
        ResetDashTime();    
        ResetDropTimer();
    }

    void Update()
    {
        if (PlaySceneManager.Instance.RoundRunning)
        {
            CheckIfGrounded();
            InputHandler();
            DashHandler();
            DropTimer();
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    #endregion Unity Functions

    #region Input

    private void InputHandler()
    {
        // Set if the player is moving or not.
        facingDirection = readPlayerInput.Movement.x;
        animator.SetFloat("Speed", Mathf.Abs(facingDirection));

        if (readPlayerInput.DashRequest)
        {
            if (numberOfDashes > 0)
            {
                dash = true;
            }
            readPlayerInput.DashRequest = false;
        }

        if (readPlayerInput.JumpRequest)
        {
            if (grounded)
            {
                jump = true;
            }
            readPlayerInput.JumpRequest = false;
        }
    }

    #endregion Input

    #region Movement

    private void Move()
    {
        if (readPlayerInput.Movement != Vector2.zero)
        {
            CheckDirection();

            // Change direction in air. Right or left.
            if (falling && facingRight && Mathf.Abs(playerRigidbody.velocity.x) < maxAirSpeed)
            {
                playerRigidbody.AddForce(new Vector2(airForce, 0));
            }
            else if (falling && !facingRight && Mathf.Abs(playerRigidbody.velocity.x) < maxAirSpeed)
            {
                playerRigidbody.AddForce(new Vector2(-airForce, 0));
            }
            // Move right or left when grounded.
            else if (!facingRight && grounded)
            {
                playerRigidbody.velocity = new Vector2(facingDirection * speed, 0f);
            }
            else if (facingRight && grounded)
            {
                playerRigidbody.velocity = new Vector2(facingDirection * speed, 0f);
            }

            if (dash)
            {
                if (numberOfDashes > 0)
                {
                    Dash();
                }
            }
        }

        if (jump)
        {
            Jump();
            jump = false;
        }
    }

    private void Dash()
    {
        if (readPlayerInput.Movement.y > dashThreshold)
        {
            animator.SetBool("DashUp", true);
            animator.SetBool("Dash", false);
            animator.SetBool("Jumping", false);
        }
        else
        {
            animator.SetBool("Dash", true);
            animator.SetBool("DashUp", false);
            animator.SetBool("Jumping", false);
        }

        CheckDirection();

        if (dashTime <= 0)
        {
            dash = false;
            animator.SetBool("Dash", false);
            animator.SetBool("DashUp", false);
            numberOfDashes--;
            ResetDashTime();
            dashDrop = 0.1f;
        }
        else
        {
            playerRigidbody.velocity = readPlayerInput.Movement * dashSpeed;
            dashTime -= Time.fixedDeltaTime;
            playerRigidbody.velocity -= new Vector2(playerRigidbody.velocity.x * dashDrop,
                playerRigidbody.velocity.y * dashDrop);
            dashDrop += 0.1f;
        }
    }

    private void Jump()
    {
        CheckDirection();
        animator.SetBool("Jumping", true);
        playerRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    #endregion Movement

    #region Helper Functions

    // Regenerate a new dash for the player at every 
    // dashCooldown interval, but only to a maximum amount.
    private void DashHandler()
    {
        if (numberOfDashes < startNumberOfDashes)
        {
            dashCooldown -= Time.deltaTime;
            if (dashCooldown <= 0)
            {
                numberOfDashes++;
                ResetDashCooldown();
            }
        }
    }

    private void DropTimer()
    {
        if (drop)
        {
            dropTimer -= Time.deltaTime;
            if (dropTimer <= 0)
            {
                drop = false;
                capsuleCollider.enabled = true;
                ResetDropTimer();
            }
        }
    }

    #region Debug

#if UNITY_EDITOR

    // Function to visualize the grounded box in the scene in Unity.
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, new Vector2(groundedBoxLength, groundedBoxDepth));
    }

#endif

    #endregion Debug

    // Check if the player is grounded.
    private void CheckIfGrounded()
    {
        // Creates a invisible box that returns true if it touches something in the specified LayerMask.
        grounded = Physics2D.OverlapArea(new Vector2(transform.position.x, transform.position.y), 
                                         new Vector2(transform.position.x + groundedBoxLength, 
                                         transform.position.y - groundedBoxDepth), groundedMask);
        if (grounded)
        {
            animator.SetBool("Falling", false);
            animator.SetBool("Jumping", false);
            falling = false;
            CheckDrop();
        }
        else
        {
            falling = true;
            animator.SetBool("Falling", true);
        }
    }

    // Check if the player is facing left or right.
    public void CheckDirection()
    {
        facingDirection = readPlayerInput.Movement.x;
        float shootDirection = readPlayerInput.Shoot.x;
        if ((facingDirection > 0 || shootDirection > 0) && !facingRight)
        {
            transform.Rotate(0f, 180f, 0f);
            facingRight = true;
        }
        else if ((facingDirection < 0 || shootDirection < 0) && facingRight)
        {
            transform.Rotate(0f, 180f, 0f);
            facingRight = false;
        }
    }

    // Check what surface is under the player.
    private void CheckDrop()
    {
        RaycastHit2D groundInfo = Physics2D.Raycast(transform.position, Vector2.down, rayCastLength);
        if (groundInfo)
        {
            lastGroundLayer = groundInfo.collider.gameObject.layer;
        }
        else
        {
            lastGroundLayer = -1; // No layers with indices below 0.
        }

        // Layer 11 is the platforms that the player should be able to drop from.
        // Todo: Fix bug while holding down.
        if (readPlayerInput.Movement.y <= dropThreshold && !drop && lastGroundLayer == groundedLayer) 
        {
            drop = true;
            capsuleCollider.enabled = false;
        }
    }

    #endregion Helper Functions

    #region Reset

    public void ResetDashes()
    {
        numberOfDashes = startNumberOfDashes;
    }

    private void ResetDropTimer()
    {
        dropTimer = startDropTimer;
    }

    private void ResetDashCooldown()
    {
        dashCooldown = startDashCooldown;
    }

    private void ResetDashTime()
    {
        dashTime = startDashTime;
    }

    #endregion Reset
}

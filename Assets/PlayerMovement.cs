using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Movement Settings")]
    [Tooltip("Player Rigidbody2D Component")]
    [SerializeField] private Rigidbody2D myRigidbody;

    [Header("User Movement Settings")]
    [Tooltip("User action settings for jumping and moving")]
    [SerializeField] private InputAction rightMovementButton;
    [SerializeField] private InputAction leftMovementButton;
    [SerializeField] private InputAction jumpMovementButton;

    [Header("User Movement Properties")]
    [Tooltip("Force applied for horizontal movement")]
    [SerializeField] private float moveForce = 50f;
    [Tooltip("Maximum movement speed")]
    [SerializeField] private float maxMoveSpeed = 5f;
    [Tooltip("Force applied when jumping")]
    [SerializeField] private float jumpForce = 12f;
    [Tooltip("Player max height movement")]
    [SerializeField] private float yPositionRangeLimit = 25f;
    [Tooltip("Player horizontal movement limits")]
    [SerializeField] private float xPositionRangeLimit = 48f;

    [Header("Logic Script")]
    [Tooltip("Logic script")]
    [SerializeField] private Logic logicScript;

    // Input state variables
    private bool isAlive = true;
    private float horizontalInput = 0f;
    private bool jumpRequested = false;
    private bool canJump = false;

    // Debug
    private Vector2 lastPosition;
    private readonly float positionChangeThreshold = 0.5f;





    void Start()
    {
        // Ensure Rigidbody2D is assigned
        if (myRigidbody == null)
        {
            myRigidbody = GetComponent<Rigidbody2D>();
            if (myRigidbody == null)
            {
                Debug.LogError("[PlayerMovement] No Rigidbody2D found!");
                enabled = false;
                return;
            }
        }

        // Configure rigidbody settings
        myRigidbody.freezeRotation = true; // Prevent rotation
        myRigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // Enable input actions
        rightMovementButton.Enable();
        leftMovementButton.Enable();
        jumpMovementButton.Enable();

        lastPosition = transform.position;
        Debug.Log("[PlayerMovement] Initialized with settings - Move Force: " + moveForce + ", Jump Force: " + jumpForce);
    }

    void Update()
    {
        if (isAlive)
        {
            // Handle input in Update for responsive controls
            horizontalInput = 0f;

            if (rightMovementButton.IsPressed() && transform.position.x < xPositionRangeLimit)
            {
                horizontalInput = 1f;
            }

            if (leftMovementButton.IsPressed() && transform.position.x > -xPositionRangeLimit)
            {
                horizontalInput = -1f;
            }

            // Check for jump input
            if (jumpMovementButton.WasPressedThisFrame() && canJump && transform.position.y < yPositionRangeLimit)
            {
                jumpRequested = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (isAlive)
        {
            // Apply horizontal movement force
            if (horizontalInput != 0)
            {
                // Only apply force if we're below max speed
                float currentSpeed = Mathf.Abs(myRigidbody.linearVelocity.x);
                if (currentSpeed < maxMoveSpeed)
                {
                    float force = moveForce;
                    // Reduce force as we approach max speed
                    if (currentSpeed > maxMoveSpeed * 0.8f)
                    {
                        force *= (maxMoveSpeed - currentSpeed) / maxMoveSpeed;
                    }
                    myRigidbody.AddForce(new Vector2(horizontalInput * force, 0) * Time.fixedDeltaTime, ForceMode2D.Impulse);
                }
            }
            else
            {
                // Apply friction when not moving horizontally
                float friction = myRigidbody.linearVelocity.x * -0.5f;
                myRigidbody.AddForce(new Vector2(friction, 0) * Time.fixedDeltaTime, ForceMode2D.Impulse);
            }

            // Handle jumping
            if (jumpRequested)
            {
                myRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                jumpRequested = false;
                canJump = false;
                Debug.Log("[PlayerMovement] Jump executed with velocity: " + myRigidbody.linearVelocity.y);
            }

            // Clamp velocity to prevent excessive speeds
            Vector2 clampedVelocity = myRigidbody.linearVelocity;
            clampedVelocity.x = Mathf.Clamp(clampedVelocity.x, -maxMoveSpeed, maxMoveSpeed);
            myRigidbody.linearVelocity = clampedVelocity;

            // Debug movement
            Vector2 currentPosition = transform.position;
            if (Vector2.Distance(currentPosition, lastPosition) > positionChangeThreshold)
            {
                Debug.Log($"[PlayerMovement] Significant position change detected. Delta: {Vector2.Distance(currentPosition, lastPosition)}");
            }
            lastPosition = currentPosition;
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision is from above (ground contact)
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.7f) // Threshold for ground detection
            {
                canJump = true;
                Debug.Log("[PlayerMovement] Ground contact detected - Jump enabled");
                break;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Only disable jumping if we're actually leaving the ground
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.7f)
            {
                canJump = false;
                Debug.Log("[PlayerMovement] Left ground - Jump disabled");
                break;
            }
        }
    }

    private void OnBecameInvisible()
    {
        logicScript.gameOver();
        isAlive = false;
        rightMovementButton.Disable();
        leftMovementButton.Disable();
        jumpMovementButton.Disable();
        Destroy(gameObject, 3f);
    }
}


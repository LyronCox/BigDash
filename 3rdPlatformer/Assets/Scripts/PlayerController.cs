using UnityEngine;
using UnityEngine.InputSystem;

// A small Rigidbody controller for the PreClassCheck lesson.
[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class NewPlayerController : MonoBehaviour
{
    public float walkForce = 20.0f;
    public float speed = 0.0f;
    public float maxSpeed = 20.0f;
    public float jumpForce = 80.0f;
    public int maxJumpCount = 2;
    public int jumpCount = 0;
    public float lookSpeed = 2.0f;
    public bool isGrounded = false;
    public float groundStopMultiplier = 0.5f;
    public float airStopMultiplier = 0.1f;
    public LayerMask resetJumpLayer;
    public Transform pivot;
    public Transform graphics;
    public Animator animator;
    public float graphicsTurnSpeed = 720.0f;
    public float footRadius = 0.5f;
    public bool perfictDirectionChange = false;
    private Rigidbody rb;
    private bool tryToJump = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDrawGizmos()
    {
        if (pivot == null) return;

        // Set the color with custom alpha.
        if (isGrounded)
            Gizmos.color = new Color(0f, 1f, 0f, 0.5f); // Green with custom alpha
        else
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f); // Red with custom alpha

        // Draw the sphere.
        Gizmos.DrawSphere(pivot.position, footRadius);
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;

        isGrounded = Physics.CheckSphere(pivot.position, footRadius, resetJumpLayer, QueryTriggerInteraction.Ignore);

        if (isGrounded)
            jumpCount = 0;

        if (keyboard.spaceKey.wasPressedThisFrame && jumpCount < maxJumpCount)
            tryToJump = true;
        
        // Orbit the camera without turning the player's graphics or Rigidbody.
        pivot.Rotate(Vector3.up * Input.mousePositionDelta.x * lookSpeed * Time.deltaTime, Space.World);

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        bool isMoving = h != 0f || v != 0f;

        if (animator != null)
        {
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetBool("IsMoving", isGrounded && isMoving);
        }

        // Keep the last facing direction while idle; face camera-relative movement when moving.
        if (graphics != null && isMoving)
        {
            Vector3 direction = pivot.forward * v + pivot.right * h;
            Vector3 facing = Vector3.ProjectOnPlane(direction, Vector3.up);
            if (facing.sqrMagnitude > 0.001f)
            {
                graphics.rotation = Quaternion.RotateTowards(graphics.rotation,
                    Quaternion.LookRotation(facing, Vector3.up), graphicsTurnSpeed * Time.deltaTime);
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        UpdateOdometer();

        Vector3 direction = pivot.forward * v;
        direction += pivot.right * h;

        if (direction != Vector3.zero)
        {
            if (perfictDirectionChange)
            {
                Vector3 velocity = rb.linearVelocity;
                float horizontalSpeed = new Vector3(velocity.x, 0f, velocity.z).magnitude;
                Vector3 horizontalDirection = new Vector3(direction.x, 0f, direction.z).normalized;
                rb.linearVelocity = horizontalDirection * horizontalSpeed + Vector3.up * velocity.y;
            }

            rb.AddForce(direction * walkForce, ForceMode.Force);
        }
        else
        {
            if (isGrounded)
            {
                rb.linearVelocity -= rb.linearVelocity * groundStopMultiplier * Time.deltaTime;
            }
            else
            {
                float saveY = rb.linearVelocity.y;
                rb.linearVelocity -= rb.linearVelocity * airStopMultiplier * Time.deltaTime;
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, saveY, rb.linearVelocity.z);
            }
        }
        

        if (tryToJump) {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            tryToJump = false;
            jumpCount++;
        }
    }

    void UpdateOdometer()
    {
        Vector3 zeroYVel = new Vector3(rb.linearVelocity.x, 0.0f, rb.linearVelocity.z);

        speed = zeroYVel.magnitude;

        if (speed > maxSpeed)
        {
            zeroYVel = Vector3.Normalize(zeroYVel) * maxSpeed;
            rb.linearVelocity = new Vector3(zeroYVel.x, rb.linearVelocity.y, zeroYVel.z);
        }
    }
}

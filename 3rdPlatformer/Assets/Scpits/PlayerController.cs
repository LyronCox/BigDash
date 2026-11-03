using Mono.Cecil;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    public float walkForce = 20.0f;
    public float jumpForce = 80.0f;
    public int maxJumpCount = 2;
    public int JumpCounter = 0;
    public float lookspeed = 2.0f;
    public bool isGrounded = false;
    public LayerMask resetJumpLayer;
    public Transform pivot;
    public float footRadius = 0.5f;
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

        //Draw the sphere
        Gizmos.DrawSphere(pivot.position, footRadius);

        //Draw wire shpere outline
        //Gizmos.color = Color.white;
        //Gizmos.DrawWireSphere(pivot.position, footRadius);
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;

        isGrounded = Physics.CheckSphere(pivot.position, footRadius, resetJumpLayer, QueryTriggerInteraction.Ignore);

        if (keyboard.spaceKey.wasPressedThisFrame && JumpCounter < maxJumpCount)
            tryToJump = true;
        if (isGrounded && JumpCounter >= maxJumpCount)
            JumpCounter = 0;   
        

        // Look
        transform.Rotate(Vector3.up * Input.mousePositionDelta.x * lookspeed * Time.deltaTime);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 direction = pivot.forward * v;
        direction += pivot.right * h;

        rb.AddForce(direction * walkForce, ForceMode.Force);

        if(tryToJump){
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            tryToJump = false;
            JumpCounter++;
        }
    }
}

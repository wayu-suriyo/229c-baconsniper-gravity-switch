using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float torqueStrength = 4f;

    public bool canControl = true;

    [Header("Gravity (Custom F=ma)")]
    public float gravityStrength = 9.81f;
    public bool isFlipped = false;

    [Header("Ground Check")]
    public float checkRadius = 0.55f;
    public LayerMask groundLayer;
    private bool isGrounded = false;

    [Header("Audio Settings")]
    public AudioClip flipUpSound;
    public AudioClip flipDownSound;

    [Range(0f, 1f)]
    public float flipVolume = 0.5f;

    private Rigidbody rb;
    private float moveInput = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        canControl = true;
    }

    void OnMove(InputValue value)
    {
        if (!canControl) return;

        moveInput = value.Get<Vector2>().x;
    }

    void OnJump()
    {
        if (!canControl || !isGrounded) return;

        isFlipped = !isFlipped;

        if (isFlipped)
        {
            if (flipUpSound != null)
                AudioSource.PlayClipAtPoint(flipUpSound, transform.position, flipVolume);
        }
        else
        {
            if (flipDownSound != null)
                AudioSource.PlayClipAtPoint(flipDownSound, transform.position, flipVolume);
        }

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, 0);
    }

    void FixedUpdate()
    {
        Vector3 checkDirection = isFlipped ? Vector3.up : Vector3.down;
        isGrounded = Physics.CheckSphere(
            transform.position + checkDirection * 0.5f,
            checkRadius,
            groundLayer
        );

        // §”π«≥·√ß‚πÈ¡∂Ë«ß (F = ma)
        float acceleration = isFlipped ? gravityStrength : -gravityStrength;
        float gravityForce = rb.mass * acceleration;
        rb.AddForce(new Vector3(0, gravityForce, 0), ForceMode.Force);

        if (!canControl)
        {
            float stopX = Mathf.Lerp(rb.linearVelocity.x, 0, Time.fixedDeltaTime * 10f);
            rb.linearVelocity = new Vector3(stopX, rb.linearVelocity.y, 0);
            return;
        }

        float targetVelocityX = moveInput * moveSpeed;
        float lerpSpeed = moveInput != 0 ? 8f : 20f;
        float smoothedX = Mathf.Lerp(rb.linearVelocity.x, targetVelocityX, Time.fixedDeltaTime * lerpSpeed);
        rb.linearVelocity = new Vector3(smoothedX, rb.linearVelocity.y, 0);

        if (isGrounded)
        {
            rb.AddTorque(Vector3.forward * -moveInput * torqueStrength);
        }
    }
}
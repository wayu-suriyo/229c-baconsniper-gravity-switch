using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float torqueStrength = 4f;

    [Header("Gravity (Custom F=ma)")]
    public float gravityStrength = 9.81f;
    public bool isFlipped = false;

    [Header("Ground Check")]
    public float checkRadius = 0.55f;
    public LayerMask groundLayer;
    private bool isGrounded = false;

    [Header("Audio Settings")]
    public AudioClip flipUpSound;   // เสียงตอนแรงโน้มถ่วงกลับหัว (ขึ้นเพดาน)
    public AudioClip flipDownSound; // เสียงตอนแรงโน้มถ่วงปกติ (ลงพื้น)

    [Range(0f, 1f)]
    public float flipVolume = 0.5f;

    private Rigidbody rb;
    private float moveInput = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>().x;
    }

    void OnJump()
    {
        // ถ้าไม่แตะพื้น ห้ามกดสลับ
        if (!isGrounded) return;

        isFlipped = !isFlipped;

        // เช็คว่าตอนนี้แรงโน้มถ่วงไปทางไหน แล้วเล่นเสียงให้ถูกอัน
        if (isFlipped)
        {
            // ลอยขึ้นเพดาน
            if (flipUpSound != null)
                AudioSource.PlayClipAtPoint(flipUpSound, transform.position, flipVolume);
        }
        else
        {
            // ตกลงพื้นปกติ
            if (flipDownSound != null)
                AudioSource.PlayClipAtPoint(flipDownSound, transform.position, flipVolume);
        }

        // reset velocity แกน Y ตอน flip
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

        // F = ma เก็บแต้มฟิสิกส์
        float acceleration = isFlipped ? gravityStrength : -gravityStrength;
        float gravityForce = rb.mass * acceleration;
        rb.AddForce(new Vector3(0, gravityForce, 0), ForceMode.Force);

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
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

    [Header("Audio Settings (Flip)")]
    public AudioClip flipUpSound;
    public AudioClip flipDownSound;
    [Range(0f, 1f)] public float flipVolume = 0.5f;

    [Header("Audio Settings (Movement)")]
    public AudioClip moveSound; // เอาไฟล์เสียงลูปมาใส่ตรงนี้
    [Range(0f, 1f)] public float maxMoveVolume = 0.5f; // ความดังสูงสุดตอนกลิ้งเร็วๆ

    private Rigidbody rb;
    private float moveInput = 0f;
    private AudioSource moveAudioSource; // ตัวเล่นเสียงตอนขยับ

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        canControl = true;

        // สร้าง AudioSource สำหรับเสียงขยับให้มึงอัตโนมัติ ไม่ต้องไปนั่งแอดเอง
        if (moveSound != null)
        {
            moveAudioSource = gameObject.AddComponent<AudioSource>();
            moveAudioSource.clip = moveSound;
            moveAudioSource.loop = true; // สำคัญมาก สั่งให้มันเล่นวนลูป!
            moveAudioSource.playOnAwake = false;
            moveAudioSource.volume = 0f;
            moveAudioSource.Play(); // สั่งเล่นไปเลยแต่ปิดเสียงไว้ก่อน
        }
    }

    void OnMove(InputValue value)
    {
        if (!canControl) return;
        moveInput = value.Get<Vector2>().x;
    }

    void OnJump()
    {
        if (!canControl || !isGrounded) return;
        ForceFlip();
    }

    public void ForceFlip()
    {
        isFlipped = !isFlipped;

        if (isFlipped && flipUpSound != null)
            AudioSource.PlayClipAtPoint(flipUpSound, transform.position, flipVolume);
        else if (!isFlipped && flipDownSound != null)
            AudioSource.PlayClipAtPoint(flipDownSound, transform.position, flipVolume);

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, 0);
    }

    void FixedUpdate()
    {
        Vector3 checkDirection = isFlipped ? Vector3.up : Vector3.down;
        isGrounded = Physics.CheckSphere(transform.position + checkDirection * 0.5f, checkRadius, groundLayer);

        float acceleration = isFlipped ? gravityStrength : -gravityStrength;
        float gravityForce = rb.mass * acceleration;
        rb.AddForce(new Vector3(0, gravityForce, 0), ForceMode.Force);

        if (!canControl)
        {
            float stopX = Mathf.Lerp(rb.linearVelocity.x, 0, Time.fixedDeltaTime * 10f);
            rb.linearVelocity = new Vector3(stopX, rb.linearVelocity.y, 0);
            HandleMoveSound(0f); // สั่งปิดเสียงตอนโดนล็อค
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

        // โยนความเร็วปัจจุบันไปคำนวณเสียง
        HandleMoveSound(rb.linearVelocity.magnitude);
    }

    // ฟังก์ชันจัดการเสียงขยับ
    void HandleMoveSound(float currentSpeed)
    {
        if (moveAudioSource == null) return;

        // ถ้าความเร็วมากกว่า 0.1 ถือว่าขยับอยู่ (ไม่ว่าจะบนพื้นหรือกลางอากาศ)
        if (currentSpeed > 0.1f)
        {
            // เฟดเสียงให้ดังขึ้นเนียนๆ
            moveAudioSource.volume = Mathf.Lerp(moveAudioSource.volume, maxMoveVolume, Time.deltaTime * 10f);
            // ปรับเสียงแหลมขึ้นนิดนึงตามความเร็ว แม่งจะดูซิ่งขึ้น!
            moveAudioSource.pitch = 1f + (currentSpeed * 0.03f);
        }
        else
        {
            // ถ้าหยุดนิ่ง ให้เฟดเสียงเบาลงจนเงียบ
            moveAudioSource.volume = Mathf.Lerp(moveAudioSource.volume, 0f, Time.deltaTime * 15f);
        }
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float torqueStrength = 4f;

    [Header("Gravity (Custom F=ma)")]
    public float gravityStrength = 9.81f;
    private bool isFlipped = false;

    [Header("Ground Check")]
    public float checkRadius = 0.55f;
    public LayerMask groundLayer;
    private bool isGrounded = false;

    private Rigidbody rb;
    private float moveInput = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // ปิด Use Gravity ของ Unity เพราะเราจะคำนวณเองเอาคะแนน
        rb.useGravity = false;
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>().x;
    }

    void OnJump()
    {
        if (!isGrounded) return;

        isFlipped = !isFlipped;

        // reset velocity แกน Y ตอน flip ให้การตกดูสม่ำเสมอ
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, 0);
    }

    void FixedUpdate()
    {
        // 1. เช็คพื้น
        Vector3 checkDirection = isFlipped ? Vector3.up : Vector3.down;
        isGrounded = Physics.CheckSphere(
            transform.position + checkDirection * 0.5f,
            checkRadius,
            groundLayer
        );

        // 2. คำนวณและใส่แรงโน้มถ่วงด้วยตัวเอง (F = ma) -> เก็บแต้มหัวข้อ C + Coding 2 คะแนนเต็ม
        float acceleration = isFlipped ? gravityStrength : -gravityStrength;
        float gravityForce = rb.mass * acceleration; // สูตร F = ma
        rb.AddForce(new Vector3(0, gravityForce, 0), ForceMode.Force);

        // 3. การเคลื่อนที่แกน X
        float targetVelocityX = moveInput * moveSpeed;
        float lerpSpeed = moveInput != 0 ? 8f : 20f;
        float smoothedX = Mathf.Lerp(rb.linearVelocity.x, targetVelocityX, Time.fixedDeltaTime * lerpSpeed);
        rb.linearVelocity = new Vector3(smoothedX, rb.linearVelocity.y, 0);

        // 4. กลิ้งเฉพาะตอนแตะพื้น (เก็บแต้มหัวข้อ F)
        if (isGrounded)
        {
            rb.AddTorque(Vector3.forward * -moveInput * torqueStrength);
        }
    }
}
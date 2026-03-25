using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public Transform spawnPoint;
    public LayerMask groundLayer;
    public float maxRayDistance = 30f;  // ระยะ Raycast หาพื้น/เพดาน
    public float timeBeforeRespawn = 2f; // รอกี่วิก่อน respawn

    private PlayerControl playerControl;
    private Rigidbody rb;
    private float outOfBoundsTimer = 0f;

    void Start()
    {
        playerControl = GetComponent<PlayerControl>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        bool hitFloor = Physics.Raycast(transform.position, Vector3.down, maxRayDistance, groundLayer);
        bool hitCeiling = Physics.Raycast(transform.position, Vector3.up, maxRayDistance, groundLayer);

        // ถ้าไม่เจอพื้นหรือเพดานเลยทั้งคู่ = หลุดออกนอกด่านแน่ๆ
        if (!hitFloor && !hitCeiling)
        {
            outOfBoundsTimer += Time.deltaTime;

            if (outOfBoundsTimer >= timeBeforeRespawn)
                Respawn();
        }
        else
        {
            // เจออย่างใดอย่างหนึ่ง = ยังอยู่ในด่าน reset timer
            outOfBoundsTimer = 0f;
        }
    }

    void Respawn()
    {
        transform.position = spawnPoint.position;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        playerControl.isFlipped = false;
        outOfBoundsTimer = 0f;
    }
}
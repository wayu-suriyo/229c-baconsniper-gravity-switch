using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;

    [Header("Zoom")]
    public float minZ = -8f;   // ใกล้สุด (ด่านแคบ)
    public float maxZ = -20f;  // ไกลสุด (ด่านกว้าง)
    public float minHeight = 4f;  // ความสูงด่านขั้นต่ำที่คาดไว้
    public float maxHeight = 16f; // ความสูงด่านสูงสุดที่คาดไว้

    [Header("Layer Setup")]
    public LayerMask groundLayer; // เอาไว้กัน Raycast ไปชนเหรียญหรือหนาม

    private PlayerControl playerController;
    private float currentLevelHeight;

    void Start()
    {
        // แก้ให้ชื่อตัวแปรตรงกัน
        playerController = target.GetComponent<PlayerControl>();
    }

    void FixedUpdate()
    {
        // ตั้งค่าเผื่อไว้เวลาหลุดแมพ กล้องจะได้ไม่บัคปลิวไปที่ระยะ 999
        float distToFloor = 5f;
        float distToCeiling = 5f;

        // เติมระยะ ray 50f และ LayerMask ให้มันหาแต่พื้นจริงๆ
        if (Physics.Raycast(target.position, Vector3.down, out RaycastHit hitFloor, 50f, groundLayer))
            distToFloor = hitFloor.distance;

        if (Physics.Raycast(target.position, Vector3.up, out RaycastHit hitCeiling, 50f, groundLayer))
            distToCeiling = hitCeiling.distance;

        // ความสูงรวมของด่าน
        currentLevelHeight = distToFloor + distToCeiling;

        // แปลงความสูงเป็นค่า zoom (0 = แคบสุด, 1 = กว้างสุด)
        float t = Mathf.InverseLerp(minHeight, maxHeight, currentLevelHeight);
        float targetZ = Mathf.Lerp(minZ, maxZ, t);

        // Y อยู่กึ่งกลางระหว่างพื้นกับเพดานเสมอ
        float floorY = target.position.y - distToFloor;
        float ceilingY = target.position.y + distToCeiling;
        float targetY = (floorY + ceilingY) / 2f;

        Vector3 targetPos = new Vector3(
            target.position.x,
            Mathf.Lerp(transform.position.y, targetY, Time.fixedDeltaTime * smoothSpeed),
            Mathf.Lerp(transform.position.z, targetZ, Time.fixedDeltaTime * smoothSpeed)
        );

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.fixedDeltaTime * smoothSpeed);
    }
}
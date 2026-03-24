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

    private PlayerControl playerControl;
    private float currentLevelHeight;

    void Start()
    {
        playerControl = target.GetComponent<PlayerControl>();
    }

    void FixedUpdate()
    {
        // ยิง Raycast หาพื้นและเพดานจากตำแหน่งบอล
        float distToFloor = 999f;
        float distToCeiling = 999f;

        if (Physics.Raycast(target.position, Vector3.down, out RaycastHit hitFloor))
            distToFloor = hitFloor.distance;

        if (Physics.Raycast(target.position, Vector3.up, out RaycastHit hitCeiling))
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
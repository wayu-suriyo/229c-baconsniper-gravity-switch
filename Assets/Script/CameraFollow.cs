using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    [Header("Camera Speeds")]
    public float smoothSpeed = 5f; // ความเร็วกล้องตามผู้เล่น
    public float zoomSpeed = 3f;   // ความเร็วตอนซูม (แยกออกมาจะได้ปรับให้ซูมนุ่มๆ ได้)

    [Header("Zoom Settings")]
    public float minZ = -8f;
    public float maxZ = -20f;
    public float minHeight = 4f;
    public float maxHeight = 16f;

    [Header("Raycast Penetration")]
    public float rayDistance = 50f;
    public LayerMask groundLayer;

    [Header("Anti-Jerk (กันกล้องกระชาก)")]
    public float fallbackDistance = 5f; // ระยะซูมเริ่มต้นถ้าหาพื้นไม่เจอ
    public float memoryTime = 0.5f;     // เวลาที่กล้องจะ "จำ" ระยะเดิมไว้ (วินาที)

    // ตัวแปรความจำกล้อง
    private float lastGoodFloorY;
    private float lastGoodCeilingY;
    private float floorTimer;
    private float ceilingTimer;

    void Start()
    {
        if (target != null)
        {
            // ตั้งค่าเริ่มต้นกันเหนียว
            lastGoodFloorY = target.position.y - fallbackDistance;
            lastGoodCeilingY = target.position.y + fallbackDistance;
        }
    }

    void FixedUpdate()
    {
        if (target == null) return;

        // เช็คพื้นล่าง
        RaycastHit[] downHits = Physics.RaycastAll(target.position, Vector3.down, rayDistance, groundLayer);
        bool foundFloor = false;
        float lowestFloorY = target.position.y; // ตั้งต้นไว้ที่ตัวผู้เล่น

        foreach (RaycastHit hit in downHits)
        {
            if (!foundFloor || hit.point.y < lowestFloorY)
            {
                lowestFloorY = hit.point.y;
                foundFloor = true;
            }
        }

        // ระบบความจำ (ถ้าเจอพื้นให้จำไว้ ถ้าไม่เจอให้รอแป๊บนึง)
        if (foundFloor)
        {
            lastGoodFloorY = lowestFloorY;
            floorTimer = 0f; // รีเซ็ตเวลา
        }
        else
        {
            floorTimer += Time.fixedDeltaTime;
            if (floorTimer <= memoryTime)
                lowestFloorY = lastGoodFloorY; // ยังไม่หมดเวลา ใช้ค่าเดิมไปก่อน
            else
                lowestFloorY = target.position.y - fallbackDistance; // หมดเวลาแล้ว ยอมแพ้ กลับไปใช้ค่า default
        }

        // 2. เช็คเพดานบน
        RaycastHit[] upHits = Physics.RaycastAll(target.position, Vector3.up, rayDistance, groundLayer);
        bool foundCeiling = false;
        float highestCeilingY = target.position.y;

        foreach (RaycastHit hit in upHits)
        {
            if (!foundCeiling || hit.point.y > highestCeilingY)
            {
                highestCeilingY = hit.point.y;
                foundCeiling = true;
            }
        }

        if (foundCeiling)
        {
            lastGoodCeilingY = highestCeilingY;
            ceilingTimer = 0f;
        }
        else
        {
            ceilingTimer += Time.fixedDeltaTime;
            if (ceilingTimer <= memoryTime)
                highestCeilingY = lastGoodCeilingY;
            else
                highestCeilingY = target.position.y + fallbackDistance;
        }

        // 3. คำนวณความสูงและเป้าหมาย
        float currentLevelHeight = highestCeilingY - lowestFloorY;
        float t = Mathf.InverseLerp(minHeight, maxHeight, currentLevelHeight);
        float targetZ = Mathf.Lerp(minZ, maxZ, t);
        float targetY = (lowestFloorY + highestCeilingY) / 2f;

        // 4. สั่งกล้องขยับ (แยกความเร็ว Zoom ออกมาให้เนียนขึ้น)
        Vector3 targetPos = new Vector3(
            target.position.x,
            Mathf.Lerp(transform.position.y, targetY, Time.fixedDeltaTime * smoothSpeed),
            Mathf.Lerp(transform.position.z, targetZ, Time.fixedDeltaTime * zoomSpeed) // ซูมแยกกันละ
        );

        transform.position = targetPos;
    }
}
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    public Transform target;

    [Header("Camera Speeds")]
    public float smoothSpeed = 5f;
    public float zoomSpeed = 3f;

    [Header("Auto Zoom (FOV-based)")]
    public float zoomPaddingMultiplier = 1.3f;

    public float minClampZ = -8f;   
    public float maxClampZ = -25f;  

    [Header("Raycast Penetration")]
    public float rayDistance = 50f;
    public LayerMask groundLayer;

    [Header("Anti-Jerk")]
    public float fallbackDistance = 5f; // ระยะซูมเริ่มต้นถ้าหาพื้นไม่เจอ
    public float memoryTime = 0.5f;

    private Camera cam;
    private float lastGoodFloorY;
    private float lastGoodCeilingY;
    private float floorTimer;
    private float ceilingTimer;

    void Start()
    {
        cam = GetComponent<Camera>();

        if (cam == null)
        {
            Debug.LogError("Error");
            enabled = false;
            return;
        }

        if (target != null)
        {
            lastGoodFloorY = target.position.y - fallbackDistance;
            lastGoodCeilingY = target.position.y + fallbackDistance;
        }
    }

    void FixedUpdate()
    {
        if (target == null || cam == null) return;

        // ระบบ Raycast
        // เช็คพื้นล่าง
        RaycastHit[] downHits = Physics.RaycastAll(target.position, Vector3.down, rayDistance, groundLayer);
        bool foundFloor = false;
        float lowestFloorY = target.position.y;

        foreach (RaycastHit hit in downHits)
        {
            if (!foundFloor || hit.point.y < lowestFloorY)
            {
                lowestFloorY = hit.point.y;
                foundFloor = true;
            }
        }

        if (foundFloor)
        {
            lastGoodFloorY = lowestFloorY;
            floorTimer = 0f;
        }
        else
        {
            floorTimer += Time.fixedDeltaTime;
            if (floorTimer <= memoryTime) lowestFloorY = lastGoodFloorY;
            else lowestFloorY = target.position.y - fallbackDistance;
        }

        // เช็คเพดานบน
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
            if (ceilingTimer <= memoryTime) highestCeilingY = lastGoodCeilingY;
            else highestCeilingY = target.position.y + fallbackDistance;
        }


        // คำนวณความสูงรวมของด่าน
        float currentLevelHeight = highestCeilingY - lowestFloorY;

        // คำนวณระยะซูม Z ที่ทำให้มองเห็นความสูงนี้พอดี
        // สูตร: ระยะ = ความสูง / (2 * tan(FOV/2))
        float fovRad = cam.fieldOfView * Mathf.Deg2Rad;
        float baseRequiredDistance = currentLevelHeight / (2.0f * Mathf.Tan(fovRad / 2.0f));

        // ใส่ระยะเผื่อ Padding เพื่อให้ภาพดูไม่แน่นเกินไป
        float desiredDistance = baseRequiredDistance * zoomPaddingMultiplier;

        // แปลงเป็นค่าแกน Z
        float targetZ = target.position.z - desiredDistance;

        // Clamping กันกล้องบั๊กมุดดิน
        targetZ = Mathf.Clamp(targetZ, maxClampZ, minClampZ);

        // หาจุดกึ่งกลางด่าน (แกน Y)
        float targetY = (lowestFloorY + highestCeilingY) / 2f;

        // สั่งกล้องขยับแบบ Smooth
        Vector3 targetPos = new Vector3(
            target.position.x,
            Mathf.Lerp(transform.position.y, targetY, Time.fixedDeltaTime * smoothSpeed),
            Mathf.Lerp(transform.position.z, targetZ, Time.fixedDeltaTime * zoomSpeed)
        );

        transform.position = targetPos;
    }
}
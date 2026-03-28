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
    public float fallbackDistance = 5f;
    public float memoryTime = 0.5f;

    [HideInInspector]
    public BoxCollider currentFocusZone; 

    private Camera cam;
    private float lastGoodFloorY;
    private float lastGoodCeilingY;
    private float floorTimer;
    private float ceilingTimer;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (target != null)
        {
            lastGoodFloorY = target.position.y - fallbackDistance;
            lastGoodCeilingY = target.position.y + fallbackDistance;
        }
    }

    void FixedUpdate()
    {
        if (target == null || cam == null) return;

        float lowestFloorY = target.position.y;
        float highestCeilingY = target.position.y;

        // เช็คว่าอยู่ใน Focus Zone ไหม
        if (currentFocusZone != null)
        {
            // ถ้าอยู่ในโซน ให้เข้าม Raycast
            lowestFloorY = currentFocusZone.bounds.min.y;
            highestCeilingY = currentFocusZone.bounds.max.y;
        }
        else
        {
            // ถ้าไม่อยู่ในโซน ค่อยใช้ระบบ Raycast ตามปกติ
            RaycastHit[] downHits = Physics.RaycastAll(target.position, Vector3.down, rayDistance, groundLayer);
            bool foundFloor = false;

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

            RaycastHit[] upHits = Physics.RaycastAll(target.position, Vector3.up, rayDistance, groundLayer);
            bool foundCeiling = false;

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
        }

        // FOV Math 
        float currentLevelHeight = highestCeilingY - lowestFloorY;
        float fovRad = cam.fieldOfView * Mathf.Deg2Rad;
        float baseRequiredDistance = currentLevelHeight / (2.0f * Mathf.Tan(fovRad / 2.0f));
        float desiredDistance = baseRequiredDistance * zoomPaddingMultiplier;
        float targetZ = target.position.z - desiredDistance;

        targetZ = Mathf.Clamp(targetZ, maxClampZ, minClampZ);
        float targetY = (lowestFloorY + highestCeilingY) / 2f;

        Vector3 targetPos = new Vector3(
            target.position.x,
            Mathf.Lerp(transform.position.y, targetY, Time.fixedDeltaTime * smoothSpeed),
            Mathf.Lerp(transform.position.z, targetZ, Time.fixedDeltaTime * zoomSpeed)
        );

        transform.position = targetPos;
    }
}
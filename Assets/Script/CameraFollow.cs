using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 10f;

    [Header("Zoom")]
    public float minZ = -8f;
    public float maxZ = -20f;
    public float edgePadding = 1.5f;

    [Header("Raycast")]
    public float rayDistance = 50f;
    public LayerMask groundLayer;

    [Header("Fallback")]
    public float defaultFloorDistance = 5f;
    public float defaultCeilingDistance = 5f;

    [Header("Anti Jitter")]
    public float missingGraceTime = 0.25f;   // หายชั่วคราวให้ใช้ค่าก่อนหน้าไปก่อน
    public float zoomSmoothTime = 0.15f;     // ยิ่งมากยิ่งนุ่ม
    public float positionSmoothTime = 0.12f; // ความนิ่งของตำแหน่งกล้อง

    private Camera cam;

    private float lastGoodBottomY;
    private float lastGoodTopY;
    private float lastGoodTime = -999f;
    private bool hasLastGoodBounds = false;

    private float zVelocity;
    private float yVelocity;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (target == null || cam == null) return;

        float bottomY, topY;
        bool hasBounds = TryGetVerticalBounds(out bottomY, out topY);

        // ถ้าเจอครบ 2 ฝั่ง หรือเจอค่าที่เชื่อถือได้ ให้บันทึกไว้
        if (hasBounds)
        {
            lastGoodBottomY = bottomY;
            lastGoodTopY = topY;
            lastGoodTime = Time.time;
            hasLastGoodBounds = true;
        }
        else if (hasLastGoodBounds && Time.time - lastGoodTime <= missingGraceTime)
        {
            // หายแป๊บเดียว ใช้ค่าล่าสุดไปก่อน
            bottomY = lastGoodBottomY;
            topY = lastGoodTopY;
        }
        else
        {
            // ไม่มีข้อมูลจริง ใช้ fallback
            bottomY = target.position.y - defaultFloorDistance;
            topY = target.position.y + defaultCeilingDistance;
        }

        bottomY -= edgePadding;
        topY += edgePadding;

        float span = Mathf.Max(0.1f, topY - bottomY);
        float centerY = (bottomY + topY) * 0.5f;

        float halfSpan = span * 0.5f;
        float distance = halfSpan / Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);

        float targetZ = Mathf.Clamp(target.position.z - distance, maxZ, minZ);

        float smoothY = Mathf.SmoothDamp(transform.position.y, centerY, ref yVelocity, positionSmoothTime);
        float smoothZ = Mathf.SmoothDamp(transform.position.z, targetZ, ref zVelocity, zoomSmoothTime);

        Vector3 desiredPos = new Vector3(target.position.x, smoothY, smoothZ);

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPos,
            Time.deltaTime * smoothSpeed
        );
    }

    bool TryGetVerticalBounds(out float bottomY, out float topY)
    {
        bottomY = target.position.y;
        topY = target.position.y;

        bool foundDown = false;
        bool foundUp = false;

        RaycastHit[] downHits = Physics.RaycastAll(
            target.position,
            Vector3.down,
            rayDistance,
            groundLayer,
            QueryTriggerInteraction.Ignore
        );

        for (int i = 0; i < downHits.Length; i++)
        {
            float y = downHits[i].point.y;
            if (!foundDown || y < bottomY)
            {
                bottomY = y;
                foundDown = true;
            }
        }

        RaycastHit[] upHits = Physics.RaycastAll(
            target.position,
            Vector3.up,
            rayDistance,
            groundLayer,
            QueryTriggerInteraction.Ignore
        );

        for (int i = 0; i < upHits.Length; i++)
        {
            float y = upHits[i].point.y;
            if (!foundUp || y > topY)
            {
                topY = y;
                foundUp = true;
            }
        }

        // ต้องมีทั้งบนและล่างถึงจะถือว่า "bounds ดี"
        return foundDown && foundUp;
    }
}
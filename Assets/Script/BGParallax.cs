using UnityEngine;

public class BGParallax : MonoBehaviour
{
    [Header("Camera & Speed Settings")]
    public Transform cameraTransform;

    [Tooltip("0 = ภาพอยู่นิ่งๆ, 1 = ตามกล้องเป๊ะ 100%, 0.5 = ตามกล้องครึ่งนึง (ดูลึกมีมิติ)")]
    [Range(0f, 1f)]
    public float parallaxEffectMultiplier = 0.8f;

    private Vector3 lastCameraPosition;

    void Start()
    {
        // ถ้าไม่ได้ลากกล้องมาใส่ ให้มันหา Main Camera เองอัตโนมัติ
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        lastCameraPosition = cameraTransform.position;
    }

    void LateUpdate() // ใช้ LateUpdate เพื่อให้กล้องขยับเสร็จก่อน แล้ว BG ค่อยขยับตาม (กันภาพกระตุก)
    {
        // คำนวณว่าเฟรมนี้กล้องขยับไปเท่าไหร่
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;

        // เอาความคลาดเคลื่อนมาคูณกับตัวคูณ ยิ่งน้อย ภาพยิ่งตามกล้องช้า (ดูเหมือนอยู่ไกล)
        // ล็อคแกน Y ไว้ให้เป็น 0 ถ้าไม่อยากให้ภาพขยับขึ้นลงตามตอนมึงกระโดด (แต่ถ้าอยากให้ขยับด้วยก็เปลี่ยน 0 เป็น deltaMovement.y * multiplier)
        transform.position += new Vector3(deltaMovement.x * parallaxEffectMultiplier, deltaMovement.y * parallaxEffectMultiplier, 0);

        // จำค่าตำแหน่งกล้องไว้ใช้เฟรมต่อไป
        lastCameraPosition = cameraTransform.position;
    }
}
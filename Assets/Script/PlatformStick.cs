using UnityEngine;

public class PlatformStick : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // สั่งให้ไปเป็นลูกของ transform.parent (ตัว Empty)
            collision.gameObject.transform.SetParent(transform.parent);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // ยกเลิกการเกาะตอนกระโดดออก
            collision.gameObject.transform.SetParent(null);
        }
    }
}
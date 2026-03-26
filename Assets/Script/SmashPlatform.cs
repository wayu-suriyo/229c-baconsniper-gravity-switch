using UnityEngine;

public class SmashPlatform : MonoBehaviour
{
    [Header("Smash Settings")]
    public float breakVelocityThreshold = 8f;

    [Header("Audio Settings")]
    public AudioClip smashSound;
    [Range(0f, 1f)]
    public float volume = 0.8f;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            float impactForce = collision.relativeVelocity.magnitude;

            if (impactForce >= breakVelocityThreshold)
            {
                if (smashSound != null)
                {
                    AudioSource.PlayClipAtPoint(smashSound, transform.position, volume);
                }

                // 1. สั่งปิดกล่อง Collider ทันที! ระบบฟิสิกส์จะได้หยุดคำนวณการเด้งในเฟรมนี้
                GetComponent<Collider>().enabled = false;

                // 2. ดึงสคริปต์บอลมาบังคับความเร็วทะลวงแผ่น
                Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
                PlayerControl playerCtrl = collision.gameObject.GetComponent<PlayerControl>();

                if (playerRb != null && playerCtrl != null)
                {
                    // เช็คว่าตอนนี้แรงโน้มถ่วงไปทางไหน (พลิกลอยขึ้นเพดาน = 1, ตกพื้น = -1)
                    float direction = playerCtrl.isFlipped ? 1f : -1f;

                    // จับความแรงที่ชนเมื่อกี้ (impactForce) มายัดใส่แกน Y ให้พุ่งทะลุไปทางนั้นเลย!
                    playerRb.linearVelocity = new Vector3(playerRb.linearVelocity.x, impactForce * direction, 0);
                }

                // 3. ทำลายแผ่นทิ้ง
                Destroy(gameObject);
            }
            
        }
    }
}
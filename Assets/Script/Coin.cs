using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Coin Gain")]
    public int scoreValue = 1;

    [Header("Sound")]
    public AudioClip collectSound;

    // [Range(0f, 1f)] จะสร้างแถบสไลเดอร์ให้มึงเลื่อนปรับเสียงใน Unity ได้เลย
    [Range(0f, 1f)]
    public float soundVolume = 0.5f; // ค่าเริ่มต้นตั้งไว้ครึ่งนึง (0.5)

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (collectSound != null)
            {
                // เติม soundVolume ไว้ข้างหลังสุด เพื่อกำหนดความดัง
                AudioSource.PlayClipAtPoint(collectSound, transform.position, soundVolume);
            }

            // บวกแต้มและทำลายเหรียญ
            GameManager.instance.AddScore(scoreValue);
            Destroy(gameObject);
        }
    }
}
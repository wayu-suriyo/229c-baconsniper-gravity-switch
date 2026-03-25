using UnityEngine;

public class Spike : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip deathSound;
    [Range(0f, 1f)]
    public float soundVolume = 0.5f;

    void OnTriggerEnter(Collider other)
    {
        // เช็คว่าคนที่มาโดนหนามคือ Player ใช่ไหม
        if (other.CompareTag("Player"))
        {
            // เล่นเสียงตอนตาย
            if (deathSound != null)
            {
                AudioSource.PlayClipAtPoint(deathSound, transform.position, soundVolume);
            }

            // ดึงสคริปต์ RespawnManager จากตัว Player แล้วสั่งให้มัน Respawn
            RespawnManager respawn = other.GetComponent<RespawnManager>();
            if (respawn != null)
            {
                respawn.Respawn();
            }
        }
    }
}
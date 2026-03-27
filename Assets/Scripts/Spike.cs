using UnityEngine;

public class Spike : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip deathSound;
    [Range(0f, 1f)]
    public float soundVolume = 0.5f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (deathSound != null)
            {
                AudioSource.PlayClipAtPoint(deathSound, transform.position, soundVolume);
            }

            RespawnManager respawn = other.GetComponent<RespawnManager>();
            if (respawn != null)
            {
                respawn.Respawn();
            }
        }
    }
}
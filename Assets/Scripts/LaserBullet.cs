using UnityEngine;

public class LaserBullet : MonoBehaviour
{
    public float lifeTime = 3f;

    [Header("Hit Audio")]
    public AudioClip playerHitSound;
    [Range(0f, 1f)]
    public float soundVolume = 0.5f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CameraFocusZone>() != null)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            if (playerHitSound != null)
            {
                AudioSource.PlayClipAtPoint(playerHitSound, transform.position, soundVolume);
            }

            RespawnManager respawn = other.GetComponent<RespawnManager>();
            if (respawn != null)
            {
                respawn.Respawn();
            }
        }
        else if (other.GetComponent<SmashPlatform>() != null)
        {
            Destroy(other.gameObject);
        }

        if (!other.CompareTag("Coin") && !other.CompareTag("GravityPad"))
        {
            Destroy(gameObject);
        }
    }
}
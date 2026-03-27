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

                GetComponent<Collider>().enabled = false;
                Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
                PlayerControl playerCtrl = collision.gameObject.GetComponent<PlayerControl>();

                if (playerRb != null && playerCtrl != null)
                {
                    float direction = playerCtrl.isFlipped ? 1f : -1f;
                    playerRb.linearVelocity = new Vector3(playerRb.linearVelocity.x, impactForce * direction, 0);
                }
                Destroy(gameObject);
            }
            
        }
    }
}
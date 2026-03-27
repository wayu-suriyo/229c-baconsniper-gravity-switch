using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Coin Gain")]
    public int scoreValue = 1;

    [Header("Sound")]
    public AudioClip collectSound;

    [Range(0f, 1f)]
    public float soundVolume = 0.5f; 

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position, soundVolume);
            }
            GameManager.instance.AddScore(scoreValue);
            Destroy(gameObject);
        }
    }
}
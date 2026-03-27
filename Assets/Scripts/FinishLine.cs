using UnityEngine;
using System.Collections;

public class FinishLine : MonoBehaviour
{
    [Header("Delay Settings")]
    public float delayBeforeSummary = 1f;

    [Header("Audio Settings")]
    public AudioClip winSound;
    [Range(0f, 1f)]
    public float soundVolume = 0.5f;

    private bool hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;

            // ดึงสคริปต์ PlayerControl สั่งล็อคขา
            PlayerControl player = other.GetComponent<PlayerControl>();
            if (player != null)
            {
                player.canControl = false;
            }

            // สั่งหยุดเวลา UI
            GameManager.instance.StopTimer();

            // เข้าสู่การหน่วงเวลาเพื่อรอเปิดหน้าต่างสรุป
            StartCoroutine(ShowSummaryRoutine());
        }
    }

    IEnumerator ShowSummaryRoutine()
    {
        if (winSound != null)
        {
            AudioSource.PlayClipAtPoint(winSound, transform.position, soundVolume);
        }

        yield return new WaitForSeconds(delayBeforeSummary);

        GameManager.instance.ShowLevelComplete();
    }
}
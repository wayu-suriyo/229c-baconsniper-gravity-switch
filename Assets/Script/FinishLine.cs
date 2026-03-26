using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{
    [Header("Scene Settings")]
    public string nextSceneName = "";

    // ตั้งค่าว่าจะให้หน่วงกี่วินาที
    public float delayBeforeLoad = 2f;

    [Header("Audio Settings")]
    public AudioClip winSound;
    [Range(0f, 1f)]
    public float soundVolume = 0.5f;

    // ตัวแปรกันบั๊กชนซ้ำ
    private bool hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        // เช็คว่าเป็น Player และยังไม่เคยชนเส้นชัยนี้มาก่อน
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true; // ไม่ให้ทำงานซ้ำ

            // เรียกใช้งานระบบหน่วงเวลา
            StartCoroutine(LoadNextSceneRoutine());
        }
    }

    // ฟังก์ชันแบบ Coroutine เอาไว้สั่งงานเรียงลำดับและรอเวลา
    IEnumerator LoadNextSceneRoutine()
    {
        // 1. เล่นเสียงเข้าเส้นชัย
        if (winSound != null)
        {
            AudioSource.PlayClipAtPoint(winSound, transform.position, soundVolume);
        }

        // 2. สั่งให้หยุดรอตามเวลาที่ตั้งไว้
        yield return new WaitForSeconds(delayBeforeLoad);

        // 3. พอรอครบเวลาค่อยสลับด่าน
        SceneManager.LoadScene(nextSceneName);
    }
}
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    void Awake() { instance = this; }

    [Header("UI References")]
    public TMP_Text scoreText;
    public TMP_Text timeText;

    [Header("Level Progress")]
    public GameObject finishLineObject; // ช่องสำหรับใส่ประตูเส้นชัย

    private int currentScore = 0;
    private int totalCoins = 0; // จำนวน Plasma ทั้งหมดในด่าน
    private float currentTime = 0f;
    private bool isGameActive = true;

    void Start()
    {
        // นับจำนวนของที่มี Tag Coin ทั้งหมดในด่านตอนเริ่มเกม
        totalCoins = GameObject.FindGameObjectsWithTag("Coin").Length;

        // ซ่อนเส้นชัยไว้ก่อนตั้งแต่เริ่ม
        if (finishLineObject != null)
        {
            finishLineObject.SetActive(false);
        }

        UpdateScoreUI();
    }

    void Update()
    {
        if (isGameActive)
        {
            currentTime += Time.deltaTime;
            UpdateTimeUI();
        }
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        UpdateScoreUI();

        // เช็คว่าเก็บครบยัง
        if (currentScore >= totalCoins)
        {
            // ถ้าเก็บครบแล้ว ให้เปิดใช้งานเส้นชัย
            if (finishLineObject != null)
            {
                finishLineObject.SetActive(true);
            }
        }
    }

    private void UpdateScoreUI()
    {
        scoreText.text = "Plasma: " + currentScore + " / " + totalCoins;
    }

    private void UpdateTimeUI()
    {
        timeText.text = "Time: " + currentTime.ToString("F2") + " s";
    }
}
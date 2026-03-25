using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    void Awake() { instance = this; }

    [Header("UI References")]
    public TMP_Text scoreText;
    public TMP_Text timeText;

    private int currentScore = 0;
    private float currentTime = 0f;
    private bool isGameActive = true;

    void Start()
    {
        // อัปเดต UI ตั้งแต่เริ่มเกม
        UpdateScoreUI();
    }

    void Update()
    {
        // จับเวลาไปเรื่อยๆ ถ้าเกมยังเล่นอยู่
        if (isGameActive)
        {
            currentTime += Time.deltaTime;
            UpdateTimeUI();
        }
    }

    // ฟังก์ชันนี้เอาไว้ให้เหรียญเรียกใช้ตอนโดนเก็บ
    public void AddScore(int amount)
    {
        currentScore += amount;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        scoreText.text = "Score: " + currentScore.ToString();
    }

    private void UpdateTimeUI()
    {
        // ToString("F2") คือให้แสดงทศนิยม 2 ตำแหน่ง จะได้ดูเท่ๆ
        timeText.text = "Time: " + currentTime.ToString("F2") + " s";
    }
}
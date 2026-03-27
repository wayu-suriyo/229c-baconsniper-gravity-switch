using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; 

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    void Awake() { instance = this; }

    [Header("UI References")]
    public TMP_Text scoreText;
    public TMP_Text timeText;

    [Header("Level Progress")]
    public GameObject finishLineObject;

    [Header("Level Complete UI")]
    public GameObject levelCompletePanel; 
    public TMP_Text summaryTimeText;      

    private int currentScore = 0;
    private int totalCoins = 0;
    private float currentTime = 0f;
    public bool isGameActive = true;

    void Start()
    {
        totalCoins = GameObject.FindGameObjectsWithTag("Coin").Length;

        if (finishLineObject != null) finishLineObject.SetActive(false);

        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);

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

        if (currentScore >= totalCoins)
        {
            if (finishLineObject != null) finishLineObject.SetActive(true);
        }
    }

    public void StopTimer()
    {
        isGameActive = false;
    }

    public void ShowLevelComplete()
    {
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
            summaryTimeText.text = "Clear Time: " + currentTime.ToString("F2") + " s";
        }
    }

    public void LoadNextStage(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
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
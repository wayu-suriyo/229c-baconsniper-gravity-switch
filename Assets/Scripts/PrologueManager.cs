using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class PrologueManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text storyText;
    public TMP_Text status1Text;
    public TMP_Text status2Text;
    public TMP_Text status3Text;
    public GameObject startButton;

    [Header("Story Settings")]
    [TextArea(3, 5)]
    public string fullStory = "YEAR 2142. THE SYNDICATE CONTROLS THE WORLD THROUGH FEAR.\nYOU ARE THE ANOMALY. AN EXPERIMENTAL ANTI-GRAVITY CORE STOLEN BY THE RESISTANCE.\nTHEY WANT YOU BACK. WE NEED YOU TO RUN.";

    public float typeSpeed = 0.05f;
    public float delayBetweenLines = 1f; 

    [Header("Next Scene")]
    public string nextSceneName = "Level01";

    void Start()
    {
        storyText.text = "";

        status1Text.text = "SECURITY PROTOCOL: ACTIVE";
        status2Text.text = "UNAUTHORIZED BREACH DETECTED. ASSET: GRAVITY CORE";
        status3Text.text = "DIRECTIVE: ESCAPE FACILITY. GATHER PLASMA TO INITIALIZE DIMENSIONAL GATE.";

        status1Text.gameObject.SetActive(false);
        status2Text.gameObject.SetActive(false);
        status3Text.gameObject.SetActive(false);
        startButton.SetActive(false);

        StartCoroutine(PlayPrologue());
    }

    IEnumerator PlayPrologue()
    {
        yield return new WaitForSeconds(1f);

        foreach (char c in fullStory)
        {
            storyText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        yield return new WaitForSeconds(delayBetweenLines);

        status1Text.gameObject.SetActive(true);
        yield return new WaitForSeconds(delayBetweenLines);

        status2Text.gameObject.SetActive(true);
        yield return new WaitForSeconds(delayBetweenLines);

        status3Text.gameObject.SetActive(true);
        yield return new WaitForSeconds(delayBetweenLines);

        startButton.SetActive(true);
    }

    public void GoToNextLevel()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
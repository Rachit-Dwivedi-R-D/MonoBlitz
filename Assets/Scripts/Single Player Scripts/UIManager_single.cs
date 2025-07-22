using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager_Single : MonoBehaviour
{
    public static UIManager_Single instance;

    [SerializeField] private TMP_Text countText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text gameOverScoreText;

    [SerializeField, Space(10)] private Image healthBar;
    [SerializeField, Space(10)] private GameObject gameOverPanel;
    [SerializeField, Space(10)] private Button retryButton;

    [Header("Pause Functionality")]
    [SerializeField] private GameObject pausePanel; // ✨ Assign your pause panel
    private bool isPaused = false;

    [SerializeField, Space(10)] private float maxCountTime = 5f;

    private float countTimer;
    private int score;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance);
    }

    private void Start()
    {
        scoreText.gameObject.SetActive(false);
        scoreText.text = score.ToString();

        pausePanel?.SetActive(false); // ✨ Hide pause panel on start
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (GameManager_Single.instance.GameState == GameState.Preparing)
            CountTime();

        if (GameManager_Single.instance.GameState == GameState.GameOver)
            retryButton.interactable = true;

        // ✨ Optional: Press Esc to toggle pause
        if (Input.GetKeyDown(KeyCode.Escape) && GameManager_Single.instance.GameState == GameState.Playing)
            TogglePause();
    }

    private void CountTime()
    {
        if (countTimer > maxCountTime)
        {
            GameManager_Single.instance.GameState = GameState.Playing;
            countText.gameObject.SetActive(false);
            healthBar.transform.parent.gameObject.SetActive(true);
            scoreText.gameObject.SetActive(true);
            countTimer = 0;
            return;
        }

        countTimer += Time.deltaTime;
        countText.text = "Starting in ... " + (maxCountTime - countTimer).ToString("0");
    }

    public void UpdateHealthBar(float value)
    {
        healthBar.fillAmount = value / 100f;
    }

    public void AddScore(int value)
    {
        score += value;
        scoreText.text = score.ToString();
        scoreText.GetComponent<PopUp>()?.Popup();
    }

    public void GameOver()
    {
        healthBar.transform.parent.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(false);
        gameOverPanel.SetActive(true);
        gameOverScoreText.text = "Score: " + score.ToString();
    }

    public void Retry()
    {
        AudioManager.instance.PlaySoundEffect("Hit");
        Time.timeScale = 1f; // ✨ Make sure time resumes
        SceneManager.LoadScene("Single Player Game");
    }

    public void OpenMenu()
    {
        AudioManager.instance.PlaySoundEffect("Hit");
        Time.timeScale = 1f; // ✨ Make sure time resumes
        SceneManager.LoadScene("Menu");
    }

    // ✨ Called by Pause Button
    public void TogglePause()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    // ✨ Called by Resume button inside PausePanel
    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }
}

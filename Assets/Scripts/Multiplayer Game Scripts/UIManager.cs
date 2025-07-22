using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField]
    private TMP_Text countText;

    [SerializeField]
    private TMP_Text scoreText;

    [SerializeField]
    private TMP_Text gameOverScoreText;

    [SerializeField, Space(10)]
    private Image healthBar;

    [SerializeField, Space(10)]
    private GameObject gameOverPanel;

    [SerializeField]
    private GameObject warningGameObject;

    [SerializeField, Space(10)]
    private Button retryButton;

    [SerializeField, Space(10)]
    private float maxCountTime;

    private float countTimer;

    private int score;

    private PhotonView view;

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

        view = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if(GameManager.instance.GameState == GameState.Preparing)
            CountTime();

        if (GameManager.instance.GameState == GameState.GameOver)
            UpdateRetryButton();
    }

    private void CountTime()
    {
        if(countTimer > maxCountTime)
        {
            GameManager.instance.GameState = GameState.Playing;
            countText.gameObject.SetActive(false);
            healthBar.transform.parent.gameObject.SetActive(true);
            scoreText.gameObject.SetActive(true);
            countTimer = 0;
        }

        countTimer += Time.deltaTime;

        countText.text = "Starting in ...." + (5 - countTimer).ToString("0");
    }

    public void UpdateHealthBar(float value)
    {
        healthBar.fillAmount = value / 100;
    }

    public void AddScore(int value)
    {
        score += value;

        scoreText.text = score.ToString();

        scoreText.GetComponent<PopUp>().Popup();
    }

    public void GameOver()
    {
        healthBar.transform.parent.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(false);
        gameOverPanel.SetActive(true);

        retryButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        warningGameObject.SetActive(!PhotonNetwork.IsMasterClient);

        gameOverScoreText.text = "Score: " + score.ToString();
    }

    public void Retry()
    {
        AudioManager.instance.PlaySoundEffect("Hit");
        view.RPC("RPC_Retry", RpcTarget.All);
    }

    [PunRPC]
    public void RPC_Retry()
    {
        PhotonNetwork.LoadLevel("Multiplayer Game");
    }

    public void OpenMenu()
    {
        AudioManager.instance.PlaySoundEffect("Hit");
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("Menu");
    }

    private void UpdateRetryButton()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            retryButton.enabled = false;
            retryButton.GetComponent<Image>().color = new Color(0.7f, 0.7f, 0.7f);
        }
        else
        {
            retryButton.enabled = true;
            retryButton.GetComponent<Image>().color = Color.white;
        }
    }
}

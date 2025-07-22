using Photon.Pun;
using UnityEngine;

public enum GameState
{
    Preparing,
    Playing,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private GameState gameState;

    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private GameObject player;

    [Space(10)]
    public Vector2 min, max;

    private GameObject spawnedPlayer;
    private bool isDisconnected;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject); // correctly destroy duplicate
            return;
        }

        gameState = GameState.Preparing;
    }

    private void Start()
    {
        isDisconnected = false;
    }

    private void Update()
    {
        if (isDisconnected)
            return;

        Disconnect();
    }

    private void SpawnPlayer()
    {
        Vector2 randomPos = new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
        spawnedPlayer = PhotonNetwork.Instantiate(player.name, randomPos, Quaternion.identity);
    }

    public void GameOver()
    {
        gameState = GameState.GameOver;

        if (spawnedPlayer != null)
            spawnedPlayer.SetActive(false);

        // ✅ Play GameOver sound only on local player
        if (PhotonNetwork.LocalPlayer.IsLocal)
            AudioManager.instance?.PlaySoundEffect("gameover");
    }

    public void Disconnect()
    {
        if (GameState == GameState.Playing && PhotonNetwork.PlayerList.Length == 1)
        {
            isDisconnected = true;
            UIManager.instance.OpenMenu();
        }
    }

    public GameState GameState
    {
        get => gameState;
        set
        {
            gameState = value;

            if (gameState == GameState.Playing)
                SpawnPlayer();
        }
    }

    public CameraShake GetCameraShake()
    {
        return cameraShake;
    }
}

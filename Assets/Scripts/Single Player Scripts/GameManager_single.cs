using UnityEngine;

public class GameManager_Single : MonoBehaviour
{
    public static GameManager_Single instance;

    private GameState gameState;

    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private GameObject player;

    [Space(10)]
    public Vector2 min, max;

    private GameObject spawnedPlayer;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        gameState = GameState.Preparing;

        if (player == null)
            Debug.LogError("GameManager_Single: Player prefab is not assigned!");

        if (cameraShake == null)
            Debug.LogWarning("GameManager_Single: CameraShake is not assigned!");
    }

    private void Start()
{
    // ✅ FORCE STOP background music on single-player scene start
    AudioManager.instance?.StopMusic();

    GameState = GameState.Preparing;
}


    private void SpawnPlayer()
    {
        if (player == null)
            return;

        Vector2 randomPos = new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
        spawnedPlayer = Instantiate(player, randomPos, Quaternion.identity);
        
    }

    public void GameOver()
{
    gameState = GameState.GameOver;

    if (spawnedPlayer != null)
    {
        spawnedPlayer.SetActive(false);
        
    }

    AudioManager.instance?.PlaySoundEffect("gameover"); // 🔊 Play Game Over sound
}

    public GameState GameState
    {
        get => gameState;
        set
        {
            gameState = value;
            

            if (gameState == GameState.Playing)
            {
                SpawnPlayer();
            }
            else if (gameState == GameState.GameOver)
            {
                // Add game over logic if needed
            }
            else if (gameState == GameState.Preparing)
            {
                // Add preparing logic if needed
            }
        }
    }

    public CameraShake GetCameraShake()
    {
        return cameraShake;
    }
}

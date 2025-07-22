using Photon.Pun;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private float maxTime;
    private float timer;

    [Space(10)]
    [SerializeField]
    private GameObject[] spawnPoints;

    private void Start()
    {
        timer = 0;
    }

    private void Update()
    {
        // ✅ Only MasterClient spawns enemies
        if (!PhotonNetwork.IsMasterClient)
            return;

        // ✅ Allow spawning if there's at least 1 player
        if (PhotonNetwork.CurrentRoom.PlayerCount < 1)
            return;

        if (GameManager.instance.GameState != GameState.Playing)
            return;

        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        if (timer > maxTime)
        {
            GameObject _enemy = null;

            float rand = Random.value;

            if (rand <= 0.66f)
                _enemy = ObjectPool.instance.GetFollowEnemy();
            else if (rand <= 0.90f)
                _enemy = ObjectPool.instance.GetShootingEnemy();
            else
                _enemy = ObjectPool.instance.GetlaserEnemy();

            if (_enemy == null)
            {
                Debug.LogWarning("EnemySpawner: No enemy returned from pool!");
                return;
            }

            Vector3 pos = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;

            EnemyController enemyController = _enemy.GetComponent<EnemyController>();
            enemyController.SendInfo(true, pos);
            enemyController.ResetInfo();

            timer = 0;
        }

        timer += Time.deltaTime;
    }
}

using UnityEngine;

public class EnemySpawner_Single : MonoBehaviour
{
    [SerializeField] private float maxTime;
    private float timer;

    [Space(10)]
    [SerializeField] private GameObject[] spawnPoints;

    private void Start()
    {
        timer = 0;
    }

    private void Update()
    {
  if (GameManager_Single.instance.GameState != GameState.Playing)

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
            _enemy = ObjectPool_Single.instance.GetFollowEnemy();
        else if (rand > 0.66f && rand <= 0.90f)
            _enemy = ObjectPool_Single.instance.GetShootingEnemy();
        else
            _enemy = ObjectPool_Single.instance.GetlaserEnemy();

        if (_enemy == null)
        {
            Debug.LogWarning("EnemySpawner_Single: Failed to get enemy from pool.");
            return;
        }

        Vector3 pos = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;

        var controller = _enemy.GetComponent<EnemyController_Single>();
        if (controller != null)
        {
            controller.SendInfo(true, pos);
            controller.ResetInfo();
        }
        else
        {
            Debug.LogError("EnemySpawner_Single: Enemy prefab missing EnemyController_Single script.");
        }

        timer = 0;
    }

    timer += Time.deltaTime;
}

}

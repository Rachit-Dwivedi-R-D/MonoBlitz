using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    private List<GameObject> bulletPool;
    private List<GameObject> deathParliclesPool;
    private List<GameObject> followEnemiesPool;
    private List<GameObject> shootingEnemiesPool;
    private List<GameObject> laserEnemiesPool;

    [Header("Pool Sizes")]
    [SerializeField] private float maxBullets;
    [SerializeField] private float maxDeathParlicles;
    [SerializeField] private float maxFollowEnemies;
    [SerializeField] private float maxShootingEnemies;
    [SerializeField] private float maxlaserEnemies;

    [Header("Prefabs")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject deathParlicle;
    [SerializeField] private GameObject followEnemy;
    [SerializeField] private GameObject shootingEnemy;
    [SerializeField] private GameObject laserEnemy;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        bulletPool = new List<GameObject>();
        deathParliclesPool = new List<GameObject>();
        followEnemiesPool = new List<GameObject>();
        shootingEnemiesPool = new List<GameObject>();
        laserEnemiesPool = new List<GameObject>();

        InstantiateBullets();
        InstantiateDeathParticles();

        if (!PhotonNetwork.IsMasterClient)
            return;

        InstantiateFollowEnemies();
        InstantiateShootingEnemies();
        InstantiateLaserEnemies();

        Debug.Log($"[ObjectPool] Enemy pools initialized. Follow: {followEnemiesPool.Count}, Shooting: {shootingEnemiesPool.Count}, Laser: {laserEnemiesPool.Count}");
    }

    private void InstantiateBullets()
    {
        for (int i = 0; i < maxBullets; i++)
        {
            GameObject _bullet = Instantiate(bullet);
            _bullet.SetActive(false);
            bulletPool.Add(_bullet);
        }
    }

    private void InstantiateDeathParticles()
    {
        for (int i = 0; i < maxDeathParlicles; i++)
        {
            GameObject _particle = Instantiate(deathParlicle);
            _particle.GetComponent<ParticleSystem>().Stop();
            deathParliclesPool.Add(_particle);
        }
    }

    private void InstantiateFollowEnemies()
    {
        for (int i = 0; i < maxFollowEnemies; i++)
        {
            GameObject _enemy = PhotonNetwork.Instantiate(followEnemy.name, Vector3.zero, Quaternion.identity);
            _enemy.GetComponent<EnemyController>().SendInfo(false, Vector3.zero);
            followEnemiesPool.Add(_enemy);
        }
    }

    private void InstantiateShootingEnemies()
    {
        for (int i = 0; i < maxShootingEnemies; i++)
        {
            GameObject _enemy = PhotonNetwork.Instantiate(shootingEnemy.name, Vector3.zero, Quaternion.identity);
            _enemy.GetComponent<EnemyController>().SendInfo(false, Vector3.zero);
            shootingEnemiesPool.Add(_enemy);
        }
    }

    private void InstantiateLaserEnemies()
    {
        for (int i = 0; i < maxlaserEnemies; i++)
        {
            GameObject _enemy = PhotonNetwork.Instantiate(laserEnemy.name, Vector3.zero, Quaternion.identity);
            _enemy.GetComponent<EnemyController>().SendInfo(false, Vector3.zero);
            laserEnemiesPool.Add(_enemy);
        }
    }

    public GameObject GetBullets()
    {
        foreach (GameObject bullet in bulletPool)
        {
            if (!bullet.activeInHierarchy)
                return bullet;
        }
        Debug.LogWarning("[ObjectPool] All bullets are active!");
        return null;
    }

    public GameObject GetDeathParticle()
    {
        foreach (GameObject particle in deathParliclesPool)
        {
            if (particle.GetComponent<ParticleSystem>().isStopped)
                return particle;
        }
        Debug.LogWarning("[ObjectPool] No available death particles!");
        return null;
    }

    public GameObject GetFollowEnemy()
    {
        foreach (GameObject enemy in followEnemiesPool)
        {
            if (!enemy.activeInHierarchy)
            {
                Debug.Log("[ObjectPool] Returning a follow enemy.");
                return enemy;
            }
        }
        Debug.LogWarning("[ObjectPool] All follow enemies are active!");
        return null;
    }

    public GameObject GetShootingEnemy()
    {
        foreach (GameObject enemy in shootingEnemiesPool)
        {
            if (!enemy.activeInHierarchy)
            {
                Debug.Log("[ObjectPool] Returning a shooting enemy.");
                return enemy;
            }
        }
        Debug.LogWarning("[ObjectPool] All shooting enemies are active!");
        return null;
    }

    public GameObject GetlaserEnemy()
    {
        foreach (GameObject enemy in laserEnemiesPool)
        {
            if (!enemy.activeInHierarchy)
            {
                Debug.Log("[ObjectPool] Returning a laser enemy.");
                return enemy;
            }
        }
        Debug.LogWarning("[ObjectPool] All laser enemies are active!");
        return null;
    }
}

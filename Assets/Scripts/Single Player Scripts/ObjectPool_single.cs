using System.Collections.Generic;
using UnityEngine;

public class ObjectPool_Single : MonoBehaviour
{
    public static ObjectPool_Single instance;

    private List<GameObject> bulletPool;
    private List<GameObject> deathParliclesPool;
    private List<GameObject> followEnemiesPool;
    private List<GameObject> shootingEnemiesPool;
    private List<GameObject> laserEnemiesPool;

    [SerializeField] private float maxBullets;
    [SerializeField] private float maxDeathParlicles;
    [SerializeField] private float maxFollowEnemies;
    [SerializeField] private float maxShootingEnemies;
    [SerializeField] private float maxlaserEnemies;

    [Space(10)]
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
            Destroy(gameObject);
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
        InstantiateFollowEnemies();
        InstantiateShootingEnemies();
        InstantiateLaserEnemies();
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
            GameObject _followEnemy = Instantiate(followEnemy);
            _followEnemy.GetComponent<EnemyController_Single>()?.SendInfo(false, Vector3.zero);
;
            followEnemiesPool.Add(_followEnemy);
        }
    }

    private void InstantiateShootingEnemies()
{
    for (int i = 0; i < maxShootingEnemies; i++)
    {
        GameObject _shootingEnemy = Instantiate(shootingEnemy);
        _shootingEnemy.GetComponent<EnemyController_Single>()?.SendInfo(false, Vector3.zero);
        shootingEnemiesPool.Add(_shootingEnemy);
    }
}

private void InstantiateLaserEnemies()
{
    for (int i = 0; i < maxlaserEnemies; i++)
    {
        GameObject _laserEnemy = Instantiate(laserEnemy);
        _laserEnemy.GetComponent<EnemyController_Single>()?.SendInfo(false, Vector3.zero);


        laserEnemiesPool.Add(_laserEnemy);
    }
}


    public GameObject GetBullets()
    {
        foreach (var b in bulletPool)
            if (!b.activeInHierarchy)
                return b;
        return null;
    }

    public GameObject GetDeathParticle()
    {
        foreach (var p in deathParliclesPool)
            if (p.GetComponent<ParticleSystem>().isStopped)
                return p;
        return null;
    }

    public GameObject GetFollowEnemy()
    {
        foreach (var e in followEnemiesPool)
            if (!e.activeInHierarchy)
                return e;
        return null;
    }

    public GameObject GetShootingEnemy()
    {
        foreach (var e in shootingEnemiesPool)
            if (!e.activeInHierarchy)
                return e;
        return null;
    }

    public GameObject GetlaserEnemy()
    {
        foreach (var e in laserEnemiesPool)
            if (!e.activeInHierarchy)
                return e;
        return null;
    }
}

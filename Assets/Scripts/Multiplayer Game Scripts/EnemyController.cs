using Photon.Pun;
using UnityEngine;

public abstract class EnemyController : MonoBehaviour
{
    public float moveSpeed;
    public float maxHealth;

    [HideInInspector]
    public CameraShake cameraShake;

    [HideInInspector]
    public GameObject target;

    [HideInInspector]
    public PlayerController[] players;

    [HideInInspector]
    public SpriteRenderer spriteRenderer;

    [HideInInspector]
    public PhotonView view;

    [HideInInspector]
    public PopUp popUp;
   protected float m_health;

protected int once;
    public float Health
    {
        get 
        { 
            return m_health;
        }
        set 
        { 
            m_health = value;
        }
    }

    private void Awake()
{
    spriteRenderer = GetComponent<SpriteRenderer>();
    popUp = GetComponent<PopUp>();


    view = GetComponent<PhotonView>();
    if (view == null)
        Debug.LogError($"{gameObject.name}: Missing PhotonView component!");


    if (GameManager.instance != null)
        cameraShake = GameManager.instance.GetCameraShake();
    else if (GameManager_Single.instance != null)
        cameraShake = GameManager_Single.instance.GetCameraShake();
    else
        Debug.LogError("EnemyController: No GameManager found!");

    m_health = maxHealth;
    once = 0;
}

    private void Start()
    {
        players = FindObjectsOfType<PlayerController>();

        target = GetClosestTarget();
    }

    public void Die()
    {
        view.RPC("RPC_Die", RpcTarget.All);
    }

    [PunRPC]
    public void RPC_Die()
    {
        if (once >= 1)
            return;

        once++;

        UIManager.instance.AddScore(50);

        cameraShake.TriggerShake(0.2f, 0.35f);

        AudioManager.instance.PlaySoundEffect("Die");

        gameObject.SetActive(false);

        GameObject deathParticle = ObjectPool.instance.GetDeathParticle();

        if (deathParticle)
        {
            deathParticle.transform.position = transform.position;
            deathParticle.GetComponent<ParticleSystem>().Play();
        }
    }

    public abstract void Follow();

    public abstract void Attack();

    public  void TakeDamage(float value)
    {
        Health -= value;

        spriteRenderer.color = Color.white;

        popUp.Popup();

        Invoke("ResetColor", 0.075f);

        if (Health <= 0) Die();
        else AudioManager.instance.PlaySoundEffect("Hit");

    }

    public Vector2 GetDirection(GameObject _target)
{
    if (_target == null)
        return Vector2.zero;

    Vector2 direction = _target.transform.position - transform.position;
    direction.Normalize();
    return direction;
}


    public float GetDistance(GameObject _target)
{
    if (_target == null)
        return float.MaxValue; // Or just return 0 safely

    return Vector2.Distance(transform.position, _target.transform.position);
}


    public GameObject GetClosestTarget()
{
    if (players == null || players.Length == 0)
        return null;

    if (players.Length == 1)
        return players[0].gameObject;

    float dis_1 = Vector2.Distance(transform.position, players[0].transform.position);
    float dis_2 = Vector2.Distance(transform.position, players[1].transform.position);

    return dis_1 < dis_2 ? players[0].gameObject : players[1].gameObject;
}

    public virtual void SendInfo(bool active, Vector3 pos)
{
    view.RPC("RPC_SendInfo", RpcTarget.AllBuffered, active, pos);
}

    [PunRPC]
    public void RPC_SendInfo(bool active, Vector3 pos)
    {
        gameObject.SetActive(active);
        transform.position = pos;
    }

    public virtual void ResetInfo()
{
    if (view != null)
        view.RPC("RPC_ResetInfo", RpcTarget.AllBuffered);
}


    [PunRPC]
    public void RPC_ResetInfo()
    {
        Health = maxHealth;
        once = 0;
    }
}

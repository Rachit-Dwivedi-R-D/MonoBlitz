using UnityEngine;
using System.Collections;

public abstract class EnemyController_Single : MonoBehaviour
{
    public float moveSpeed;
    public float maxHealth;

    [HideInInspector] public CameraShake cameraShake;
    [HideInInspector] public GameObject target;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public PopUp popUp;
    [HideInInspector] public SinglePlayerController player;

    protected float m_health;
    protected int once;

    public float Health
    {
        get => m_health;
        set => m_health = value;
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        popUp = GetComponent<PopUp>();

        if (GameManager_Single.instance != null)
            cameraShake = GameManager_Single.instance.GetCameraShake();
        else
            Debug.LogError("EnemyController_Single: No GameManager_Single found!");

        m_health = maxHealth;
        once = 0;
    }

    protected virtual void Start()
    {
        StartCoroutine(WaitForPlayer());
    }

    private IEnumerator WaitForPlayer()
    {
        while (FindObjectOfType<SinglePlayerController>() == null)
            yield return null;

        player = FindObjectOfType<SinglePlayerController>();
        if (player != null)
            target = player.gameObject;
        else
            Debug.LogError("EnemyController_Single: No SinglePlayerController found.");
    }

    public void Die()
    {
        if (once >= 1)
            return;

        once++;

        if (UIManager_Single.instance != null)
            UIManager_Single.instance.AddScore(50);

        if (cameraShake != null)
            cameraShake.TriggerShake(0.2f, 0.35f);

        AudioManager.instance.PlaySoundEffect("Die");

        gameObject.SetActive(false);

        GameObject deathParticle = ObjectPool_Single.instance.GetDeathParticle();
        if (deathParticle != null)
        {
            deathParticle.transform.position = transform.position;
            deathParticle.GetComponent<ParticleSystem>().Play();
        }
    }

    public void TakeDamage(float value)
    {
        Health -= value;

        spriteRenderer.color = Color.white;
        popUp.Popup();

        Invoke(nameof(ResetColor), 0.075f);

        if (Health <= 0)
            Die();
        else
            AudioManager.instance.PlaySoundEffect("Hit");
    }

    private void ResetColor()
    {
        spriteRenderer.color = new Color(0.2509804f, 1f, 0.9088863f); // original color
    }

    public Vector2 GetDirection(Transform _target)
    {
        return (_target.position - transform.position).normalized;
    }

    public float GetDistance(Transform _target)
    {
        return Vector2.Distance(transform.position, _target.position);
    }

    public virtual void ResetInfo()
    {
        Health = maxHealth;
        once = 0;
    }

    public virtual void SendInfo(bool active, Vector3 pos)
    {
        gameObject.SetActive(active);
        transform.position = pos;
    }

    public Transform GetClosestTarget()
    {
        GameObject found = GameObject.FindGameObjectWithTag("Player");
        return found != null ? found.transform : null;
    }

    public abstract void Follow();
    public abstract void Attack();
}

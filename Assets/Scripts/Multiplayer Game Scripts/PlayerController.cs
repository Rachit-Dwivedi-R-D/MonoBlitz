using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerController : MonoBehaviourPun
{
    [SerializeField] private float speed;
    private float minX, maxX, minY, maxY;
    private float clampPadding = 0.5f;

    private Vector2 moveDirection;

    [SerializeField, Space(10)] private float bulletSpeed;
    [SerializeField, Range(0f, 10f)] private float bulletSpread;
    [SerializeField, Range(0f, 10f)] private float rotateAngle;
    [SerializeField] public float maxHealth;

    [HideInInspector] public PopUp popUp;

    private Camera cam;
    private PhotonView view;
    private float m_health;
    private CameraShake cameraShake;

    private SpriteRenderer spriteRenderer;

    public float Health
    {
        get => m_health;
        set => m_health = value;
    }

    // Fallback color palette
    private readonly Color[] PlayerColors = new Color[]
    {
        Color.green,
        Color.yellow,
        new Color(1f, 0.5f, 0f), // orange
        Color.magenta,
        Color.blue,
    };

    private void Awake()
    {
        // Ensure spriteRenderer is available before RPCs may fire
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        CursorManager.instance?.SetCrosshairCursor();
        cam = Camera.main;
        view = GetComponent<PhotonView>();
        popUp = GetComponent<PopUp>();
        cameraShake = GameManager.instance.GetCameraShake();
        m_health = maxHealth;

        Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, 0));

        minX = bottomLeft.x + clampPadding;
        maxX = topRight.x - clampPadding;
        minY = bottomLeft.y + clampPadding;
        maxY = topRight.y - clampPadding;

        // Assign color using synced custom properties or fallback
        if (view.IsMine)
        {
            if (view.Owner.CustomProperties.TryGetValue("Color", out object colorValue))
            {
                if (ColorUtility.TryParseHtmlString("#" + colorValue.ToString(), out Color parsedColor))
                {
                    view.RPC("SetColor", RpcTarget.AllBuffered, parsedColor.r, parsedColor.g, parsedColor.b);
                }
            }
            else
            {
                int index = Mathf.Min(view.Owner.ActorNumber - 1, PlayerColors.Length - 1);
                Color fallbackColor = PlayerColors[index];
                view.RPC("SetColor", RpcTarget.AllBuffered, fallbackColor.r, fallbackColor.g, fallbackColor.b);
            }
        }
    }

    private void Update()
    {
        Move();
        UserInput();
    }

    private void UserInput()
    {
        if (view.IsMine && GameManager.instance.GameState == GameState.Playing)
        {
            moveDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            if (Input.GetMouseButtonDown(0))
                Shoot();
        }
    }

    private void Move()
    {
        Vector3 newPosition = transform.position + (Vector3)moveDirection * speed * Time.deltaTime;

        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

        transform.position = newPosition;
        transform.rotation = Quaternion.Euler(0, 0, -moveDirection.x * rotateAngle);
    }

    private void Shoot()
    {
        float spread = Random.Range(-bulletSpread, bulletSpread);
        cameraShake.TriggerShake(0.15f, 0.15f);
        view.RPC("RPC_Shoot", RpcTarget.All, GetBulletAngle(), spread);
    }

    [PunRPC]
    private void RPC_Shoot(float bulletAngle, float spread)
    {
        GameObject bullet = ObjectPool.instance.GetBullets();
        if (!bullet) return;

        bullet.GetComponent<Bullet>().bulletParent = BulletParent.Player;
        bullet.transform.position = transform.position;
        bullet.SetActive(true);
        bullet.GetComponent<Bullet>().Init(bulletSpeed, spread, bulletAngle);

        popUp.Popup();
        AudioManager.instance.PlaySoundEffect("Shooting");
    }

    public void TakeDamage(float value)
    {
        Health -= value;

        if (view.IsMine)
            UIManager.instance.UpdateHealthBar(Health);

        popUp.Popup();

        if (Health <= 0) Die();
        else AudioManager.instance.PlaySoundEffect("Hit");
    }

    private void Die()
    {
        if (view.IsMine)
        {
            AudioManager.instance.PlaySoundEffect("GameOver");
        }

        view.RPC("RPC_Die", RpcTarget.All);
    }

    [PunRPC]
    public void RPC_Die()
    {
        AudioManager.instance.PlaySoundEffect("Die");
        GameManager.instance.GameOver();
        UIManager.instance.GameOver();
    }

    private float GetBulletAngle()
    {
        Vector3 dir = GetBulletDirection();
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }

    private Vector3 GetBulletDirection()
    {
        Vector3 dir = cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        dir.z = 0;
        dir.Normalize();
        return dir;
    }

    [PunRPC]
    public void SetColor(float r, float g, float b)
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            Color color = new Color(r, g, b);
            spriteRenderer.color = color;
        }
        else
        {
            Debug.LogWarning("SpriteRenderer is still null when SetColor was called.");
        }
    }
}

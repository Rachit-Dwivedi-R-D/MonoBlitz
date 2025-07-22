using UnityEngine;
using UnityEngine.EventSystems;

public class SinglePlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    private Vector2 moveDirection;
private float minX, maxX, minY, maxY;
private float clampPadding = 0.5f;

    [SerializeField, Space(10)] private float bulletSpeed;
    [SerializeField, Range(0f, 10f)] private float bulletSpread;
    [SerializeField, Range(0f, 10f)] private float rotateAngle;
    [SerializeField] public float maxHealth;

    [HideInInspector] public PopUp popUp;

    private float m_health;
    private Camera cam;
    private CameraShake cameraShake;

    public float Health
    {
        get => m_health;
        set => m_health = value;
    }

    private void Start()
{
    CursorManager.instance?.SetCrosshairCursor();
    cam = Camera.main;
    popUp = GetComponent<PopUp>();
    cameraShake = GameManager_Single.instance.GetCameraShake();

    m_health = maxHealth;

    // Calculate world bounds from camera
    Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, 0));
    Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, 0));

    minX = bottomLeft.x + clampPadding;
    maxX = topRight.x - clampPadding;
    minY = bottomLeft.y + clampPadding;
    maxY = topRight.y - clampPadding;
}

    private void Update()
    {
        if (GameManager_Single.instance.GameState != GameState.Playing)
            return;

        HandleInput();
        Move();
    }

    private void HandleInput()
{
    moveDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

    // Prevent shooting when clicking on UI
    if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
    {
        Shoot();
    }
}

    private void Move()
{
    Vector3 newPosition = transform.position + (Vector3)moveDirection * speed * Time.deltaTime;

    // Clamp within screen
    newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
    newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

    transform.position = newPosition;

    if (moveDirection != Vector2.zero)
    {
        transform.rotation = Quaternion.Euler(0, 0, -moveDirection.x * rotateAngle);
    }
}

    private void Shoot()
    {
        float spread = Random.Range(-bulletSpread, bulletSpread);
        cameraShake.TriggerShake(0.15f, 0.15f);

        GameObject bullet = ObjectPool_Single.instance.GetBullets();
        if (!bullet) return;

        bullet.GetComponent<Bullet>().bulletParent = BulletParent.Player;
        bullet.transform.position = transform.position;
        bullet.SetActive(true);

        bullet.GetComponent<Bullet>().Init(bulletSpeed, spread, GetBulletAngle());
        popUp.Popup();
        AudioManager.instance.PlaySoundEffect("Shooting");
    }

    public void TakeDamage(float value)
    {
        Health -= value;
        UIManager_Single.instance.UpdateHealthBar(Health);

        popUp.Popup();

        if (Health <= 0) Die();
        else AudioManager.instance.PlaySoundEffect("Hit");
    }

    private void Die()
    {
        AudioManager.instance.PlaySoundEffect("Die");
        GameManager_Single.instance.GameOver();
        UIManager_Single.instance.GameOver();
    }

    private float GetBulletAngle()
    {
        Vector3 lookPos = GetBulletDirection();
        return Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;
    }

    private Vector3 GetBulletDirection()
    {
        Vector3 dir = cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        dir.Normalize();
        return dir;
    }
}

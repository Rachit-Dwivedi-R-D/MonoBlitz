using UnityEngine;

public enum BulletParent
{
    Player,
    Enemy
}

public class Bullet : MonoBehaviour
{
    private float bulletSpeed;

    [HideInInspector]
    public BulletParent bulletParent;

    void Update()
    {
        transform.position += transform.right * bulletSpeed * Time.deltaTime;
    }

    public void Init(float _bulletSpeed, float _bulletSpread, float angle)
    {
        float desiredBulletSpread = Random.Range(-_bulletSpread, _bulletSpread);
        transform.rotation = Quaternion.Euler(0, 0, angle + desiredBulletSpread);

        bulletSpeed = _bulletSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("End"))
        {
            gameObject.SetActive(false);
        }
        else if (collision.CompareTag("Enemy") && bulletParent == BulletParent.Player)
        {
            // Try single-player enemy
            var enemySingle = collision.GetComponent<EnemyController_Single>();
            if (enemySingle != null)
            {
                enemySingle.TakeDamage(20);
            }
            else
            {
                // Fall back to multiplayer enemy
                var enemyMulti = collision.GetComponent<EnemyController>();
                if (enemyMulti != null)
                    enemyMulti.TakeDamage(20);
            }

            gameObject.SetActive(false);
        }
        else if (collision.CompareTag("Player") && bulletParent == BulletParent.Enemy)
        {
            // Try single-player player
            var playerSingle = collision.GetComponent<SinglePlayerController>();
            if (playerSingle != null)
            {
                playerSingle.TakeDamage(10);
            }
            else
            {
                // Fall back to multiplayer player
                var playerMulti = collision.GetComponent<PlayerController>();
                if (playerMulti != null)
                    playerMulti.TakeDamage(10);
            }

            gameObject.SetActive(false);
        }
    }
}

using UnityEngine;

public class FollowEnemy_Single : EnemyController_Single
{
    private void Update()
    {
        if (GameManager_Single.instance.GameState == GameState.GameOver)
            return;

        if (target == null)
        {
            target = GetClosestTarget()?.gameObject;

            if (target == null) return;
        }

        Follow();
    }

    public override void Follow()
    {
        transform.position += (Vector3)GetDirection(target.transform) * moveSpeed * Time.deltaTime;

    }

    public override void Attack()
    {
        // No attack logic for follow enemy in single player
    }

    private void ResetColor()
    {
        spriteRenderer.color = new Color(1f, 0.25f, 0.25f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var player = collision.gameObject.GetComponent<SinglePlayerController>();
            if (player != null)
            {
                player.TakeDamage(50);
            }

            Die();
        }
    }
}

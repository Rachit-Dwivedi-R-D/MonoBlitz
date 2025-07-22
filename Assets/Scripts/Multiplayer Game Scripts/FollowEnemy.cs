using UnityEngine;

public class FollowEnemy : EnemyController
{
    private void Update()
    {
        if (GameManager.instance.GameState == GameState.GameOver)
            return;

        target = GetClosestTarget();

        Follow();
    }

    public override void Follow()
    {
        transform.position += (Vector3)GetDirection(target) * moveSpeed * Time.deltaTime;
    }

    public override void Attack()
    {

    }

    private void ResetColor()
    {
        spriteRenderer.color = new Color(1f, 0.25f, 0.25f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(50);
            Die();
        }
    }
}

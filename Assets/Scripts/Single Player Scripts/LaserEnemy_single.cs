using System.Collections.Generic;
using UnityEngine;

public class LaserEnemy_Single : EnemyController_Single
{
    [SerializeField] private float attackDistance;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float rateOfFire;

    [SerializeField, Space(10)] private LineRenderer laserPoint;
    [SerializeField] private EdgeCollider2D laserCollider;

    public List<Vector2> laserPoints = new List<Vector2>();

    [SerializeField, Space(10)] private float maxDamageTime;
    private float damageTimer;

    private bool laserFired;

    private void Update()
{
    if (GameManager_Single.instance == null || GameManager_Single.instance.GameState == GameState.GameOver)
        return;

    target = GetClosestTarget()?.gameObject;

    if (target == null) return;

    if (GetDistance(target.transform) > attackDistance)
        Follow();
    else if (!laserFired)
        Attack();

    transform.rotation = Quaternion.Lerp(
        transform.rotation,
        Quaternion.Euler(0, 0, GetBulletAngle() - 90),
        rotateSpeed * Time.deltaTime
    );
}


    public override void Follow()
    {
        transform.position += (Vector3)GetDirection(target.transform) * moveSpeed * Time.deltaTime;  // ✅ FIXED
    }

    public override void Attack()
    {
        PerformAttack();
    }

    private void PerformAttack()
    {
        laserFired = true;

        laserPoints[0] = laserPoint.GetPosition(0);
        SetLaserPosAndCollider(new Vector3(0, 40, 1));

        popUp.Popup();
        AudioManager.instance.PlaySoundEffect("Shooting");

        Invoke("ResetLaser", rateOfFire / 2f);
        Invoke("ResetLaserShot", rateOfFire);
    }

    private void ResetLaser()
    {
        SetLaserPosAndCollider(Vector3.zero);
        AudioManager.instance.PlaySoundEffect("Shooting");
    }

    private void ResetLaserShot()
    {
        laserFired = false;
    }

    private void SetLaserPosAndCollider(Vector3 pos)
    {
        laserPoint.SetPosition(1, new Vector3(pos.x, pos.y, 1));
        laserPoints[1] = laserPoint.GetPosition(1);
        laserCollider.SetPoints(laserPoints);
    }

    private float GetBulletAngle()
    {
        Vector3 lookPos = GetDirection(target.transform);  // ✅ FIXED
        return Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && GameManager_Single.instance.GameState == GameState.Playing)
        {
            if (damageTimer >= maxDamageTime)
            {
                var player = collision.GetComponent<SinglePlayerController>();
                if (player != null)
                    player.TakeDamage(10);

                damageTimer = 0;
            }

            damageTimer += Time.deltaTime;
        }
    }
}

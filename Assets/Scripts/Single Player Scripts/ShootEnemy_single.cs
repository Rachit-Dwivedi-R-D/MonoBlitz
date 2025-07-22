using UnityEngine;

public class ShootEnemy_Single : EnemyController_Single
{
    [SerializeField] private float attackDistance;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float rateOfFire;

    [SerializeField, Range(0f, 5f)]
    private float bulletSpread;

    [Space(10)]
    [SerializeField] private GameObject shootPoint;

    private bool shotFired;

    private void Update()
    {
        if (GameManager_Single.instance.GameState == GameState.GameOver)
            return;

        // Assign closest player target (transform)
        var closest = GetClosestTarget();
        if (closest == null) return;

        target = closest.gameObject;

        if (GetDistance(closest) > attackDistance)
            Follow();
        else if (!shotFired)
            Attack();

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.Euler(0, 0, GetBulletAngle(closest) - 90),
            rotateSpeed * Time.deltaTime
        );
    }

    public override void Follow()
    {
        if (target == null) return;
        transform.position += (Vector3)GetDirection(target.transform) * moveSpeed * Time.deltaTime;
    }

    public override void Attack()
    {
        GameObject tempBullet = ObjectPool_Single.instance.GetBullets();
        if (!tempBullet) return;

        tempBullet.GetComponent<Bullet>().bulletParent = BulletParent.Enemy;
        popUp.Popup();
        AudioManager.instance.PlaySoundEffect("Shooting");

        shotFired = true;

        float desiredBulletSpread = Random.Range(-bulletSpread, bulletSpread);
        tempBullet.transform.position = shootPoint.transform.position;

        tempBullet.SetActive(true);
        tempBullet.GetComponent<Bullet>().Init(bulletSpeed, desiredBulletSpread, GetBulletAngle(target.transform));

        Invoke("ResetShot", rateOfFire);
    }

    private void ResetShot()
    {
        shotFired = false;
    }

    private float GetBulletAngle(Transform targetTransform)
    {
        Vector3 lookPos = GetDirection(targetTransform);
        return Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;
    }
}

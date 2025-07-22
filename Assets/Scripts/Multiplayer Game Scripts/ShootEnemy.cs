using UnityEngine;
using Photon.Pun;

public class ShootEnemy : EnemyController
{
    [SerializeField]
    private float attackDistance;

    [SerializeField]
    private float rotateSpeed;

    [SerializeField]
    private float bulletSpeed;

    [SerializeField]
    private float rateOfFire;

    [SerializeField]
    [Range(0f, 5f)]
    private float bulletSpread;

    [Space(10)]
    [SerializeField]
    private GameObject shootPoint;

    private bool shotFired;

    private void Update()
    {
        if (GameManager.instance.GameState == GameState.GameOver)
            return;

        target = GetClosestTarget();

        if (GetDistance(target) > attackDistance)
            Follow();
        else if (!shotFired)
            Attack();


        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, GetBulletAngle() - 90), rotateSpeed * Time.deltaTime);

    }

    public override void Follow()
    {
        transform.position += (Vector3)GetDirection(target) * moveSpeed * Time.deltaTime;
    }


    public override void Attack()
    {
        if(PhotonNetwork.IsMasterClient)
            view.RPC("RPC_Attack", RpcTarget.All);
    }

    [PunRPC]
    public void RPC_Attack()
    {
        GameObject tempBullet = ObjectPool.instance.GetBullets();

        if (!tempBullet) return;

        tempBullet.GetComponent<Bullet>().bulletParent = BulletParent.Enemy;

        popUp.Popup();

        AudioManager.instance.PlaySoundEffect("Shooting");

        shotFired = true;

        float desiredBulletSpread = Random.Range(-bulletSpread, bulletSpread);

        tempBullet.transform.position = shootPoint.transform.position;

        tempBullet.SetActive(true);

        tempBullet.GetComponent<Bullet>().Init(bulletSpeed, desiredBulletSpread, GetBulletAngle());

        Invoke("ResetShot", rateOfFire);
    }

    private void ResetShot()
    {
        shotFired = false;
    }

    private void ResetColor()
    {
        spriteRenderer.color = new Color(1f, 0.25f, 0.25f);
    }

    private float GetBulletAngle()
    {
        Vector3 lookPos = GetDirection(target);

        float angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;

        return angle;
    }
}

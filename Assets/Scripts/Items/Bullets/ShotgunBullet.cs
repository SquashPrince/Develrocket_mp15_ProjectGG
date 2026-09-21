using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ShotgunBullet : GunBullet
{
    [SerializeField] private int _gauge;
    [SerializeField] private GunBullet bullet;
    [SerializeField] private float _fireRadius;

    private GunBullet[] _bullets;

    private void Init()
    {
        _bullets = new GunBullet[_gauge];

        for (int i = 0; i < _bullets.Length; i++)
        {
            GunBullet b = Instantiate(bullet, transform);
            b.SetData(_damage / _gauge, _bulletSpeed, _range);
            _bullets[i] = b;
        }

        gameObject.SetActive(false);
    }

    public void SetData(int damage, float range, float bulletSpeed)
    {
        _damage = damage;
        _range = range;
        _bulletSpeed = bulletSpeed;

        Init();
    }

    private void ShotToRadious()
    {
        for (int i = 0; i < _bullets.Length; i++)
        {
            Vector3 randomDirection = Random.insideUnitCircle * _fireRadius;
            Vector3 targetPosition = transform.position
                + transform.forward * _range
                + transform.right * randomDirection.x
                + transform.up * randomDirection.y;

            _bullets[i].transform.LookAt(targetPosition, transform.forward);
            _bullets[i].gameObject.SetActive(true);
            _bullets[i].transform.SetParent(null);
        }

        gameObject.SetActive(false);
    }

    public override void OnBulletFire()
    {
        gameObject.SetActive(true);
        ShotToRadious();
        gameObject.SetActive(false);
    }

    public override void OnBulletDest()
    {
        gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Handles.DrawWireArc(transform.position, transform.forward, Vector3.up, 360, _fireRadius);
    }
#endif
}

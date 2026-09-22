using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ShotgunBullet : GunBullet
{
    [SerializeField] private int _gauge;
    [SerializeField] private GunBullet bullet;
    [SerializeField] private float _fireRadius;

    [SerializeField] protected ObjectPool<GunBullet> _gunbullet;
    private GunBullet[] _bullets;

    private void InitGauge()
    {
        _gunbullet = new ObjectPool<GunBullet>(
            bullet,
            _gauge,
            transform,
            _bullet => _bullet.SetData(_damage / _gauge, _range, _bulletSpeed)
            );

        //_bullets = new GunBullet[_gauge];
        //for (int i = 0; i < _bullets.Length; i++)
        //{
        //    GunBullet b = Instantiate(bullet, transform);
        //    b.SetData(_damage / _gauge, _bulletSpeed, _range);
        //    _bullets[i] = b;
        //}

        ReturnToPool();
    }

    public override void SetData(int damage, float range, float bulletSpeed)
    {
        base.SetData(damage, range, bulletSpeed);

        InitGauge();
    }

    private void ShotToRadious()
    {
        for (int i = 0; i < _gauge; i++)
        {
            Vector3 randomDirection = Random.insideUnitCircle * _fireRadius;
            Vector3 targetPosition = transform.position
                + transform.forward * _range
                + transform.right * randomDirection.x
                + transform.up * randomDirection.y;

            _gunbullet.ObjectList[i].transform.LookAt(targetPosition, transform.forward);
        }

        _gunbullet.PopAll();

        ReturnToPool();
    }

    public override void OnBulletFire()
    {
        gameObject.SetActive(true);
        ShotToRadious();
        gameObject.SetActive(false);
    }


#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Handles.DrawWireArc(transform.position, transform.forward, Vector3.up, 360, _fireRadius);
    }
#endif
}

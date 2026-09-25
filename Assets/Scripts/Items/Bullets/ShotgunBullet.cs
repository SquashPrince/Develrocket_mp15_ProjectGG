using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class ShotgunBullet : GunBullet
{
    [SerializeField] private int _gauge;
    [SerializeField] private GunBullet bullet;
    [Range(30f, 180f)] [SerializeField] private float _fireAngle;

    [SerializeField] protected ObjectPool<GunBullet> _gunbullet;
    private GunBullet[] _bullets;

    private void InitGauge()
    {
        _gunbullet = new ObjectPool<GunBullet>(bullet, _gauge, transform, pellet =>
            {
                pellet.SetData(_damage / _gauge, _range, _bulletSpeed);
                pellet.SetDamageSource(() => _damage / _gauge);
            }
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

    // 원형 랜덤 발사 ※미사용
    /*
    private float _fireRadius;
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
    */

    private void ShotToSector()
    {
        for (int i = 0; i < _gauge; i++)
        {
            Vector3 directiion = GetShotDirection(i);
            Vector3 targetPosition = transform.position + directiion * _range;

            _gunbullet.ObjectList[i].transform.LookAt(targetPosition, transform.forward);
        }

        _gunbullet.PopAll();
        ReturnToPool();
    }
    private Vector3 GetShotDirection(int index)
    {
        float centerIndex = (_gauge - 1) * 0.5f;
        float angleSpace = _fireAngle / _gauge;
        float angle = (index - centerIndex) * angleSpace;

        return Quaternion.AngleAxis(angle, transform.up) * transform.forward;
    }

    public override void OnBulletFire()
    {
        RefreshDamage();
        gameObject.SetActive(true);
        //ShotToRadious();
        ShotToSector();
        gameObject.SetActive(false);
    }


#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (_gauge <= 0)
            return;

        Vector3 origin = transform.position;
        float shotAngle = _fireAngle / _gauge;

        float halfAngle = (_gauge - 1) * shotAngle * 0.5f;

        Vector3 leftEnd =
            origin + Quaternion.AngleAxis(-halfAngle, transform.up) * transform.forward * _range;

        Vector3 rightEnd =
            origin + Quaternion.AngleAxis(halfAngle, transform.up) * transform.forward * _range;

        // 부채꼴 양쪽 변
        Gizmos.DrawLine(origin, leftEnd);
        Gizmos.DrawLine(origin, rightEnd);

        // 부채꼴 바깥쪽 호
        const int segments = 32;
        Vector3 previousPoint = leftEnd;

        for (int i = 1; i <= segments; i++)
        {
            float t = i / (float)segments;
            float angle = Mathf.Lerp(-halfAngle, halfAngle, t);

            Vector3 point =
                origin + Quaternion.AngleAxis(angle, transform.up) * transform.forward * _range;

            Gizmos.DrawLine(previousPoint, point);
            previousPoint = point;
        }

        // 실제 탄환이 발사되는 방향
        for (int i = 0; i < _gauge; i++)
        {
            Vector3 end =
                origin + GetShotDirection(i) * _range;

            Gizmos.DrawLine(origin, end);
            Gizmos.DrawSphere(end, 0.05f);
        }
    }
#endif
}

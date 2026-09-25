using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFanAttack : BossPattern
{
    // --------------- 오브젝트 풀
    //[SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private GunBullet _bulletPrefab;
    [SerializeField] private LayerMask _targetLayer;

    private ObjectPool<GunBullet> _bulletPool;
    // --------------- 오브젝트 풀

    [SerializeField] private int _bulletCount = 10;
    private float _startAngle = 0f;

    [SerializeField] private Transform _targetTransform;


    // --------------- 오브젝트 풀
    private void Start()
    {
        base.Start();
        _bulletPool = new ObjectPool<GunBullet>(
            _bulletPrefab,
            10,
            gameObject.transform,
            bullet =>
            {
                bullet.SetData(
                    _damage,     // 데미지
                    20f,    // 사거리
                    10f     // 총알 속도
                );
            }
        );
    }


// --------------- 오브젝트 풀

    protected override IEnumerator PatternRoutine()
    {
        if (_targetTransform == null)
        {
            _targetTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        Vector3 targetDirection =
        _targetTransform.position - transform.position;

        targetDirection.y = 0f;
        targetDirection.Normalize();

        float angleStep =
            (_bulletCount > 1) ? 120f / (_bulletCount - 1) : 0f;

        float startAngle = -60f;

        for (int i = 0; i < _bulletCount; i++)
        {
            float currentAngle =
                (_bulletCount > 1)
                    ? startAngle + (angleStep * i)
                    : 0f;

            Vector3 finalDirection =
                Quaternion.Euler(0f, currentAngle, 0f) * targetDirection;

            FireBullet(finalDirection);
        }
        yield return null;
    }

    private void FireBullet(Vector3 direction)
    {
        // --------------- 오브젝트 풀

        /*Bullet bullet = Instantiate(
            _bulletPrefab,
            transform.position,
            Quaternion.identity
        );

        bullet.SetDirection(direction);*/
        GunBullet bullet = _bulletPool.Pop();

        bullet.transform.position = transform.position;
        bullet.transform.forward = direction;
        // --------------- 오브젝트 풀
    }
}

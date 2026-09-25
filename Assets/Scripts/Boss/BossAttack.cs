using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : BossPattern
{
    // --------------- 오브젝트 풀
    //[SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private GunBullet _bulletPrefab;
    [SerializeField] private LayerMask _targetLayer;

    private ObjectPool<GunBullet> _bulletPool;

    // --------------- 오브젝트 풀

    [SerializeField] private int _bulletCount = 30;
    private float _startAngle = 0f;

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
        _animator.SetTrigger("StartFanAttack");
        
        Debug.Log("실행");
        float angleStep = 360f / _bulletCount;

        for (int i = 0; i < _bulletCount; i++)
        {
            float angle = _startAngle + angleStep * i;

            float x = Mathf.Cos(angle * Mathf.Deg2Rad);
            float z = Mathf.Sin(angle * Mathf.Deg2Rad);

            Vector3 direction = new Vector3(x, 0f, z);

            FireBullet(direction);
        }

        _startAngle += 30f;

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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAimedAttack : BossPattern
{
    // --------------- 오브젝트 풀
    //[SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private GunBullet _bulletPrefab;
    [SerializeField] private LayerMask _targetLayer;

    private ObjectPool<GunBullet> _bulletPool;

    // --------------- 오브젝트 풀

    [SerializeField] private int _shotCount = 4;
    [SerializeField] private float _shotDelay = 0.2f;
    
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
        _animator.SetTrigger("StartAimedAttack");
        Debug.Log(Time.time);
        for (int i = 0; i < _shotCount; i++)
        {
            Vector3 direction =
                _player.position - transform.position;

            direction.y = 0f;

            direction.Normalize();

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

            yield return new WaitForSeconds(_shotDelay);
        }
    }
}

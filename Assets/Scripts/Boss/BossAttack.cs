using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    [SerializeField] private Bullet _bulletPrefab;

    [SerializeField] private int _bulletCount = 30;
    private float _startAngle = 0f;

    // BossControl
    [SerializeField] private BossControl _bossControl;

    // 이 패턴의 쿨타임
    [SerializeField] private float _cooldown = 3f;

    private void Start()
    {
        StartCoroutine(Cooldown());
    }


    private IEnumerator Cooldown()
    {
        while (true)
        {
            // 쿨타임 기다리기
            yield return new WaitForSeconds(_cooldown);

            // 쿨타임이 끝나면 Queue에 패턴 추가
            _bossControl.AddPattern(RoundFire());
        }
    }


    public IEnumerator RoundFire()
    {
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
        Bullet bullet = Instantiate(
            _bulletPrefab,
            transform.position,
            Quaternion.identity
        );

        bullet.SetDirection(direction);
    }

    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Fire();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine(RoundFire());
        }
    }

    private void Fire()
    {
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
    }

    public IEnumerator RoundFire()
    {
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
        Bullet bullet = Instantiate(
            _bulletPrefab,
            transform.position,
            Quaternion.identity
        );

        bullet.SetDirection(direction);
    }*/
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : BossPattern
{
    [SerializeField] private Bullet _bulletPrefab;

    [SerializeField] private int _bulletCount = 30;
    private float _startAngle = 0f;


    protected override IEnumerator PatternRoutine()
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

}

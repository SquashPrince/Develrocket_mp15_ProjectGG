using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    [SerializeField] private Bullet _bulletPrefab;

    [SerializeField] private int _bulletCount = 30;
    private float _startAngle = 0f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Fire();
        }
    }

    private void Fire()
    {
        float angleStep = 360f / _bulletCount;

        for (int i = 0; i < _bulletCount; i++)
        {
            float angle = _startAngle + angleStep * i;

            float x = Mathf.Cos(angle * Mathf.Deg2Rad);
            float y = Mathf.Sin(angle * Mathf.Deg2Rad);

            Vector2 direction = new Vector2(x, y);

            FireBullet(direction);
        }

        _startAngle += 30f;
    }

    private void FireBullet(Vector2 direction)
    {
        Bullet bullet = Instantiate(
            _bulletPrefab,
            transform.position,
            Quaternion.identity
        );

        bullet.SetDirection(direction);
    }
}

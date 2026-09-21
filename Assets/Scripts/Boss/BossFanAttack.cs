using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFanAttack : MonoBehaviour
{
    [SerializeField] private Bullet _bulletPrefab;

    [SerializeField] private int _bulletCount = 30;
    private float _startAngle = 0f;

    [SerializeField] private Transform _targetTransform;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Fire();
        }
    }

    private void Fire()
    {
        Vector2 targetDirection = (_targetTransform.position - transform.position).normalized;

        float angleStep = (_bulletCount > 1) ? 120f / (_bulletCount - 1) : 0f;
        float startAngle = -60f;

        for (int i = 0; i < _bulletCount; i++)
        {
            float currentAngle = (_bulletCount > 1) ? startAngle + (angleStep * i) : 0f;

            Vector2 finalDirection = Quaternion.Euler(0, 0, currentAngle) * targetDirection;

            FireBullet(finalDirection);
        }
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

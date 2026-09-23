using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFanAttack : MonoBehaviour
{
    [SerializeField] private Bullet _bulletPrefab;

    [SerializeField] private int _bulletCount = 10;
    private float _startAngle = 0f;

    [SerializeField] private Transform _targetTransform;

    [SerializeField] private BossControl _bossControl;
    [SerializeField] private float _cooldown = 5f;

    private void Start()
    {
        StartCoroutine(Cooldown());
    }

    private IEnumerator Cooldown()
    {
        while (true)
        {
            yield return new WaitForSeconds(_cooldown);

            _bossControl.AddPattern(FanFire());
        }
    }

    public IEnumerator FanFire()
    {
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Fire();
        }
    }

    private void Fire()
    {
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

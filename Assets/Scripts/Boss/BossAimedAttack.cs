using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAimedAttack : BossPattern
{
    [SerializeField] private Bullet _bulletPrefab;

    [SerializeField] private int _shotCount = 4;
    [SerializeField] private float _shotDelay = 0.2f;

    private void OnEnable()
    {
        Debug.Log(_bossControl == null);
    }

    protected override IEnumerator PatternRoutine()
    {
        for (int i = 0; i < _shotCount; i++)
        {
            Vector3 direction =
                _player.position - transform.position;

            direction.y = 0f;

            direction.Normalize();

            Bullet bullet = Instantiate(
                _bulletPrefab,
                transform.position,
                Quaternion.identity
            );

            bullet.SetDirection(direction);

            yield return new WaitForSeconds(_shotDelay);
        }
    }
}

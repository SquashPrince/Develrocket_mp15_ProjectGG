using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAimedAttack : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private Bullet _bulletPrefab;

    [SerializeField] private int _shotCount = 4;
    [SerializeField] private float _shotDelay = 0.2f;

    private void Update()
    {
        // 테스트용
        if (Input.GetKeyDown(KeyCode.V))
        {
            StartCoroutine(AimedBurst());
        }
    }

    public IEnumerator AimedBurst()
    {
        for (int i = 0; i < _shotCount; i++)
        {
            Vector2 direction =
                (_player.position - transform.position).normalized;

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

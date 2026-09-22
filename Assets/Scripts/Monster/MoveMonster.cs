using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMonster : MonoBehaviour
{
    [SerializeField] private Monster _monster;
    [SerializeField] private Transform _player;
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private float _shotDelay = 0.2f;

    // 데미지를 입었냐
    [SerializeField] private bool isDamaged = false;
    private SphereCollider _sphereColider;
    private bool _isPlayerInSight = false;
    private bool _isShooting = false;


    private void Awake()
    {
        CacheComponents();

        if (_player == null)
        {
            _player = GameObject.FindGameObjectWithTag("Player").transform;
        }

    }

    private void Update()
    {
        if (isDamaged || _isPlayerInSight)
        {
            MoveToPlayer();
            if (!_isShooting)
            {
                StartCoroutine(AimedBurst());
            }
        }
    }

    private void MoveToPlayer()
    {
        Vector3 direction =
        _player.position - transform.position;

        direction.y = 0f;
        direction.Normalize();

        transform.position +=
            direction * _monster.MoveSpeed * Time.deltaTime;
    }

    public IEnumerator AimedBurst()
    {
        _isShooting = true;
        while (_isPlayerInSight == true)
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
        _isShooting = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInSight = true;
        }
    }


    private void CacheComponents()
    {
        _sphereColider = GetComponent<SphereCollider>();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretMonster : MonoBehaviour
{
    [SerializeField] private Monster _monster;
    [SerializeField] private Transform _player;
    [SerializeField] private Bullet _bulletPrefab;

    [SerializeField] private float _shotDelay = 0.2f;
    private float _currentCooldown;

    private bool _isPlayerInTrigger => _player != null;
    private bool _isPlayerInSight = false;
    private bool _isReadyToFire { get { return _currentCooldown >= _shotDelay; } }
    private SphereCollider _sphereColider;
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
        if (_isPlayerInSight == true && !_isShooting)
        {
            StartCoroutine(AimedBurst());
        }

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
            Debug.Log("Player is in sight");
            // 무적 해제 
            _monster.Invincibility = false;
            Debug.Log($"{_monster.Invincibility} : 무적 해제");
            // 공격 

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInSight = false;
            Debug.Log("Player is out sight");
            // 무적 
            _monster.Invincibility = true;
            Debug.Log($"{_monster.Invincibility} : 무적 작동");
            // 공격 x 
        }
    }



    private void CacheComponents()
    {
        _sphereColider = GetComponent<SphereCollider>();
    }
}

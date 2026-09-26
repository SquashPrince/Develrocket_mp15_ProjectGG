using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretMonster : MonoBehaviour
{
    [SerializeField] private Monster _monster;
    private Transform _player;
    // --------------- 오브젝트 풀
    //[SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private GunBullet _bulletPrefab;
    [SerializeField] private LayerMask _targetLayer;

    private ObjectPool<GunBullet> _bulletPool;
    // --------------- 오브젝트 풀

    [SerializeField] private float _shotDelay = 0.2f;
    private float _currentCooldown;

    private bool _isPlayerInTrigger => _player != null;
    private bool _isPlayerInSight = false;
    private bool _isReadyToFire { get { return _currentCooldown >= _shotDelay; } }
    private SphereCollider _sphereColider;
    private bool _isShooting = false;

    private void Awake()
    {
        
    }

    // --------------- 오브젝트 풀
    private void Start()
    {
        CacheComponents();
        StartCoroutine(InitRoutine());
        _bulletPool = new ObjectPool<GunBullet>(
            _bulletPrefab,
            10,
            gameObject.transform,
            bullet =>
            {
                bullet.SetData(
                    1,     // 데미지
                    20f,    // 사거리
                    10f     // 총알 속도
                );
            }
        );
    }

    public IEnumerator InitRoutine()
    {
        yield return new WaitUntil(()=> GameManager.Instance.PlayerTransform != null);
        _player = GameManager.Instance.PlayerTransform;
    }

// --------------- 오브젝트 풀


    private void Update()
    {
        if (_isPlayerInSight == true && !_isShooting && _player != null)
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
        _isShooting = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
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
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _isPlayerInSight = false;
            Debug.Log("Player is out sight");
            // 무적 
            _monster.Invincibility = true;
            Debug.Log($"{_monster.Invincibility} : 무적 작동");
            // 공격 x 
        }
    }

    public void Dead()
    {
        for (int i = 0; i < 4; i++)
        {
            float angle = 90 * i;

            float x = Mathf.Cos(angle * Mathf.Deg2Rad);
            float z = Mathf.Sin(angle * Mathf.Deg2Rad);

            Vector3 direction = new Vector3(x, 0f, z);

            FireBullet(direction);
        }
        
    }
    
    private void FireBullet(Vector3 direction)
    {
        GunBullet bullet = _bulletPool.Pop();

        bullet.transform.position = transform.position;
        bullet.transform.forward = direction;
    }

    private void CacheComponents()
    {
        _sphereColider = GetComponent<SphereCollider>();
    }
}

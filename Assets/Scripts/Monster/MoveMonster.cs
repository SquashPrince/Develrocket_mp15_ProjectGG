using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMonster : MonoBehaviour
{
    [SerializeField] private Monster _monster;
    [SerializeField] private Transform _player;
    // --------------- 오브젝트 풀
    //[SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private GunBullet _bulletPrefab;
    [SerializeField] private LayerMask _targetLayer;

    private ObjectPool<GunBullet> _bulletPool;

    // --------------- 오브젝트 풀
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
    
    // --------------- 오브젝트 풀
    private void Start()
    {
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


// --------------- 오브젝트 풀

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
        transform.LookAt(_player.position);
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

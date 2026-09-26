using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMonster : MonoBehaviour
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

    // 데미지를 입었냐
    [SerializeField] private bool isDamaged = false;
    private SphereCollider _sphereColider;
    private bool _isPlayerInSight = false;
    private bool _isShooting = false;
    
    private Animator _animator;

    public bool IsplayerInsight
    {
        get{ return _isPlayerInSight; }
        set
        {
            _isPlayerInSight = value;
        }
    }


    private void Awake()
    {
        CacheComponents();
    }
    
    // --------------- 오브젝트 풀
    private void Start()
    {
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
        if ((isDamaged || _isPlayerInSight) && _player !=null)
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
        if (_animator == null)
        {
            _animator = GetComponentInChildren<Animator>();
        }
        _animator.SetBool("isWalk", true);
        transform.LookAt(_player.position);
        Vector3 direction =
        _player.position - transform.position;

        direction.y = 0f;
        direction.Normalize();

        float distance = Vector3.Distance(transform.transform.position, _player.transform.position);

        if (distance > 5)
        {
            transform.position +=
                direction * _monster.MoveSpeed * Time.deltaTime;    
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
        }
    }


    private void CacheComponents()
    {
        _sphereColider = GetComponent<SphereCollider>();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombMonster : MonoBehaviour
{
    private Transform _player;
    
    [SerializeField] private GameObject _chargeWarning;

    [SerializeField] private float _warningTime = 0.5f;
    [SerializeField] private float _chargeSpeed = 10f;
    [SerializeField] private float _bombDistance = 5f;
    private Animator _animator;

    //[SerializeField] private Transform _target;
    private bool isBomb = false;
    
    private bool _isPlayerInSight = false;

    public bool IsplayerInsight
    {
        get{ return _isPlayerInSight; }
        set
        {
            _isPlayerInSight = value;
        }
    }
    
    public void Start()
    {
        StartCoroutine(InitRoutine());
    }

    private void Update()
    {
        if (_isPlayerInSight == true)
        {
            _isPlayerInSight = false;
            if (isBomb == false && _player != null)
            {
                isBomb = true;
                //_target.position = other.gameObject.transform.position;
                StartCoroutine(Charge());
            }
        }
    }

    public IEnumerator InitRoutine()
    {
        yield return new WaitUntil(()=> GameManager.Instance.PlayerTransform != null);
        _player = GameManager.Instance.PlayerTransform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && isBomb == false && _player != null)
        {
            isBomb = true;
            //_target.position = other.gameObject.transform.position;
            StartCoroutine(Charge());
        }
    }


    public IEnumerator Charge()
    {
        if (_animator == null)
        {
            _animator = GetComponentInChildren<Animator>();
        }
        _animator.SetBool("isRun", true);
        transform.LookAt(_player.position);

        Vector3 direction =
            _player.position - transform.position;

        direction.y = 0f;

        float distance = direction.magnitude;
        direction.Normalize();

        SetWarning(direction, distance);

        _chargeWarning.SetActive(true);

        yield return new WaitForSeconds(_warningTime);

        _chargeWarning.SetActive(false);

        Vector3 startPosition = transform.position;

        Vector3 targetPosition =
            startPosition + direction * distance;

        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                _chargeSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = targetPosition;

        //Debug.Log($"폭탄 이동? : {isBomb}");
        //Debug.Log("목표 도착");
        _animator.SetBool("isRun", false);
        StartCoroutine(Bomb());
    }
    
    public IEnumerator Bomb()
    {
        // 1초 뒤 폭발
        //Debug.Log("1초 기다림 시작");
        yield return new WaitForSeconds(1f);

        Vector3 monsterPosition = transform.position;
        Vector3 playerPosition = _player.position;

        // 높이 차이 무시
        monsterPosition.y = 0f;
        playerPosition.y = 0f;

        /*
        float distance = Vector3.Distance(monsterPosition, playerPosition);

        // 현재위치와 플레이어 위치 비교해서 _bombDistance 보다 거리가 작으면 데미지 입힘
        // 이렇게 했지만 나중에 Circle Collider 넣어서 radius로 해도됨
        if (distance <= _bombDistance)
        {
            Debug.Log("1초 기다림 끝 폭탄 터짐: 데미지 받음");
            // 플레이어한테 데미지 주기 
            _playerDamage.TakeDamage(10);      // 테스트
            //Debug.Log($"데미지 받음 거리 :{Vector2.Distance(transform.position, _player.position)}");
        }
        else
        {
            Debug.Log("1초 기다림 끝 폭탄 터짐 : 데미지 안받음");
            //Debug.Log($"데미지 안받음 거리 :{Vector2.Distance(transform.position, _player.position)}");
        }
        */
        
        
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _bombDistance, LayerMask.GetMask("Player"));

        foreach (Collider hit in hitColliders)
        {
            if (hit.TryGetComponent<IDamagable>(out IDamagable damageable))
            {
                damageable.TakeDamage(10); 
            }
        }


        Destroy(gameObject);
    }

    private void SetWarning(Vector3 direction, float distance)
    {
        float angle =
            Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;

        _chargeWarning.transform.rotation =
            Quaternion.Euler(0f, -angle, 0f);

        _chargeWarning.transform.localScale =
            new Vector3(distance, 0.05f, 0.5f);

        Vector3 warningPosition =
            (transform.position + _player.position) / 2f;

        warningPosition.y = transform.position.y;

        _chargeWarning.transform.position = warningPosition;
    }
}

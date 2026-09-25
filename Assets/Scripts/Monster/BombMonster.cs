using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombMonster : MonoBehaviour
{
    [SerializeField] private Transform _player;
    
    // 나중에 Player 스크립트중에 TakeDamage있는 스크립트로 변경
    [SerializeField] private PlayerTest _playerMovment;              // 테스트

    // 테스트
    // [SerializeField] private GameObject _playertest;

    [SerializeField] private GameObject _chargeWarning;

    [SerializeField] private float _warningTime = 0.5f;
    [SerializeField] private float _chargeDistance = 5f;
    [SerializeField] private float _chargeSpeed = 10f;
    [SerializeField] private float _bombDistance = 5f;


    //[SerializeField] private Transform _target;
    private bool isBomb = false;

    private void Awake()
    {
        // 테스트 잘 되는듯 나중에 다시 수정
        /*if (_playertest == null)
        {
            _playertest = GameObject.FindGameObjectWithTag("Player");
        }*/

        if (_player == null)
        {
            _player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        if (_playerMovment == null)         // 테스트
        {
            _playerMovment = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerTest>();
        }

        /*if (_target == null)
        {
            _target = GameObject.Find("GameObject").transform;
        }*/
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isBomb == false)
        {
            isBomb = true;
            //_target.position = other.gameObject.transform.position;
            StartCoroutine(Charge());
        }
    }


    public IEnumerator Charge()
    {
        transform.LookAt(_player.position);
        // 테스트 잘 작동하는 듯 나중에 다 수정
        /*Vector2 direction = (_playertest.gameObject.transform.position - transform.position).normalized;*/

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

        Debug.Log($"폭탄 이동? : {isBomb}");
        Debug.Log("목표 도착");
        StartCoroutine(Bomb());

        /*if (transform.position == _target.position)
        {
            Debug.Log($"폭탄 이동? : {isBomb}");
            Debug.Log("목표 도착");
            StartCoroutine(Bomb());
        }*/
    }

    public IEnumerator Bomb()
    {
        // 1초 뒤 폭발
        Debug.Log("1초 기다림 시작");
        yield return new WaitForSeconds(1f);

        Vector3 monsterPosition = transform.position;
        Vector3 playerPosition = _player.position;

        // 높이 차이 무시
        monsterPosition.y = 0f;
        playerPosition.y = 0f;

        float distance = Vector3.Distance(monsterPosition, playerPosition);

        // 현재위치와 플레이어 위치 비교해서 _bombDistance 보다 거리가 작으면 데미지 입힘
        // 이렇게 했지만 나중에 Circle Collider 넣어서 radius로 해도됨
        if (distance <= _bombDistance)
        {
            Debug.Log("1초 기다림 끝 폭탄 터짐: 데미지 받음");
            // 플레이어한테 데미지 주기 
            _playerMovment.TakeDamage(10);      // 테스트
            //Debug.Log($"데미지 받음 거리 :{Vector2.Distance(transform.position, _player.position)}");
        }
        else
        {
            Debug.Log("1초 기다림 끝 폭탄 터짐 : 데미지 안받음");
            //Debug.Log($"데미지 안받음 거리 :{Vector2.Distance(transform.position, _player.position)}");
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

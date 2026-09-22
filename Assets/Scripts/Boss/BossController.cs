using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    // -------- 부채꼴 공격, 원형 공격
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private int _bulletCount = 10;
    private float _startAngle = 0f;
    private float _startRoundAngle = 0f;
    [SerializeField] private Transform _playerTransform;                // 보스 돌진도 사용
    // -------- 부채꼴 공격, 원형 공격


    // -------- 일반 몬스터 소환 공격
    [SerializeField] private Monster[] _monsters;
    [SerializeField] private GameObject[] _spawnPoints;
    // -------- 일반 몬스터 소환 공격


    // -------- 보스 전체 맵 공격
    [SerializeField] private GameObject _safeZonePrafab;
    [SerializeField] private PlayerTest _player;
    private GameObject newSafeZone;
    [SerializeField] private Monster _monster;
    public Transform centerPoint;    // 기준점 (예: 플레이어 위치)

    [SerializeField] private float minRadius;     // 최소 거리
    [SerializeField] private float maxRadius;    // 최대 거리
    // -------- 보스 전체 맵 공격


    // -------- 보스 돌진 공격
    [SerializeField] private GameObject _chargeWarning;
    [SerializeField] private float _warningTime = 0.5f;
    [SerializeField] private float _chargeDistance = 5f;
    [SerializeField] private float _chargeSpeed = 10f;
    // -------- 보스 돌진 공격


    // -------- 보스 연속 공격
    [SerializeField] private int _shotCount = 4;
    [SerializeField] private float _shotDelay = 0.2f;
    // -------- 보스 연속 공격


    private void Start()
    {
        StartCoroutine(Think());
    }


    IEnumerator Think()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);

            int ranAction = Random.Range(0, 6);

            switch (ranAction)
            {
                case 0:
                    StartCoroutine(AimedBurst());
                    break;

                case 1:
                    FanFire();
                    break;

                case 2:
                    Spawn();
                    break;

                case 3:
                    StartCoroutine(MapAttack());
                    break;

                case 4:
                    RoundFire();
                    break;

                case 5:
                    StartCoroutine(Charge());
                    break;
            }
        }
    }


    // -------- 원형 공격
    private void RoundFire()
    {
        float angleStep = 360f / _bulletCount;

        for (int i = 0; i < _bulletCount; i++)
        {
            float angle = _startRoundAngle + angleStep * i;

            float x = Mathf.Cos(angle * Mathf.Deg2Rad);
            float z = Mathf.Sin(angle * Mathf.Deg2Rad);

            Vector3 direction = new Vector3(x, 0, z);

            FireBullet(direction);
        }

        _startRoundAngle += 30f;
    }
    // -------- 원형 공격


    // -------- 부채꼴 공격
    private void FanFire()
    {
        Vector3 targetDirection =
            (_playerTransform.position - transform.position).normalized;

        targetDirection.y = 0f;

        float angleStep = (_bulletCount > 1) ? 120f / (_bulletCount - 1) : 0f;
        float startAngle = -60f;

        for (int i = 0; i < _bulletCount; i++)
        {
            float currentAngle =
                (_bulletCount > 1) ? startAngle + (angleStep * i) : 0f;

            Vector3 finalDirection =
                Quaternion.Euler(0, currentAngle, 0) * targetDirection;

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
    // -------- 부채꼴 공격


    // -------- 일반 몬스터 소환 공격
    private void Spawn()
    {
        // 문제 같은 장소에 몬스터 2마리가 나올 수도 있음
        for (int i = 0; i < 2; i++)
        {
            int randommosnterindex = Random.Range(0, _monsters.Length);
            int randomspawnindex = Random.Range(0, _spawnPoints.Length);

            Instantiate(
                _monsters[randommosnterindex],
                _spawnPoints[randomspawnindex].gameObject.transform.position,
                _spawnPoints[randomspawnindex].gameObject.transform.rotation
            );
        }
    }
    // -------- 일반 몬스터 소환 공격


    // -------- 보스 전체 맵 공격
    public IEnumerator MapAttack()
    {
        // 이때 보스 몬스터 이동속도 잠시 0으로 만들어야 할듯
        gameObject.transform.position = new Vector3(0, 0, 0);
        _monster.MoveSpeed = 0f;

        StartCoroutine(SpawnSafeZone());

        yield return new WaitForSeconds(3f);

        if (newSafeZone.GetComponent<SafeZone>().IsPlayerInSight == true)
        {
            Debug.Log("안전지대 들어옴");
        }
        else
        {
            Debug.Log("안전 지대 아님 데미지 받음");
            _player.TakeDamage(20);
        }

        Destroy(newSafeZone);

        // 보스 이동속도 원래대로
        _monster.InitSpeed();

        yield return new WaitForSeconds(2f);
    }


    public IEnumerator SpawnSafeZone()
    {
        // 범위 내에서 랜덤 방향 생성
        Vector2 randomDirection = Random.insideUnitCircle.normalized;

        // 최소 거리와 최대 거리 사이의 랜덤한 값 구하기
        float randomDistance = Random.Range(minRadius, maxRadius);

        Vector3 spawnPosition = new Vector3(
            randomDirection.x * randomDistance,
            0f,
            randomDirection.y * randomDistance
        );

        // 오브젝트 생성
        newSafeZone = Instantiate(
            _safeZonePrafab,
            spawnPosition,
            Quaternion.identity
        );

        yield return new WaitForSeconds(2f);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Vector3 origin =
            centerPoint != null ? centerPoint.position : transform.position;

        // 최소, 최대 반경 그리기
        Gizmos.DrawWireSphere(origin, minRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(origin, maxRadius);
    }
    // -------- 보스 전체 맵 공격


    // -------- 보스 돌진 공격
    public IEnumerator Charge()
    {
        Vector3 direction =
            _playerTransform.position - transform.position;

        direction.y = 0f;
        direction.Normalize();

        float distance =
            Vector3.Distance(transform.position, _playerTransform.position);

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
    }


    private void SetWarning(Vector3 direction, float distance)
    {
        float angle =
            Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        _chargeWarning.transform.rotation =
            Quaternion.Euler(0f, angle, 0f);

        // 플레이어까지 거리만큼 경고 길이 설정
        _chargeWarning.transform.localScale =
            new Vector3(1f, 1f, distance);

        // 보스와 플레이어 사이 중앙 위치
        _chargeWarning.transform.position =
            (transform.position + _playerTransform.position) / 2f;
    }
    // -------- 보스 돌진 공격


    // -------- 보스 연속 공격
    public IEnumerator AimedBurst()
    {
        for (int i = 0; i < _shotCount; i++)
        {
            Vector3 direction =
                _playerTransform.position - transform.position;

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
    // -------- 보스 연속 공격
}

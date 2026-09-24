using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMapAttack : BossPattern
{
    [SerializeField] private GameObject _safeZonePrafab;
    [SerializeField] private PlayerTest _playertest;

    private GameObject newSafeZone;
    private Monster _monster;

    private void Awake()
    {
        base.Awake();
        _monster = GetComponent<Monster>();
    }
    
    protected override IEnumerator PatternRoutine()
    {
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
            _playertest.TakeDamage(20);
        }
        Destroy(newSafeZone);

        // 보스 이동속도 원래대로
        _monster.InitSpeed();
    }
    
    [SerializeField] private float minRadius;     // 최소 거리
    [SerializeField] private float maxRadius;    // 최대 거리


    public IEnumerator SpawnSafeZone()
    {
        // 범위 내에서 랜덤 X, Y 좌표 생성
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


    // 에디터 뷰에서 범위를 시각적으로 확인하기 위한 기즈모 (선택 사항)
    [SerializeField] private Transform centerPoint;    // 기즈모 확인 용
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 origin = centerPoint != null ? centerPoint.position : transform.position;

        // 최소, 최대 반경 그리기
        Gizmos.DrawWireSphere(origin, minRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(origin, maxRadius);
    }
}

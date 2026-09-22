using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMapAttack : MonoBehaviour
{
    //[SerializeField] private SafeZone _safeZone;
    [SerializeField] private GameObject _safeZonePrafab;

    [SerializeField] private PlayerTest _player;

    private GameObject newSafeZone;
    // 범위 지정 콜라이더
    // public Collider2D spawnArea;    // 범위를 지정할 2D 트리거 콜라이더

    [SerializeField] private Monster _monster;

    private void Update()
    {
        // �׽�Ʈ��
        if (Input.GetKeyDown(KeyCode.M))
        {
            StartCoroutine(MapAttack());
        }
    }

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
    }

    /*public IEnumerator SpawnSafeZone()
    {
        // 트리거의 2D 바운드(영역) 정보 가져오기
        Bounds bounds = spawnArea.bounds;

        // 범위 내에서 랜덤 X, Y 좌표 생성
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);
        Debug.Log($"{bounds.min.x}  /   {bounds.max.x}");
        Debug.Log($"{bounds.min.y}  /   {bounds.max.y}");
        Debug.Log($"{randomX}  /  {randomY}");

        Vector2 spawnPos = new Vector2(randomX/2, randomY/2);

        // 오브젝트 생성
        newSafeZone = Instantiate(_safeZonePrafab, spawnPos, Quaternion.identity);
        
        yield return new WaitForSeconds(2f);
    }*/

    // 테스트

    public Transform centerPoint;    // 기준점 (예: 플레이어 위치)

    [SerializeField] private float minRadius;     // 최소 거리
    [SerializeField] private float maxRadius;    // 최대 거리


    public IEnumerator SpawnSafeZone()
    {
        // 범위 내에서 랜덤 X, Y 좌표 생성
        Vector2 randomDirection = Random.insideUnitCircle.normalized;

        // 3. 최소 거리와 최대 거리 사이의 랜덤한 값 구하기
        float randomDistance = Random.Range(minRadius, maxRadius);
        //Debug.Log($"{bounds.min.x}  /   {bounds.max.x}");
        //Debug.Log($"{bounds.min.y}  /   {bounds.max.y}");
        //Debug.Log($"{randomX}  /  {randomY}");

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

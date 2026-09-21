using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossChargeAttack : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private GameObject _chargeWarning;

    [SerializeField] private float _warningTime = 0.5f;
    [SerializeField] private float _chargeDistance = 5f;
    [SerializeField] private float _chargeSpeed = 10f;

    private void Update()
    {
        // 테스트용
        if (Input.GetKeyDown(KeyCode.C))
        {
            StartCoroutine(Charge());
        }
    }

    public IEnumerator Charge()
    {
        Vector3 direction =
            _player.position - transform.position;

        // 높이 차이는 무시
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
    }

    private void SetWarning(Vector3 direction, float distance)
    {
        float angle =
            Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;

        // XZ 평면이므로 Y축 회전
        _chargeWarning.transform.rotation =
            Quaternion.Euler(0f, -angle, 0f);

        // Cube의 X축을 길이 방향으로 사용
        _chargeWarning.transform.localScale =
            new Vector3(distance, 0.05f, 0.5f);

        // 보스와 플레이어 사이 중앙
        Vector3 warningPosition =
            (transform.position + _player.position) / 2f;

        // 바닥에 묻히면 이 값을 조금 올려주면 됨
        warningPosition.y = transform.position.y;

        _chargeWarning.transform.position = warningPosition;
    }
}

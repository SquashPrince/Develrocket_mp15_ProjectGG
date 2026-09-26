using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossChargeAttack : BossPattern
{
    // [SerializeField] private Transform _player;
    [SerializeField] private GameObject _chargeWarning;

    [SerializeField] private float _warningTime = 0.5f;
    //[SerializeField] private float _chargeDistance = 5f;
    [SerializeField] private float _chargeSpeed = 10f;

    [SerializeField] private BossMovement _bossMovement;
    
    protected override IEnumerator PatternRoutine()
    {
        Vector3 direction =
            _player.position - transform.position;
        
        Transform target = _player.transform;

        // 타겟 플레이어 당시 위치로 바꾸기
        _bossMovement.SetTargetChange(target);
        // 타겟 플레이어 당시 위치로 바꾸기
        
        
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

        _bossMovement.canRotate = true;
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

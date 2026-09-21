using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeMonster : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private GameObject _chargeWarning;

    [SerializeField] private float _warningTime = 0.5f;
    [SerializeField] private float _chargeDistance = 5f;
    [SerializeField] private float _chargeSpeed = 10f;

    private bool _isCharge = false;


    //[SerializeField] private Transform _target;


    private void Awake()
    {
        if (_player == null)
        {
            _player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && _isCharge == false)
        {
            _isCharge = true;
            StartCoroutine(Charge());
        }
    }


    public IEnumerator Charge()
    {
        Vector2 direction =
            (_player.position - transform.position).normalized;
        float distance =
            Vector2.Distance(transform.position, _player.position);

        SetWarning(direction, distance);

        _chargeWarning.SetActive(true);

        yield return new WaitForSeconds(_warningTime);

        _chargeWarning.SetActive(false);

        Vector2 startPosition = transform.position;

        Vector2 targetPosition =
            startPosition + direction * distance;

        while (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                targetPosition,
                _chargeSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = targetPosition;
        Debug.Log("목표 도착");
        _isCharge = false;
    }

    private void SetWarning(Vector2 direction, float distance)
    {
        float angle =
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        _chargeWarning.transform.rotation =
            Quaternion.Euler(0f, 0f, angle);

        _chargeWarning.transform.localScale =
            new Vector3(distance, 1f, 1f);

        _chargeWarning.transform.position =
            (transform.position + _player.position) / 2f;
    }
}

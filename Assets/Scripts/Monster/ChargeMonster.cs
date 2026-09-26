using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeMonster : MonoBehaviour
{
    private Transform _player;
    [SerializeField] private GameObject _chargeWarning;

    [SerializeField] private float _warningTime = 0.5f;
    //[SerializeField] private float _chargeDistance = 5f;
    [SerializeField] private float _chargeSpeed = 10f;
    private Animator _animator;

    private bool _isCharge = false;


    //[SerializeField] private Transform _target;


    public void Start()
    {
        StartCoroutine(InitRoutine());
    }
    
    public IEnumerator InitRoutine()
    {
        yield return new WaitUntil(()=> GameManager.Instance.PlayerTransform != null);
        _player = GameManager.Instance.PlayerTransform;
    }


    private void OnTriggerEnter(Collider other)
    {
        /*if (other.CompareTag("Player") && _isCharge == false)
        {
            _isCharge = true;
            StartCoroutine(Charge());
        }
        */

        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && _player != null)
        {
            _isCharge = true;
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
        Debug.Log("목표 도착");
        _isCharge = false;
        _animator.SetBool("isRun", false);
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

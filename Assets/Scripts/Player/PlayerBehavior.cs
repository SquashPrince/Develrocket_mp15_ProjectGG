using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Player
{
    public class PlayerBehavior : MonoBehaviour, IDamagable
    {
    private static PlayerInputManager PlayerInput => PlayerInputManager.Instance;
    private PlayerValues _playerValues;
    private List<IInteractable> _interactableList;
    private IInteractable _targetInteractable;
    private Vector3 _direction;
    private bool _hasDetectInteractable => _interactableList.Count > 0;
    public GameObject GameObject => gameObject;
    private Coroutine _dodging;
    private bool _isDodging;
    // ========================================

    private void Awake() => CacheComponents();

    private void Update()
    {
        PlayerMove();
    }

    private void OnEnable()
    {
        PlayerInput.OnInteract += OnInteract;
        PlayerInput.OnMove += OnMove;
        PlayerInput.OnDodge += OnDodge;
    }

    private void OnDisable()
    {
        PlayerInput.OnInteract -= OnInteract;
        PlayerInput.OnMove -= OnMove;
        PlayerInput.OnDodge -= OnDodge;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.TryGetComponent<IInteractable>(out IInteractable inter)) return;
        _interactableList.Add(inter);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.TryGetComponent<IInteractable>(out IInteractable inter)) return;
        _interactableList.Remove(inter);
    }

    // ========================================

    private void CacheComponents()
    {
        _playerValues = GetComponent<PlayerValues>();
    }

    // ========================================

    private void OnInteract()
    {
        if (!_hasDetectInteractable) return;
        _targetInteractable = _interactableList[0];

        // _targetInteractable.Interact(_playerValues); TODO: 인터페이스 확인 필요

        _targetInteractable = null;
    }

    // 상호작용
    // ========================================

    private void OnMove(Vector3 direction)
    {
        SetDirection(direction);
    }

    private void PlayerMove()
    {
        int dodgeRange = _isDodging ? _playerValues.MoveSpeed * 2 : _playerValues.MoveSpeed;
        transform.position += _direction * (dodgeRange * Time.deltaTime);
    }


    private void SetDirection(Vector3 direction)
    {
        if (_isDodging) return;
        _direction = direction;
    }

    // 이동
    // ========================================

    private void OnDodge()
    {
        _isDodging = true;
        StartCoroutine(Dodging());
    }

    private IEnumerator Dodging()
    {
        yield return new WaitForSeconds(_playerValues.DodgeTime);
        _isDodging = false;
    }

    public void TakeDamage(int damage)
    {
        if (!_isDodging) _playerValues.Hp -= damage;
    }
    // TODO: 회피 구현 필요
    // 회피
    // ========================================


    }
}
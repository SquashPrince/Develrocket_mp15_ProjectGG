using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Player
{
    public class PlayerBehavior : MonoBehaviour
    {
        private static PlayerInputManager PlayerInput => PlayerInputManager.Instance;
        private PlayerValues _playerValues; 
        private List<IInteractable> _interactableList;
        private IInteractable _targetInteractable;
        private bool _hasDetectInteractable => _interactableList.Count > 0;
        private Vector3 _direction;

        private Coroutine _dodging;
        
        
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
        }

        private void OnDisable()
        {
            PlayerInput.OnInteract -= OnInteract;
            PlayerInput.OnMove -= OnMove;
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
            transform.position += _direction * _playerValues.MoveSpeed * Time.deltaTime;
        }
        

        private void SetDirection(Vector3 direction)
        {
            if (_playerValues.isDodging) return;
            _direction = direction;
        }
        
        // 이동
        // ========================================

        private void OnDodge()
        {
            StartCoroutine(Dodging());

        }
        private IEnumerator Dodging()
        {
            _playerValues.isDodging = true;
            yield return new WaitForSeconds(_playerValues.DodgeTime);
            _playerValues.isDodging = false;
        }
        // TODO: 회피 구현 필요
        // 회피
        // ========================================
    
    
    }
}
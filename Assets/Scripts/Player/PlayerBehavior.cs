using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerBehavior : MonoBehaviour
    {
        private static PlayerInputManager PlayerInput => PlayerInputManager.Instance;
        private PlayerValues _playerValues; 
        private List<IInteractable> _interactableList;
        private IInteractable _targetInteractable;
        private Rigidbody _rigidbody;
        
        private Vector3 _direction;
        
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
            _rigidbody = GetComponentInChildren<Rigidbody>();
        }
        
        // ========================================

        private void OnInteract()
        {
            if (_interactableList.Count == 0) return;
            TargetInteractable();
        }

        private void TargetInteractable()
        {
            _targetInteractable = _interactableList[0];
        }

        private void Interact()
        {
            // 인터랙터블 인터페이스의 상호작용 메서드를 실행
            // _targetInteractable.Interact();
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
            _direction = direction;
        }
        
        // 이동
        // ========================================
    
    
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerBehavior : MonoBehaviour
    {
        private static PlayerInputManager PlayerInput => PlayerInputManager.Instance;
        private PlayerValues _playerValues; 
        private List<IInteractable> _fieldItemList;
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
            _fieldItemList.Add(inter);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.TryGetComponent<IInteractable>(out IInteractable inter)) return;
            _fieldItemList.Remove(inter);
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
            TargetInteractable();
        }

        private void OnMove(Vector3 direction)
        {
            SetDirection(direction);
        }

        private void TargetInteractable()
        {
            if (_fieldItemList.Count == 0) return;
            _targetInteractable = _fieldItemList[0];
        }
        
        // ========================================

        private void PlayerMove()
        {
            transform.position += _direction * _playerValues.MoveSpeed * Time.deltaTime;
        }
        

        private void SetDirection(Vector3 direction)
        {
            _direction = direction;
        }
    
    
    }
}
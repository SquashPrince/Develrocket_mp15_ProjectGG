using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerBehavior : MonoBehaviour
    {
        public PlayerInputManager PlayerInput => PlayerInputManager.Instance;
        
        private PlayerValues _playerValues; 
        private List<IInteractable> _fieldItemList;
        private IInteractable _targetInteractable;
        
        // ========================================
        
        private void Awake() => CacheComponents();

        private void OnEnable()
        {
            PlayerInputManager.Instance.OnInteract += OnInteract;
        }

        private void OnDisable()
        {
            PlayerInputManager.Instance.OnInteract -= OnInteract;
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
        }
        
        // ========================================

        private void OnInteract()
        {
            TargetInteractable();
        }

        private void TargetInteractable()
        {
            if (_fieldItemList.Count == 0) return;
            _targetInteractable = _fieldItemList[0];
        }
        
        // ========================================

        private void PlayerMovement(Vector2 input)
        {
            Vector3 velocity = new Vector3(
                input.x * _playerValues.GetMo);
        }
    
    
    }
}
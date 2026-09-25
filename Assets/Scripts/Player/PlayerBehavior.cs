using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

namespace Player
{
    public class PlayerBehavior : MonoBehaviour, IDamagable
    {
        private static PlayerInputManager PlayerInput => PlayerInputManager.Instance;
        private PlayerValues _playerValues;
        private List<IInteractable> _interactableList = new();
        private IInteractable _targetInteractable;
        private Camera _camera;
        private Vector3 _direction;
        private Vector3 _cursorPosition;
        private bool _hasDetectInteractable => _interactableList.Count > 0;
        public GameObject GameObject => gameObject;
        private Coroutine _dodging;
        private bool _isDodging;
        private LayerMask _groundLayerMask;
        // ========================================
        private void Awake() => CacheComponents();

        private void Update()
        {
            PlayerMove();
            PlayerCursor();
        }

        private void OnEnable()
        {
            PlayerInput.OnInteract += OnInteract;
            PlayerInput.OnMove += OnMove;
            PlayerInput.OnDodge += OnDodge;
            PlayerInput.OnSwap += OnSwap;
            PlayerInput.OnShot += OnShot;
        }

        private void OnDisable()
        {
            PlayerInput.OnInteract -= OnInteract;
            PlayerInput.OnMove -= OnMove;
            PlayerInput.OnDodge -= OnDodge;
            PlayerInput.OnSwap -= OnSwap;
            PlayerInput.OnShot -= OnShot;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.TryGetComponent<IInteractable>(out IInteractable inter)) return;
            _interactableList.Add(inter);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.TryGetComponent<IInteractable>(out IInteractable inter)
                || !_playerValues._hasSuccessInteract) return;
            _playerValues._hasSuccessInteract = false;
            _interactableList.Remove(inter);
        }

        // ========================================
        private void CacheComponents()
        {
            _playerValues = GetComponent<PlayerValues>();
            _camera = Camera.main;
        }

        // ========================================

        private void OnInteract()
        {
            if (!_hasDetectInteractable) return;
            Debug.Log($"{_interactableList[0].Name} : 상호작용 시도");
            _interactableList[0].Interact(_playerValues);
            
            _interactableList.RemoveAt(0);
        }
        
        // 무기교체
        // ========================================
        private void OnSwap()
        {
            _playerValues.SwapNextWeapon();
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
            if (_isDodging) return;
            _playerValues.Hp -= damage;
        }
        // 회피
        // ========================================

        private void PlayerCursor()
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            if (plane.Raycast(ray, out float rayLength))
            {
                Vector3 targetPosition = ray.GetPoint(rayLength);
                transform.LookAt(new Vector3(targetPosition.x, transform.position.y, targetPosition.z));
            }
        }

        // 마우스 포인터
        // ========================================

        private void OnShot()
        {
            if (_playerValues.EquippedWeapon == null) return;
            _playerValues.EquippedWeapon.Fire(_playerValues.DodgeTime);
        }
        // 발사
        // ========================================
        
        
    }
}
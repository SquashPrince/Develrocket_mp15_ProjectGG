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

        // 아이템 정보 표시용
        public IInteractable TargetInteractable => _targetInteractable;
        public Item TargetItem => _targetInteractable as Item;
        public event Action<IInteractable> OnTargetInteractableChanged;

        private string _itemToastMessage;
        //

        [SerializeField] private Renderer[] _hitRenderers;
        private bool[] _rendererEnabledBeforeHit;
        private float _hitBlinkInterval = 0.25f;
        private bool _isHitInvincible;
        private Coroutine _hitInvincibleRoutine;

        public GameObject GameObject => gameObject;
        private Coroutine _dodging;
        private bool _isDodging;
        private bool _isDodgeCooldown;
        private LayerMask _groundLayerMask;
        // ========================================
        private void Awake() => CacheComponents();

        private void Update()
        {
            if (GameManager.Instance != null && !GameManager.Instance.CanPlay) return;
            PlayerMove();
            PlayerCursor();

            RefreshInteractTarget();
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

            _interactableList.Clear();
            RefreshInteractTarget();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.TryGetComponent<IInteractable>(out IInteractable inter)) return;

            if (!_interactableList.Contains(inter))
                _interactableList.Add(inter);

            RefreshInteractTarget();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.TryGetComponent<IInteractable>(out IInteractable inter)) return;

            _playerValues._hasSuccessInteract = false;
            _interactableList.Remove(inter);

            RefreshInteractTarget();
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
            // 유효하지 않은 아이템 정리 및 표시 대상 갱신
            RefreshInteractTarget();

            if (!_hasDetectInteractable) return;

            Debug.Log($"{_interactableList[0].Name} : 상호작용 시도");
            
            IInteractable target = _interactableList[0]; 
            target.Interact(_playerValues);
            
            // 슬롯이 가득 차서 획득하지 못한 아이템은 재시도할 수 있다.
            if (target is Item item && item != null && item.CanInteract) return;
            
            _interactableList.Remove(target);

            // 획득후 다음 아이템 표시 or 표시 종료
            RefreshInteractTarget();
        }

        // 무기교체
        // ========================================
        private void OnSwap(int direction)
        {
            if (direction == 0) return;

            _playerValues.SwapNextWeapon(direction);
        }
        

        // 상호작용
        // ========================================

        private void OnMove(Vector3 direction)
        {
            SetDirection(direction);
        }

        private void PlayerMove()
        {
            int dodgeRange = _isDodging ? _playerValues.MoveSpeed * 3 : _playerValues.MoveSpeed;
            transform.position += _direction * (dodgeRange * Time.deltaTime);
        }


        private void SetDirection(Vector3 direction)
        {
            if (_isDodging) return;
            _direction = direction;
        }

        // 아이템 정보 출력용
        // ========================================
        private void RefreshInteractTarget()
        {
            _interactableList.RemoveAll(
                target => target is not UnityEngine.Object obj ||
                obj == null ||
                (target is Item item &&
                 (!item.CanInteract || !item.gameObject.activeInHierarchy))
                 );

            IInteractable nextTarget = _interactableList.Count > 0 ? _interactableList[0] : null;
            
            if (ReferenceEquals(_targetInteractable, nextTarget)) return;

            _targetInteractable = nextTarget;

            // 디버그용
            if (_targetInteractable != null)
            {
                _itemToastMessage = $"{_targetInteractable.Name}\n{_targetInteractable.Info}";
                Debug.Log(_itemToastMessage);

            }
            else
            {
                _itemToastMessage = null;
                Debug.Log("[아이템 정보] 표시 종료");
            }
            ///

            OnTargetInteractableChanged?.Invoke(_targetInteractable);
        }

        // 아이템 정보 출력용 임시 토스트 메세지
        private void OnGUI()
        {
            Item target = TargetItem;

            if (target == null || _camera == null ||
                string.IsNullOrEmpty(_itemToastMessage)) return;

            // 아이템보다 약간 위의 위치
            Vector3 worldPosition = target.transform.position + Vector3.up * 1.5f;
            Vector3 screenPosition = _camera.WorldToScreenPoint(worldPosition);

            // 카메라 뒤에 있으면 표시하지 않음
            if (screenPosition.z <= 0f)
                return;

            float width = 300f;
            float height = 80f;

            // 화면 좌표는 왼쪽 아래 기준,
            // OnGUI 좌표는 왼쪽 위 기준이므로 Y를 뒤집음
            Rect rect = new Rect(
                screenPosition.x - width * 0.5f,
                Screen.height - screenPosition.y - height - 10f,
                width,
                height);

            GUIStyle style = new GUIStyle(GUI.skin.box)
            {
                fontSize = 20,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true
            };

            style.normal.textColor = Color.white;

            GUI.Box(rect, _itemToastMessage, style);
        }


        // 이동
        // ========================================

        private void OnDodge()
        {
            if (_isDodgeCooldown) return;
            // 회피 시작 시 이동 방향을 바라보고, 정지 상태면 기존 시선을 유지한다.
            if (_direction.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(_direction);
            _isDodging = true;
            _isDodgeCooldown = true;
            StartCoroutine(Dodging());
        }

        private IEnumerator Dodging()
        {
            yield return new WaitForSeconds(_playerValues.DodgeTime);
            _isDodging = false;
            yield return new WaitForSeconds(_playerValues.DodgeCoolDown-_playerValues.DodgeTime);
            _isDodgeCooldown = false;
        }

        public void TakeDamage(int damage)
        {
            if (_isDodging || damage <= 0 || _isHitInvincible) return;
            if (_playerValues.TryConsumeDamageShield()) return;
            if (_playerValues.Shield > 0)
            {
                // 보호막이 조금이라도 있으면 초과 피해는 HP로 넘어가지 않는다.
                _playerValues.Shield -= damage;
                return;
            }
            _playerValues.Hp -= damage;

            _hitInvincibleRoutine = StartCoroutine(HitInvincibility());
        }


        // 피격시 무적
        // ========================================

        private IEnumerator HitInvincibility()
        {
            _isHitInvincible = true;

            _rendererEnabledBeforeHit = new bool[_hitRenderers.Length];

            for (int i = 0; i < _hitRenderers.Length; i++)
            {
                if (_hitRenderers[i] != null)
                    _rendererEnabledBeforeHit[i] = _hitRenderers[i].enabled;
            }

            float elapsed = 0f;
            float blinkElapsed = 0f;
            float interval = Mathf.Max(0.01f, _hitBlinkInterval);
            bool visible = false;

            for (int i = 0; i < _hitRenderers.Length; i++)
            {
                if (_hitRenderers[i] != null)
                {
                    _hitRenderers[i].enabled =
                        visible && _rendererEnabledBeforeHit[i];
                }
            }

            while (elapsed < _playerValues.BaseHitTime)
            {
                yield return null;

                elapsed += Time.deltaTime;
                blinkElapsed += Time.deltaTime;

                if (blinkElapsed >= interval)
                {
                    blinkElapsed %= interval;
                    visible = !visible;

                    for (int i = 0; i < _hitRenderers.Length; i++)
                    {
                        if (_hitRenderers[i] != null)
                        {
                            _hitRenderers[i].enabled =
                                visible && _rendererEnabledBeforeHit[i];
                        }
                    }
                }
            }

            for (int i = 0; i < _hitRenderers.Length; i++)
            {
                if (_hitRenderers[i] != null)
                    _hitRenderers[i].enabled = _rendererEnabledBeforeHit[i];
            }

            _rendererEnabledBeforeHit = null;
            _isHitInvincible = false;
            _hitInvincibleRoutine = null;
        }

        // 회피
        // ========================================

        private void PlayerCursor()
        {
            if (_isDodging) return;

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
            _playerValues.EquippedWeapon.Fire(_playerValues.DamageMultiplier);
        }
        // 발사
        // ========================================
        
        
    }
}

using UnityEngine;
using System;

namespace Player
{
    public class PlayerInputManager : MonoBehaviour
    {
        // ========================================
        
        private const string AXIS_HORIZONTAL = "Horizontal";
        private const string AXIS_VERTICAL = "Vertical";
        
        private KeyCode _interactKey = KeyCode.E; // 상호작용
        private KeyCode _dodgeKey = KeyCode.Space; // 회피
        private KeyCode _reLoadKey = KeyCode.R; // 장전
        private KeyCode _shotKey = KeyCode.Mouse0; // 발사
        private KeyCode _swapKey = KeyCode.Tab; // 무기교체
        private KeyCode _Item1Key = KeyCode.Alpha1;
        private KeyCode _Item2Key = KeyCode.Alpha2;
        private KeyCode _Item3Key = KeyCode.Alpha3;
        
        // 필드
        // ========================================
        
        public event Action OnInteract;
        public event Action OnShot;
        public event Action OnDodge;
        public event Action OnSwap;
        public event Action<Vector3> OnMove;
        public event Action OnItem1;
        public event Action OnItem2;
        public event Action OnItem3;
        
        // 이벤트
        // ========================================
        
        private static PlayerInputManager _instance;
        
        public static PlayerInputManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<PlayerInputManager>();
                    DontDestroyOnLoad(_instance);
                }
                return _instance;
            }
        }
        
        // ========================================

        private void Awake() => SetSingleTon();

        private void Update() => ReadInput();
        
        // ========================================

        /** 싱글톤 설정 */
        private void SetSingleTon()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
        
        // ========================================

        /** 인풋 확인 후 이벤트 알림 */
        private void ReadInput()
        {
            if (Input.GetKeyDown(_interactKey)) OnInteract?.Invoke();
            if (Input.GetKeyDown(_shotKey)) OnShot?.Invoke();
            OnMove?.Invoke(GetDirection());
            if (Input.GetKeyDown(_dodgeKey)) OnDodge?.Invoke();
            if (Input.GetKeyDown(_swapKey)) OnSwap?.Invoke();
            if (Input.GetKeyDown(_Item1Key)) OnItem1?.Invoke();
            if (Input.GetKeyDown(_Item2Key)) OnItem2?.Invoke();
            if (Input.GetKeyDown(_Item3Key)) OnItem3?.Invoke();
        }

        
        private Vector3 GetDirection()
        {
            float horizontal = Input.GetAxisRaw(AXIS_HORIZONTAL);
            float vertical = Input.GetAxisRaw(AXIS_VERTICAL);
            return new Vector3(horizontal, 0f, vertical).normalized;
        }
        
    }
}

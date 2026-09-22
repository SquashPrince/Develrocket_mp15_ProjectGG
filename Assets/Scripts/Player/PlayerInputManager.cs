using UnityEngine;
using System;

namespace Player
{
    public class PlayerInputManager : MonoBehaviour
    {
        // ========================================
        
        private KeyCode _interactKey = KeyCode.E;
        private KeyCode _dodgeKey = KeyCode.Space;
        private KeyCode _reLoadKey = KeyCode.R;
        private KeyCode _shotKey = KeyCode.Mouse0;
        
        public event Action OnInteract;
        public event Action OnReload;
        public event Action OnShot;
        public event Action<Vector2> OnMove;
        public event Action<Vector2> OnDodge;
        
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

        private void ReadInput()
        {
            if (Input.GetKeyDown(_interactKey)) OnInteract?.Invoke();
            if (Input.GetKeyDown(_shotKey)) OnShot?.Invoke();
            if (Input.GetKeyDown(_reLoadKey)) OnReload?.Invoke();
            OnMove?.Invoke(GetMovement());
            if (Input.GetKeyDown(_dodgeKey)) OnDodge?.Invoke(GetMovement());
        }

        private Vector2 GetMovement()
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            return (new Vector2(x, z)).normalized;
        }
        
    }
}

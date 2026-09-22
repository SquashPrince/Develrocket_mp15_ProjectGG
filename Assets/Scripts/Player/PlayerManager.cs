using UnityEngine;

namespace Player
{
    public class PlayerManager : MonoBehaviour
    {
        private static PlayerManager _instance;
        
        public static PlayerManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<PlayerManager>();
                    DontDestroyOnLoad(_instance);
                }
                return _instance;
            }
        }

        private void Awake() => SetSingleTon();

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




    }
}

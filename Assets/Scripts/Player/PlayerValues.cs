using UnityEngine;

namespace Player
{
    public class PlayerValue : MonoBehaviour
    {
        private PlayerValueStruct _value;



        private void Awake()
        {
            CacheComponents();
        }

        private void CacheComponents()
        {
            _value = new PlayerValueStruct();
        }
        
        
    }
    
    
}

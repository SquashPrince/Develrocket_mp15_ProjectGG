using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public interface IInteractable
    {
        // 임시 인터페이스, PR시 삭제
    }

    public class PlayerInteractor : MonoBehaviour
    {
        private List<IInteractable> _itemList;

        private void OnTriggerEnter(Collider other)
        {
            other.gameObject.TryGetComponent<IInteractable>(out IInteractable inter);
            if (inter == null) return;
            _itemList.Add(inter);
        }

        private void OnTriggerExit(Collider other)
        {
            other.gameObject.TryGetComponent<IInteractable>(out IInteractable inter);
            if (inter == null) return;
            _itemList.Remove(inter);
        }

        public IInteractable GetInteractable()
        {
            if (_itemList.Count == 0) return null;
            IInteractable inter = _itemList[0];
            _itemList.RemoveAt(0);
            return inter;
        }
    
    
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
}

// 임시 상호작용 인터페이스

public class PlayerInput : MonoBehaviour
{
    private KeyCode _interactKey = KeyCode.E;
    private Camera _camera;
    
    [SerializeField] LayerMask targetLayerMask;
    
    private IInteractable _interactable;

    private bool HasDetectInteractable => _interactable != null;
    // 상호작용 가능 인터페이스
    
    
    private void Update()
    {
        
    }

    private void ReadinputInteract()
    {
        // 마우스 커서 위치로 레이캐스트
        // 상호작용 대상 레이어
        if (!Input.GetKeyDown(_interactKey)) return;
    }

    public void DetectInteractable()
    {
        Ray ray = new Ray(_camera.transform.position,
            _camera.transform.forward);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, 50))
        {
            if (HasDetectInteractable)
            {
                _interactable = null;
            }
            return;
        }
    }
    
    
    
}

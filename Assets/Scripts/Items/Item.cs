using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    [SerializeField] private string _name;
    [SerializeField] private string _info;

    public string Name => _name;
    public string Info => _info;

    /// <summary>
    /// 상호작용이 가능한 상태인지 확인
    /// </summary>
    public bool CanInteract = true;

    /// <summary>
    /// 각 아이템 별로 아이템 습득 효과구현
    /// </summary>
    /// <param name="owner"></param>
    public virtual void Interact(IInteractor owner) { }
}

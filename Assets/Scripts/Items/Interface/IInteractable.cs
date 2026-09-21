using UnityEngine;

public interface IInteractable
{
    /// <summary>
    /// 아이템 이름
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 아이템 설명
    /// </summary>
    public string Info { get; }

    /// <summary>
    /// 상호작용 override 구현
    /// </summary>
    /// <param name="owner"></param>
    public virtual void GetItem(IInteracter owner) { }
}

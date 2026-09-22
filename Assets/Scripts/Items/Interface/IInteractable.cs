using UnityEngine;

public interface IInteractable
{
    string Name { get; }
    string Info { get; }
    void Interact(IInteractor owner);

    public virtual void SetEquip(Transform transform) { }
    public virtual void SetUnEquip() { }
}

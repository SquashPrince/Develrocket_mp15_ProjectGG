using UnityEngine;

public interface IInteractable
{
    public string Name { get; }

    public string Info { get; }

    public virtual void GetItem(IInteracter owner) { }
}

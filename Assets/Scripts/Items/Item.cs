using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    [SerializeField] private string _name;
    [SerializeField] private string _info;

    public string Name => _name;
    public string Info => _info;

    public bool CanInteract = true;

    public virtual void GetItem(IInteracter owner)
    {

    }
}

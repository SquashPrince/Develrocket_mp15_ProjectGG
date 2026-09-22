using UnityEngine;

public abstract class ItemEffect : ScriptableObject
{
    public abstract void Apply(IInteractor interacter, float amount, float time);
}

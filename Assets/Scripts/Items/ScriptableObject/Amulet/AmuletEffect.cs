using UnityEngine;

public abstract class AmuletEffect : ScriptableObject
{
    public abstract void Apply(IInteractor interacter, float amount);
}

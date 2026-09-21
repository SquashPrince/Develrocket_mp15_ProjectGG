using UnityEngine;

public abstract class AmuletEffect : ScriptableObject
{
    public abstract void Apply(IInteracter interacter);
}

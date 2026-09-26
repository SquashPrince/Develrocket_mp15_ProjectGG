using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/itemEffect/Barrier Item")]
public class BarrierItemEffect : ItemEffect
{
    public override void Apply(IInteractor interacter, float amount, float time)
    {
        if (interacter != null) interacter.Shield += Mathf.RoundToInt(amount);
    }
}

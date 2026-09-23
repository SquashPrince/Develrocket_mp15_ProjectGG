using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/AmuletEffect/Barrier Amulet")]
public class BarrierAmuletEffect : AmuletEffect
{
    public override void Apply(IInteractor interacter, float amount)
    {
        interacter.BaseMaxShield += Mathf.RoundToInt(amount);
    }
}

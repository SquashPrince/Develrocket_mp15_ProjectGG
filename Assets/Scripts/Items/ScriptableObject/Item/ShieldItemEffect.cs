using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/itemEffect/Shield Item")]
public class ShieldItemEffect : ItemEffect
{
    public override void Apply(IInteractor interacter, float amount, float time)
    {
        if (interacter != null) interacter.DamageShieldCharges = 1;
    }
}

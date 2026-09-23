using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/AmuletEffect/Damage Amulet")]
public class DamageAmuletEffect : AmuletEffect
{
    public override void Apply(IInteractor interacter, float amount)
    {
        interacter.DamageMultiplier += amount / 100f;
    }
}

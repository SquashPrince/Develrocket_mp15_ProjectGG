using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/AmuletEffect/Gold Amulet")]
public class GoldAmuletEffect1 : AmuletEffect
{
    public override void Apply(IInteractor interacter, float amount)
    {
        interacter.rateGainGold += Mathf.RoundToInt(amount);
    }
}

using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/AmuletEffect/Health Amulet")]
public class HealthAmuletEffect : AmuletEffect
{
    public override void Apply(IInteractor interacter, float amount)
    {
        interacter.BaseMaxHp += Mathf.RoundToInt(amount);
    }
}

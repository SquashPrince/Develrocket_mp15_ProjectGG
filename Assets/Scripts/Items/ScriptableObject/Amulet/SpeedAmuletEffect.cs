using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/AmuletEffect/Speed Amulet")]
public class SpeedAmuletEffect : AmuletEffect
{
    public override void Apply(IInteractor interacter, float amount)
    {
        interacter.rateMoveSpeed += Mathf.RoundToInt(amount);
    }
}

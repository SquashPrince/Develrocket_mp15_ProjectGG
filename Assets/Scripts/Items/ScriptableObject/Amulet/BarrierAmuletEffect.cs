using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/AmuletEffect/Barrier Amulet")]
public class BarrierAmuletEffect : AmuletEffect
{
    public override void Apply(IInteractor interacter, float amount)
    {
        Debug.Log(interacter.BaseMaxShield);

        interacter.BaseMaxShield += Mathf.RoundToInt(amount);

        Debug.Log(interacter.BaseMaxShield);
    }
}

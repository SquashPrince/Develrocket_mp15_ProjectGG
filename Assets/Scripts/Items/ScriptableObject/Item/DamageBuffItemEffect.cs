using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/itemEffect/Damage Buff Item")]
public class DamageBuffItemEffect : BuffItemEffct
{
    public override void Apply(IInteractor interacter, float amount, float time)
    {
        BuffActor buff = CreateBuff(interacter);
        if (buff == null) return;
        Transform target = interacter.Transform;
        float increase = amount / 100f;
        buff.StartBuff(() =>
        {
            if (target != null) interacter.DamageMultiplier += increase;
        }, () =>
        {
            if (target != null) interacter.DamageMultiplier -= increase;
        }, time);
    }
}

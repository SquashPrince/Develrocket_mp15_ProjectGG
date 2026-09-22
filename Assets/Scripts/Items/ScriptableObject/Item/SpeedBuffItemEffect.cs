using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/itemEffect/Speed Buff Item")]
public class SpeedBuffItemEffect : BuffItemEffct
{
    public override void Apply(IInteractor interacter, float amount, float time)
    {
        BuffActor buff = CreateBuff(interacter);
        if (buff == null) return;
        Transform target = interacter.Transform;
        int rateAmount = Mathf.RoundToInt(amount);
        buff.StartBuff(() =>
        {
            if (target != null) interacter.rateMoveSpeed += rateAmount;
        }, () =>
        {
            if (target != null) interacter.rateMoveSpeed -= rateAmount;
        }, time);
    }
}

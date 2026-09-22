using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/itemEffect/Shield Item")]
public class ShieldItemEffect : ItemEffect
{
    public override void Apply(IInteractor interacter, float amount, float time)
    {
        // TODO: 무효화 효과 구현. 전용 프리팹을 생성하고 피격 전까지 유지.
        Debug.Log("무효화 효과 구현 필요: 쉴드 아이템 사용");
    }
}

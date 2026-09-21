using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/AmuletEffect/Barrier Amulet")]
public class BarrierAmuletEffect : AmuletEffect
{
    [SerializeField] private int _amount;

    public override void Apply(IInteracter player)
    {
        // 플레이어 보호막 최대치 증가
        Debug.Log($"보호막 최대치 {_amount} 증가");
    }
}

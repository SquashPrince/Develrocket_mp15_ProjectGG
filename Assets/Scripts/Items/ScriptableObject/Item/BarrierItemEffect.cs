using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/itemEffect/Barrier Item")]
public class BarrierItemEffect : ItemEffect
{
    [SerializeField] private int _amount;
    public override void Apply(IInteracter interacter)
    {
        // 플레이어 보호막 회복
        Debug.Log($"{_amount} 만큼 플레이어 보호막 회복");
    }
}
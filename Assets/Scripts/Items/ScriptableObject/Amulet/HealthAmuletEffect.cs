using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/AmuletEffect/Health Amulet")]
public class HealthAmuletEffect : AmuletEffect
{
    [SerializeField] private int _amount;

    public override void Apply(IInteracter player)
    {
        // 플레이어 최대 체력 증가
        Debug.Log($"최대 체력 {_amount} 증가");
    }
}

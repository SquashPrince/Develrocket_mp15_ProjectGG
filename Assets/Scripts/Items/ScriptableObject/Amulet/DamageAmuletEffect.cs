using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/AmuletEffect/Damage Amulet")]
public class DamageAmuletEffect : AmuletEffect
{
    [SerializeField] private int _amount;

    public override void Apply(IInteracter player)
    {
        // 플레이어 공격력 증가
        Debug.Log($"공격력 {_amount} 증가");
    }
}

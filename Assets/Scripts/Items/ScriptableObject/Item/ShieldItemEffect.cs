using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/itemEffect/Shield Item")]
public class ShieldItemEffect : ItemEffect
{
    [SerializeField] private int _amount;
    public override void Apply(IInteracter interacter)
    {
        // 플레이어 무효화
        Debug.Log($"{_amount} 만큼 플레이어 무효화");
    }
}

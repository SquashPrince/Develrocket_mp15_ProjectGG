using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/itemEffect/Heal Item")]
public class HealItemEffect : ItemEffect
{
    [SerializeField] private int _amount;
    public override void Apply(IInteracter interacter)
    {
        // 플레이어 체력 회복
        Debug.Log($"{_amount} 만큼 플레이어 체력 회복");
    }
}

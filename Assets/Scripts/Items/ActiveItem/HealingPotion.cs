using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingPotion : ActiveItem
{
    [SerializeField] private int _healValue;
    [SerializeField] private int _barrierValue;
    [SerializeField] private int _shieldlValue;

    public override void GetItem(IInteracter owner)
    {
        if (!(owner is PlayerContoller)) return;

        if (!CanInteract) return;

        CanInteract = true;

        Healing(owner);

        Destroy(gameObject);
    }

    private void Healing(IInteracter owner)
    {
        // 플레이어 스탯 회복

        if(_healValue != 0)
            Debug.Log($"{_healValue} 만큼 체력 회복");

        if (_barrierValue != 0)
            Debug.Log($"{_barrierValue} 만큼 방어막 회복");

        if (_shieldlValue != 0)
            Debug.Log($"{_shieldlValue} 회 무효화 획득");
    }
}

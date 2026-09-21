using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowAmulet : PassiveItem
{
    [SerializeField] private int _getGoldIncrease;
    [SerializeField] private int _maxHpIncrease;
    [SerializeField] private int _maxBarrierIncrease;
    [SerializeField] private int _damageIncrease;
    [SerializeField] private float _speedIncrease;


    public override void GetItem(IInteracter owner)
    {
        if (!(owner is PlayerContoller)) return;

        if (!CanInteract) return;

        PlayerContoller player = (PlayerContoller)owner;

        CanInteract = false;

        transform.SetParent(player.AmuletTR);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));

        IncreaseStateByAmulet(player);
        
        player.UnlinkItem();
    }

    private void IncreaseStateByAmulet(PlayerContoller player)
    {
        if (_getGoldIncrease != 0)
            Debug.Log($"{_getGoldIncrease} 만큼 골드 획득량이 증가");

        if (_maxHpIncrease != 0)
            Debug.Log($"최대 체력 {_maxHpIncrease} 증가");

        if (_maxBarrierIncrease != 0)
            Debug.Log($"보호막 최대치 {_maxBarrierIncrease} 증가");

        if (_damageIncrease != 0)
            Debug.Log($"공격력 {_damageIncrease} 증가");

        if (_speedIncrease != 0)
            Debug.Log($"이동속도 {_speedIncrease} 증가");
    }
}

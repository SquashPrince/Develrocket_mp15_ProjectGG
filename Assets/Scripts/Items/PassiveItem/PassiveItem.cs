using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveItem : Item, IPassivable
{
    [SerializeField] private AmuletEffect[] _effects;

    public override void GetItem(IInteracter owner)
    {
        if (!(owner is TestPlayerContoller player)) return;

        if (!CanInteract) return;

        CanInteract = false;

        transform.SetParent(player.AmuletTR);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));

        foreach (AmuletEffect effect in _effects)
        {
            effect.Apply(player);
        }
        // 습득 상호작용후 상호작용 타겟 상태 해제
        // Player 구현 쪽에서 리팩토링 후 재선언 필요
        player.UnlinkItem();
    }
}

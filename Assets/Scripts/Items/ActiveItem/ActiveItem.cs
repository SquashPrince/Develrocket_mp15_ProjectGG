using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveItem : Item, IActivable
{
    [SerializeField] private ItemEffect[] _effects;

    public override void GetItem(IInteracter owner)
    {
        if (!(owner is TestPlayerContoller player)) return;

        if (!CanInteract) return;

        CanInteract = false;

        foreach (ItemEffect effect in _effects)
        {
            effect.Apply(player);
        }

        // 일단 회복류 아이템은 삭제로 처리
        Destroy(gameObject);
    }
}

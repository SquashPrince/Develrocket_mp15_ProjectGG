using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseCapsule : ActiveItem
{
    [SerializeField] private float _increaseDmgValue;
    [SerializeField] private float _increaseSpdValue;
    [SerializeField] private int _time;
    [SerializeField] private BuffActor _buffActor;

    public override void GetItem(IInteracter owner)
    {
        if (!(owner is PlayerContoller)) return;

        if (!CanInteract) return;

        PlayerContoller player = (PlayerContoller)owner;

        CanInteract = true;

        GetBuff(player);

        Destroy(gameObject);
    }

    private void GetBuff(PlayerContoller player)
    {
        // 일정 시간 동안 대미지 증가

        BuffActor buff = Instantiate(_buffActor, player.transform);
        buff.StartBuff(player, _increaseDmgValue, _increaseSpdValue, _time);
    }

}

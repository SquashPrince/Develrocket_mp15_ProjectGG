using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/itemEffect/Buff Item")]
public class BuffItemEffct : ItemEffect
{
    [SerializeField] private float _dmgAmount;
    [SerializeField] private float _spdAmount;
    [SerializeField] private float _time;
    [SerializeField] private BuffActor _buffActor;

    public override void Apply(IInteracter interacter)
    {
        if (!(interacter is TestPlayerContoller)) return;

        TestPlayerContoller player = (TestPlayerContoller)interacter;

        // 일정 시간 동안 대미지 증가
        // 아이템은 즉시 사라지고 새로 생성한 버프 오브젝트가 시간 관리
        BuffActor buff = Instantiate(_buffActor, player.transform);
        buff.StartBuff(player, _dmgAmount, _spdAmount, _time);
    }
}

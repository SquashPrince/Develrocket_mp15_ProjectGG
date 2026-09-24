using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BattleRoom : RoomBase
{
    //TODO: 여러 스폰 포인트 중에서 랜덤으로 끌고 오게 해야 함
    [SerializeField] private MonsterSpawner _monsterSpawner;

    //TODO: 바로 깨우는 것이 아니라 입장 시 깨우는 기믹으로 변경해야 함
    private void Start() => _monsterSpawner.ActiveMonster();

    private void Update()
    {
        CheckClear();
    }

    public override void CheckClear()
    {
        _isClear = _monsterSpawner.CheckEliminated();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleRoom : RoomBase
{
    //TODO: 몬스터 아이템 드랍을 확인하고, 리스트에 저장한 다음. 방에 나가면 비활성화 하는 기능 구현 시 사용
    //private List<Item> _dropItems;

    [SerializeField] private List<Monster> _monsterPrefabList;
    [SerializeField] private List<Transform> _monsterSpawnPoints;

    private List<Monster> _currentMonsters;
    private int _currentMonsterCount;
    private bool _isEntered;
    private bool _isEliminated => _currentMonsterCount <= 0;

    private void Update()
    {
        if(_isEntered) OnRunning();
    }

    private void OnDisable()
    {
        OnExit();
    }

    public override void OnEnter()
    {
        base.OnEnter();
        SpawnMonster();
        enabled = true;
    }

    public override void OnRunning()
    {
        base.OnRunning();
        if (!_isClear) CheckClear();
    }

    public override void OnExit()
    {
        base.OnExit();
        enabled = false;
    }

    private void SpawnMonster()
    {
        foreach(Transform point in _monsterSpawnPoints)
        {
            _currentMonsters.Add(Instantiate(_monsterPrefabList[0], point.position, point.rotation));
        }
        _currentMonsterCount = _currentMonsters.Count;
    }

    private void CheckClear()
    {
        if (_currentMonsters.Contains(null))
        {
            _currentMonsterCount -= _currentMonsters.FindAll(null).Count;
            _currentMonsters.RemoveAll(null);
        }

        if(_isEliminated) _isClear = true;
    }
}

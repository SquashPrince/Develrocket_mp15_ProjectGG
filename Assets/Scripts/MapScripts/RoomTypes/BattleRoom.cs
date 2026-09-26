using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using System;

public class BattleRoom : RoomBase
{
    //TODO: 몬스터 아이템 드랍을 확인하고, 리스트에 저장한 다음. 방에 나가면 비활성화 하는 기능 구현 시 사용
    //private List<Item> _dropItems;

    [SerializeField] private List<Monster> _monsterPrefabList;
    [SerializeField] private List<GameObject> _spawnInfoList;
    private Transform[] _monsterSpawnPoints;

    private List<Monster> _currentMonsters = new();
    private int _currentMonsterCount;
    private bool _isEntered;

    private int _randSpawnIndex;
    private bool _isEliminated => _currentMonsterCount <= 0;

    private void Update()
    {
        if(_isEntered) OnRunning();
        if (_isClear) OpenDoor();
    }

    public override void OnEnter()
    {
        base.OnEnter();

        if (_isClear) return;

        Initialize();
        SpawnMonster();
        enabled = true;
        _isEntered = true;
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
        foreach (Transform point in _monsterSpawnPoints)
        {
            if (point == _spawnInfoList[_randSpawnIndex].transform) continue;
            _currentMonsters.Add(Instantiate(_monsterPrefabList[0], point.position, point.rotation));
        }
        _currentMonsterCount = _currentMonsters.Count;
        Debug.Log($"현재 몬스터 수: {_currentMonsterCount}");
    }

    private void CheckClear()
    {
        foreach(Monster monster in _currentMonsters)
        {
            if(monster == null)
            {
                _currentMonsters.Remove(monster);
                _currentMonsterCount--;
                Debug.Log($"현재 몬스터 수: {_currentMonsterCount}");
            }
        }

        if (_isEliminated)
        {
            Debug.Log("모든 몬스터 처치 완료.");
            _isClear = true;
        }
    }

    private void Initialize()
    {
        System.Random rand = new System.Random();
        _randSpawnIndex = rand.Next(0, _spawnInfoList.Count);

        Debug.Log(_spawnInfoList.Count);

        _monsterSpawnPoints = _spawnInfoList[_randSpawnIndex].GetComponentsInChildren<Transform>();
    }
}

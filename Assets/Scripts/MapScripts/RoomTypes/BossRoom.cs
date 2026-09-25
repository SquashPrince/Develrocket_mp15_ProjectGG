using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoom : RoomBase
{
    [SerializeField] private GameObject _bossPrefab;
    [SerializeField] private Transform _bossSpawnPoint;
    private GameObject _bossObject;
    private Monster _currentBoss;
    private bool _isEntered;

    private void Update()
    {
        if (_isEntered) OnRunning();
    }

    private void OnDisable()
    {
        OnExit();
    }

    public override void OnEnter()
    {
        base.OnEnter();
        _isEntered = true;
        SpawnBoss();
    }

    public override void OnRunning()
    {        
        base.OnRunning();
        if (_isClear) return;
        CheckClear();
    }

    public override void OnExit()
    {
        base.OnExit();
        _isEntered = false;
    }

    private void SpawnBoss()
    {
        _bossObject = Instantiate(_bossPrefab, _bossSpawnPoint.position, _bossSpawnPoint.rotation);
        _currentBoss = _bossObject.GetComponentInChildren<Monster>();
    }

    private void CheckClear()
    {
        if(_currentBoss == null)
        {
            _isClear = true;
            CreateStair();
        }
    }

    private void CreateStair()
    {

    }
}

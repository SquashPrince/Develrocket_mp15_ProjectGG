using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureRoom : RoomBase
{
    [SerializeField] private Transform _treasureSpawnPoint;
    [SerializeField] private Item _itemPrefab;
    [SerializeField] private float _rotateSpeed;
    private Item _treasure;
    private bool _isEntered;

    private void FixedUpdate()
    {
        if(_isEntered) RotateItem();
    }

    private void OnDisable()
    {
        OnExit();
    }

    public override void OnEnter()
    {
        base.OnEnter();
        SpawnTreasure();
        _isClear = true;
    }

    public override void OnRunning()
    {
        base.OnRunning();
    }

    public override void OnExit()
    {
        base.OnExit();
        
        if(_treasure != null) _treasure.gameObject.SetActive(false);
    }

    /// <summary>
    /// 아이템 방에 아이템이 존재하지 않으면 소환하고, 그렇지 않으면 비활성화 하는 메서드
    /// </summary>
    private void SpawnTreasure()
    {
        if(_treasure != null)
        {
            _treasure.gameObject.SetActive(true);
            return;
        }

        _treasure  = Instantiate(_itemPrefab, _treasureSpawnPoint.position, _treasureSpawnPoint.rotation);
    }

    private void RotateItem()
    {
       if(_treasure != null)
        {
            _treasure.transform.Rotate(Vector3.up * _rotateSpeed * Time.deltaTime);
        } 
    }
}

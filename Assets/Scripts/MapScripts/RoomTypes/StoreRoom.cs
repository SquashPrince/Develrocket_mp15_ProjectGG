using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreRoom : RoomBase
{
    [SerializeField] List<Item> _itemPrefabList;
    [SerializeField] Transform[] _itemSpawnPoints;
    public override void OnEnter()
    {
        base.OnEnter();
        _isClear = true;
    }

    public override void OnRunning()
    {
        base.OnRunning();
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    private void SpawnItems()
    {
        for(int i = 0; i < _itemSpawnPoints.Length; i++)
        {

        }
    }
}

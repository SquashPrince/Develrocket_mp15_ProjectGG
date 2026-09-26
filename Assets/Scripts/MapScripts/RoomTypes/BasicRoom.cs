using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRoom : RoomBase
{
    [SerializeField] private Item _startWeaponPrefab;
    [SerializeField] private Transform _weaponSpawnPoint;

    private bool _isEntered = false;
    public override void OnEnter()
    {
        base.OnEnter();

        if(RoomType == RoomType.START && !_isEntered)
        {
            SpawnStartWeapon();
        }
        
        if(RoomType == RoomType.BASIC)
        {
            _weaponSpawnPoint.gameObject.SetActive(false);
        }

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

    private void SpawnStartWeapon()
    {
        Instantiate(_startWeaponPrefab, _weaponSpawnPoint.position, _weaponSpawnPoint.rotation);
        _isEntered = true;
    }
}

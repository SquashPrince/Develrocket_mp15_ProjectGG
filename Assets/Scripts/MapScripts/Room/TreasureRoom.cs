using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureRoom : RoomBase
{
    [SerializeField] private Weapon _treasure;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private float _rotateSpeed;
    private void Awake() => CheckClear();
    private void Start() => SpawnWeapon();
    private void Update() => RotateTreasure();
    public override void CheckClear()
    {
        _isClear = true;
    }

    private void SpawnWeapon()
    {
        Weapon treasure = Instantiate(_treasure, _spawnPoint.position, _spawnPoint.rotation);
    }

    private void RotateTreasure()
    {
        _spawnPoint.Rotate(Vector3.up * _rotateSpeed * Time.deltaTime);
    }
}

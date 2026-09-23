using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Player;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    /*private static PlayerInputManager PlayerInput => PlayerInputManager.Instance;
    private PlayerValues _playerValues;
    private PlayerWeaponEnum _currentSlot;
    private PlayerWeaponEnum firstSlot = PlayerWeaponEnum.First;
    private PlayerWeaponEnum secondSlot = PlayerWeaponEnum.Second;
    private PlayerWeaponEnum thirdSlot = PlayerWeaponEnum.Third;
    
    
    private Dictionary<PlayerWeaponEnum, Weapon> _weaponDictionary = new();
    
    // ========================================


    // ========================================
    
    public Weapon EquippedWeapon
    {
        get => _weaponDictionary[_currentSlot];
    }

    // TODO: PlayerValues에 기본값 1f인 공격력 배율을 구현. 무기 교체 시 유지.
    // 예시) 공격력 효과 5는 배율에 0.05f를 더함.
    public float DamageMultiplier
    { get => DamageMultiplier; set => DamageMultiplier = value/100; }
    

    private PlayerWeaponEnum WhatNextSlot()
    {
        if (_weaponDictionary.Count == 0) return firstSlot;
        int nextSlot = (int)_currentSlot%_weaponDictionary.Count+1;
        return (PlayerWeaponEnum)nextSlot;
    }

    // 현재 슬롯에 장착된 무기를 해제하고 
    public void ChangeToNextWeapon()
    {
        _weaponDictionary[_currentSlot].SetUnEquip();
        _weaponDictionary[WhatNextSlot()].SetEquip(Tran);
        _currentSlot = WhatNextSlot();
    }

    // 빈 슬롯이 있으면 True 없으면 False
    public bool IsAnyEmptySlot()
    {
        return _weaponDictionary.Count < 3;
    }

    /// <summary>
    /// 이미 소유한 무기면 true, 아니면 false
    /// </summary>
    public bool TrySetWeapon(Weapon weapon)
    {
        return _weaponDictionary.ContainsValue(weapon);
    }

    /// <summary>
    /// 빈 슬롯이 있으면 무기 장착
    /// </summary>
    public void GetNewWeapon(Weapon weapon)
    {
        if (!_weaponDictionary.ContainsKey(firstSlot))
            SetWeaponToSlot(firstSlot, weapon);
        else if (!_weaponDictionary.ContainsKey(secondSlot))
            SetWeaponToSlot(secondSlot, weapon);
        else if (!_weaponDictionary.ContainsKey(thirdSlot))
            SetWeaponToSlot(thirdSlot, weapon);
    }

    private void SetWeaponToSlot(PlayerWeaponEnum slot, Weapon weapon)
    {
        _weaponDictionary.Add(slot,weapon);
    }

    public void DropWeapon()
    {
        _weaponDictionary[_currentSlot].SetUnEquip();
        _weaponDictionary.Remove(_currentSlot);
    }
    
    
    // TODO: PlayerValues에 기본값 1f인 공격력 배율을 구현. 무기 교체 시 유지.
    // 예시) 공격력 효과 5는 배율에 0.05f를 더함.
    // float DamageMultiplier { get; set; }


    public int Hp { get => Hp; set => Hp = value; }

    public int BaseShield { get => BaseShield; set => BaseShield = value; }
    public int BaseMaxHp { get =>  BaseMaxHp; set => BaseMaxHp = value; }

    // TODO: PlayerValues에 최대 방어막 수 BaseMaxShield 프로퍼티 구현 필요.
    public int BaseMaxShield
    { get => BaseMaxShield; set =>  BaseMaxShield = value; }

    
    // ========================================
    */
   
    
}

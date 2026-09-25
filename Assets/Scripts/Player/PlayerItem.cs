using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class PlayerItemSlot : MonoBehaviour
{
    private static PlayerInputManager PlayerInput => PlayerInputManager.Instance;
    private PlayerValues _playerValues;
    private const int MaxItems = 3;
    private int _primaryCount;
    private int _secondaryCount;

    [SerializeField]private Item _primaryItem;
    [SerializeField]private Item _secondaryItem;
    [SerializeField]private Item _singleUseItem;
    private void Awake() => _playerValues = GetComponent<PlayerValues>();


    private void OnEnable()
    {
        PlayerInput.OnItem1 += UsePrimary;
        PlayerInput.OnItem2 += UseSecondary;
        PlayerInput.OnItem3 += UseThird;
    }

    private void OnDisable()
    {
       PlayerInput.OnItem1 -= UsePrimary;
       PlayerInput.OnItem2 -= UseSecondary;
       PlayerInput.OnItem3 -= UseThird;
    }
    
    // 인풋 등록
    // ========================================
    
    private void UsePrimary()
    {
        if (_primaryCount <= 0) return;
        _primaryCount--;
        _primaryItem.Use(_playerValues);
    }
    
    private void UseSecondary()
    {
        if (_secondaryCount <= 0) return;
        _secondaryCount--;
        _secondaryItem.Use(_playerValues);
    }

    private void UseThird()
    {
        if (_singleUseItem == null) return;
        _singleUseItem.Use(_playerValues);
        _singleUseItem = null;
    }
    
    // 1,2번 고정 아이템
    // ========================================
    
    public bool CanInteract(PlayerItemEnum slot)
    {
        switch (slot)
        {
            case PlayerItemEnum.Primary:
                return _primaryCount >= MaxItems;
            case PlayerItemEnum.Secondary:
                return _secondaryCount >= MaxItems;
            case PlayerItemEnum.SingleUse:
                return _singleUseItem == null;
            default:
                return false;
        }
    }

    public void SetItem(Item item, PlayerItemEnum slot)
    {
        switch (slot)
        {
            case PlayerItemEnum.Primary:
                _primaryItem = item;
                break;
            case PlayerItemEnum.Secondary:
                _secondaryItem = item;
                break;
            case PlayerItemEnum.SingleUse:
                _singleUseItem = item;
                break;
        }
    }
    
    // 3번 아이템
    // ========================================

    
    
}

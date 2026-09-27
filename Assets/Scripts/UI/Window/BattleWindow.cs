using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleWindow : MonoBehaviour, IWindowable
{
    [SerializeField] private Profile _proFile;

    [Header("던전 재화 (골드)")]
    [SerializeField] private CurrencySlot _gold;
    [SerializeField] private WeaponInven _weaponInven;
    [SerializeField] private ItemSlot[] _itemSlot;
    public ItemSlot[] ItemSlots => _itemSlot;

    public void SetActive()
    {
        _proFile.SetData();
        _gold.SetData();
        _weaponInven.SetData();

        for(int i = 0; i < _itemSlot.Length; i++)
        {
            _itemSlot[i].SetData(GameManager.Instance.PlayerValues.ItemSlots.GetSlotState((PlayerItemEnum)i));
        }

        SetUI();
    }

    private void SetUI()
    {
    }

    public void UseItem(PlayerItemEnum itemEnum)
    {
        _itemSlot[(int)itemEnum].UseItem();
    }

    public void TakeWeapon()
    {
        _weaponInven.TakeWeapon();
    }

    public void WeaponSwap(bool isLeft)
    {
        _weaponInven.WeaponSwap(isLeft);
    }

    public void ResetUI()
    {
        for (int i = 0; i < _itemSlot.Length; i++)
        {
            _itemSlot[i].SetData(GameManager.Instance.PlayerValues.ItemSlots.GetSlotState((PlayerItemEnum)i));
        }
    }

    /// <summary> 던전 나가기 버튼 </summary>
    public void DengonExit()
    {
        GameManager.Instance.SetPaused(true);
    }

    public void BackBtn()
    {
    }
}
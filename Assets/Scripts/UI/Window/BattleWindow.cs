using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleWindow : MonoBehaviour, IWindowable
{
    [SerializeField] private Profile _proFile;
    [SerializeField] private WeaponInven _weaponInven;
    [SerializeField] private ItemSlot[] _itemSlot;
    public ItemSlot[] ItemSlots => _itemSlot;

    public void SetActive()
    {
        _proFile.SetData();
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

    public void WeaponSwap(bool isLeft)
    {
        _weaponInven.WeaponSwap(isLeft);
    }

    public void ResetUI()
    {
    }

    /// <summary> 던전 나가기 버튼 </summary>
    public void DengonExit()
    {
        UIManager.Instance.PopUp.Open(EPopUpType.BattleExit);
    }

    public void BackBtn()
    {
    }
}
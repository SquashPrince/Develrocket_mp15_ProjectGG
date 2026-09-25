using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleWindow : MonoBehaviour, IWindowable
{
    [SerializeField] private Profile _proFile;
    [SerializeField] private WeaponInven _weaponInven;

    public void SetActive()
    {
        _proFile.SetData();
        _weaponInven.SetData();
        SetUI();
    }

    private void SetUI()
    {
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

    private void Update()
    {
        _weaponInven.WeaponSwap();
    } 
}
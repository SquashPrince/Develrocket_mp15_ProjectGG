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

    public void BackBtn()
    {
    }

    private void Update()
    {
        _weaponInven.WeaponSwap();
    } 
}
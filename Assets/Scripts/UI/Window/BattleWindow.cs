using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleWindow : MonoBehaviour, IWindowable
{
    [SerializeField] private WeaponInven weaponInven;

    public void SetActive()
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
        weaponInven.WeaponSwap();
    }
}
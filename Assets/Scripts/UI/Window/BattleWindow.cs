using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleWindow : MonoBehaviour
{
    [SerializeField] private WeaponInven weaponInven;

    private void Update()
    {
        weaponInven.WeaponSwap();
    }
}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Build;
using UnityEngine;

public class WeaponInven : MonoBehaviour
{
    private Animator Ani;

    [SerializeField] private WeaponSlot[] _weaponSlot;
    [SerializeField] private TextMeshProUGUI _bulletCurMagazine;
    [SerializeField] private TextMeshProUGUI _bulletMaxMagazine;

    private bool _isLeft;

    private void Awake()
    {
        CacheComponents();
    }

    public void SetData()
    {/*
        if (GameManager.Instance.PlayerValues.WeaponDictionary.Count == 0) return;

        for (int i = 0; i < _weaponSlot.Length; i++)
        {
            for (int j = 0; j < _weaponSlot[i]._weapons.Length; j++)
            {
                GameManager.Instance.PlayerValues.WeaponDictionary.TryGetValue((PlayerWeaponEnum)j, out _weaponSlot[i]._weapons[j]);
            }
        }*/

        _bulletMaxMagazine.text = $"{GameManager.Instance.PlayerValues.MaxMagazine}";
        WeaponMagazine();
    }

    public void WeaponMagazine(bool isView = true)
    {
        _bulletCurMagazine.text = isView ? $"{GameManager.Instance.PlayerValues.CurrentMagazine}" : "-";
    }

    public void TakeWeapon()
    {/*
        if (GameManager.Instance.PlayerValues.WeaponDictionary.Count == 0) return;

        for (int i = 0; i < _weaponSlot.Length; i++)
        {
            for (int j = 0; j < _weaponSlot[i]._weapons.Length; j++)
            {
                GameManager.Instance.PlayerValues.WeaponDictionary.TryGetValue((PlayerWeaponEnum)j, out _weaponSlot[i]._weapons[j]);
            }
        }*/
    }

    public void WeaponSwap(bool isLeft)
    {
        _isLeft = isLeft;
        string aniStr = _isLeft ? "Left" : "Right";
        Ani.SetTrigger(aniStr);
        _bulletMaxMagazine.text = "-";
        WeaponMagazine(false);
    }

    public void SwapStop()
    {
        for (int i = 0; i < _weaponSlot.Length; i++)
        {
            _weaponSlot[i].Swap(_isLeft);
        }

        RefreshWeaponInven();
    }

    public void RefreshWeaponInven()
    {
        SetData();
    }

    private void CacheComponents()
    {
        Ani = GetComponent<Animator>();
    }
}
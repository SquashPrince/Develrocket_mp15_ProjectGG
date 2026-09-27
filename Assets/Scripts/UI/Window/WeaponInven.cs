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

    private void Awake()
    {
        CacheComponents();
    }

    public void SetData()
    {
        for (int i = 0; i < _weaponSlot.Length; i++)
        {
            _weaponSlot[i].SetData();
        }

        _bulletMaxMagazine.text = $"{GameManager.Instance.PlayerValues.MaxMagazine}";
        WeaponMagazine();
    }

    public void WeaponMagazine(bool isView = true)
    {
        _bulletCurMagazine.text = isView ? $"{GameManager.Instance.PlayerValues.CurrentMagazine}" : "-";
    }

    public void TakeWeapon()
    {
        for (int i = 0; i < _weaponSlot.Length; i++)
        {
            _weaponSlot[i].TakeWeapon();
        }
    }

    public void WeaponSwap(bool isLeft)
    {
        //string aniStr = _isLeft ? "Right" : "Left";
        //Ani.SetTrigger(aniStr);
        for (int i = 0; i < _weaponSlot.Length; i++)
        {
            _weaponSlot[i].Swap(isLeft);
        }
        _bulletMaxMagazine.text = "-";
        WeaponMagazine(false);
    }

    public void SwapStop()
    {
        /*
        for (int i = 0; i < _weaponSlot.Length; i++)
        {
            _weaponSlot[i].Swap();
        }*/
    }
    private void CacheComponents()
    {
        Ani = GetComponent<Animator>();
    }
}
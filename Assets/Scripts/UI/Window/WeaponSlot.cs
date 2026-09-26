using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSlot : MonoBehaviour
{
    public Weapon _weapon;

    public Weapon[] _weapons;
    [SerializeField] private Image[] _weaponImg;

    public void Swap(bool isLeft)
    {
        if(isLeft)
        {
            LeftSwap();
        }
        else
        {
            RightSwap();
        }
    }

    private void LeftSwap()
    {
        Weapon weapon = _weapons[0];

        _weapons[0] = _weapons[1];
        _weapons[1] = _weapons[2];
        _weapons[2] = weapon;

        for(int i = 0; i < _weaponImg.Length; i++)
        {
            // 이미지 띄우기
            _weaponImg[i].sprite = _weapons[i].Icon;
        }
    }

    private void RightSwap()
    {
        Weapon weapon = _weapons[2];

        _weapons[2] = _weapons[1];
        _weapons[1] = _weapons[0];
        _weapons[0] = weapon;

        for (int i = 0; i < _weaponImg.Length; i++)
        {
            // 이미지 띄우기
            _weaponImg[i].sprite = _weapons[i].Icon;
        }
    }
}
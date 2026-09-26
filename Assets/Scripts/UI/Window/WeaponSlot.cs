using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSlot : MonoBehaviour
{
    public Weapon _weapon;

    private Weapon[] _weapons = new Weapon[3];
    [SerializeField] private Image[] _weaponImg;
    [SerializeField] private RectTransform[] rt;

    public void SetData()
    {
        for (int i = 0; i < _weapons.Length; i++)
        {
            //GameManager.Instance.PlayerValues.WeaponDictionary.TryGetValue((PlayerWeaponEnum)j, out _weapons[i]);
        }

        SetRectSize();
    }

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
        Weapon weapon = _weapons[2];

        _weapons[2] = _weapons[1];
        _weapons[1] = _weapons[0];
        _weapons[0] = weapon;

        for (int i = 0; i < _weaponImg.Length; i++)
        {
            // 이미지 띄우기
            _weaponImg[i].sprite = _weapons[i].Icon;
        }

        _weapon = _weapons[1];

        SetRectSize();
    }

    private void RightSwap()
    {
        Weapon weapon = _weapons[0];

        _weapons[0] = _weapons[1];
        _weapons[1] = _weapons[2];
        _weapons[2] = weapon;

        for (int i = 0; i < _weaponImg.Length; i++)
        {
            // 이미지 띄우기
            _weaponImg[i].sprite = _weapons[i].Icon;
        }

        _weapon = _weapons[1];

        SetRectSize();
    }

    private void SetRectSize()
    {
        for(int i = 0; i < _weapons.Length; i++)
        {
            if (_weapons[i] == null) continue;

            switch (_weapons[i].EWeaponType)
            {
                case EWeaponType.AutoPistol:
                    rt[i].localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    break;
                case EWeaponType.Canon:
                    rt[i].localScale = new Vector3(1f, 1f, 1f);
                    break;
                case EWeaponType.Revolver:
                    rt[i].localScale = new Vector3(0.6f, 0.6f, 0.6f);
                    break;
                case EWeaponType.Shotgun:
                    rt[i].localScale = new Vector3(1f, 1f, 1f);
                    break;
                case EWeaponType.SubmachineGun:
                    rt[i].localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    break;
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSlot : MonoBehaviour
{
    [SerializeField] private int _slotNum;
    public Weapon _weapon;

    private Dictionary<PlayerWeaponEnum, Weapon> _dicWeapon = new();
    private Weapon[] _weapons = new Weapon[3];
    [SerializeField] private Image[] _weaponImg;
    [SerializeField] private RectTransform[] rt;

    public void SetData()
    {
        _dicWeapon = GameManager.Instance.PlayerValues.WeaponDictionary;

        if (_dicWeapon.Count == 0)
        {
            for(int i = 0; i < _weaponImg.Length; i++)
            {
                _weaponImg[i].gameObject.SetActive(false);
            }
        }
        else
        {
            _weapons[0] = _dicWeapon[0];

            SetWeaponLocation(PlayerWeaponEnum.First, _weapons[0]);
            SetRectSize();
        }
    }

    public void TakeWeapon(PlayerWeaponEnum weaponEnum, Weapon weapon)
    {
        _dicWeapon = GameManager.Instance.PlayerValues.WeaponDictionary;

        for (int i = 0; i < _weaponImg.Length; i++)
        {
            _weaponImg[i].gameObject.SetActive(true);
        }

        SetWeaponLocation(weaponEnum, weapon);
    }

    private void SetWeaponLocation(PlayerWeaponEnum weaponEnum, Weapon weapon)
    {
        switch (_slotNum)
        {
            // 왼쪽 무기 슬롯
            case 0:
                // 왼쪽 무기
                // 무기가 1개라면 현재 장착 무기를 보여준다.
                if (_dicWeapon.Count == 1)
                {
                    _weapons[0] = _dicWeapon[PlayerWeaponEnum.First];
                    _weapons[1] = _dicWeapon[PlayerWeaponEnum.First];
                    _weapons[2] = _dicWeapon[PlayerWeaponEnum.First];
                }
                // 무기가 2개라면 양쪽에 장착 제외 나머지 무기를 보여준다.
                else if (_dicWeapon.Count == 2)
                {
                    // 현재 장착 무기가 첫번쨰 무기라면
                    if (weaponEnum == PlayerWeaponEnum.First)
                    {
                        // 두번째 무기를 양옆에 보여준다.
                        _weapons[0] = _dicWeapon[PlayerWeaponEnum.First];
                        _weapons[1] = _dicWeapon[PlayerWeaponEnum.Second];
                        _weapons[2] = _dicWeapon[PlayerWeaponEnum.First];
                    }
                    else if (weaponEnum == PlayerWeaponEnum.Second)
                    {
                        // 첫번째 무기를 양옆에 보여준다.
                        _weapons[0] = _dicWeapon[PlayerWeaponEnum.Second];
                        _weapons[1] = _dicWeapon[PlayerWeaponEnum.First];
                        _weapons[2] = _dicWeapon[PlayerWeaponEnum.Second];
                    }
                }
                // 무기가 3개 다 있다면.
                else if (_dicWeapon.Count == 3)
                {
                    if (weaponEnum == PlayerWeaponEnum.First)
                    {
                        _weapons[0] = _dicWeapon[PlayerWeaponEnum.Third];
                        _weapons[1] = _dicWeapon[PlayerWeaponEnum.Second];
                        _weapons[2] = _dicWeapon[PlayerWeaponEnum.First];

                    }
                    else if (weaponEnum == PlayerWeaponEnum.Second)
                    {
                        _weapons[0] = _dicWeapon[PlayerWeaponEnum.First];
                        _weapons[1] = _dicWeapon[PlayerWeaponEnum.Third];
                        _weapons[2] = _dicWeapon[PlayerWeaponEnum.Second];
                    }
                    else if (weaponEnum == PlayerWeaponEnum.Third)
                    {
                        _weapons[0] = _dicWeapon[PlayerWeaponEnum.Second];
                        _weapons[1] = _dicWeapon[PlayerWeaponEnum.First];
                        _weapons[2] = _dicWeapon[PlayerWeaponEnum.Third];
                    }
                }
                break;
            // 가운데 무기 슬롯
            case 1:
                // 현재 장착 무기
                _weapons[1] = GameManager.Instance.PlayerValues.EquippedWeapon;

                // 왼쪽 무기
                // 무기가 1개라면 현재 장착 무기를 보여준다.
                if (_dicWeapon.Count == 1)
                {
                    _weapons[0] = _dicWeapon[PlayerWeaponEnum.First];
                    _weapons[2] = _dicWeapon[PlayerWeaponEnum.First];
                }
                // 무기가 2개라면 양쪽에 장착 제외 나머지 무기를 보여준다.
                else if (_dicWeapon.Count == 2)
                {
                    // 현재 장착 무기가 첫번쨰 무기라면
                    if(weaponEnum == PlayerWeaponEnum.First)
                    {
                        // 두번째 무기를 양옆에 보여준다.
                        _weapons[0] = _dicWeapon[PlayerWeaponEnum.Second];
                        _weapons[2] = _dicWeapon[PlayerWeaponEnum.Second];
                    }
                    else if (weaponEnum == PlayerWeaponEnum.Second)
                    {
                        // 첫번째 무기를 양옆에 보여준다.
                        _weapons[0] = _dicWeapon[PlayerWeaponEnum.First];
                        _weapons[2] = _dicWeapon[PlayerWeaponEnum.First];
                    }
                }
                // 무기가 3개 다 있다면.
                else if (_dicWeapon.Count == 3)
                {
                    if (weaponEnum == PlayerWeaponEnum.First)
                    {
                        // 왼쪽 무기는 2번째 무기
                        _weapons[0] = _dicWeapon[PlayerWeaponEnum.Second];
                        // 오른쪽 무기는 3번쨰 무기
                        _weapons[2] = _dicWeapon[PlayerWeaponEnum.Third];

                    }
                    else if (weaponEnum == PlayerWeaponEnum.Second)
                    {
                        // 왼쪽 무기는 3번째 무기
                        _weapons[0] = _dicWeapon[PlayerWeaponEnum.Third];
                        // 오른쪽 무기는 1번째 무기
                        _weapons[2] = _dicWeapon[PlayerWeaponEnum.First];
                    }
                    else if (weaponEnum == PlayerWeaponEnum.Third)
                    {
                        // 왼쪽 무기는 1번째 무기
                        _weapons[0] = _dicWeapon[PlayerWeaponEnum.First];
                        // 오른쪽 무기는 2번째 무기
                        _weapons[2] = _dicWeapon[PlayerWeaponEnum.Second];
                    }
                }
                break;
            // 오른쪽 무기 슬롯
            case 2:
                // 오른쪽 무기
                // 무기가 1개라면 현재 장착 무기를 보여준다.
                if (_dicWeapon.Count == 1)
                {
                    _weapons[0] = _dicWeapon[PlayerWeaponEnum.First];
                    _weapons[1] = _dicWeapon[PlayerWeaponEnum.First];
                    _weapons[2] = _dicWeapon[PlayerWeaponEnum.First];
                }
                // 무기가 2개라면 양쪽에 장착 제외 나머지 무기를 보여준다.
                else if (_dicWeapon.Count == 2)
                {
                    // 현재 장착 무기가 첫번쨰 무기라면
                    if (weaponEnum == PlayerWeaponEnum.First)
                    {
                        // 두번째 무기를 양옆에 보여준다.
                        _weapons[0] = _dicWeapon[PlayerWeaponEnum.First];
                        _weapons[1] = _dicWeapon[PlayerWeaponEnum.Second];
                        _weapons[2] = _dicWeapon[PlayerWeaponEnum.First];
                    }
                    else if (weaponEnum == PlayerWeaponEnum.Second)
                    {
                        // 첫번째 무기를 양옆에 보여준다.
                        _weapons[0] = _dicWeapon[PlayerWeaponEnum.Second];
                        _weapons[1] = _dicWeapon[PlayerWeaponEnum.First];
                        _weapons[2] = _dicWeapon[PlayerWeaponEnum.Second];
                    }
                }
                // 무기가 3개 다 있다면.
                else if (_dicWeapon.Count == 3)
                {
                    if (weaponEnum == PlayerWeaponEnum.First)
                    {
                        _weapons[0] = _dicWeapon[PlayerWeaponEnum.First];
                        _weapons[1] = _dicWeapon[PlayerWeaponEnum.Third];
                        _weapons[2] = _dicWeapon[PlayerWeaponEnum.Second];

                    }
                    else if (weaponEnum == PlayerWeaponEnum.Second)
                    {
                        _weapons[0] = _dicWeapon[PlayerWeaponEnum.Second];
                        _weapons[1] = _dicWeapon[PlayerWeaponEnum.First];
                        _weapons[2] = _dicWeapon[PlayerWeaponEnum.Third];
                    }
                    else if (weaponEnum == PlayerWeaponEnum.Third)
                    {
                        _weapons[0] = _dicWeapon[PlayerWeaponEnum.Third];
                        _weapons[1] = _dicWeapon[PlayerWeaponEnum.Second];
                        _weapons[2] = _dicWeapon[PlayerWeaponEnum.First];
                    }
                }
                break;
        }

        SetRectSize();
        WeaponView();
    }

    public void Swap(bool isLeft)
    {
        if (isLeft)
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

    }

    private void RightSwap()
    {
        Weapon weapon = _weapons[0];

        _weapons[0] = _weapons[1];
        _weapons[1] = _weapons[2];
        _weapons[2] = weapon;

        _weapon = _weapons[1];

    }

    private void WeaponView()
    {
        for (int i = 0; i < _weaponImg.Length; i++)
        {
            // 이미지 띄우기
            _weaponImg[i].sprite = _weapons[i].Icon;
        }
    }

    private void SetRectSize()
    {
        for (int i = 0; i < _weapons.Length; i++)
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
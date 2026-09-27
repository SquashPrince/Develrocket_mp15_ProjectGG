using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSlot : MonoBehaviour
{
    private RectTransform _rt;

    [SerializeField] private int _slotNum;
    public Weapon _weapon;

    private Weapon[] _weapons = new Weapon[3];
    [SerializeField] private Image[] _weaponImg;
    [SerializeField] private RectTransform[] rt;

    private void Awake() => CacheComponents();

    private void CacheComponents()
    {
        _rt = GetComponent<RectTransform>();
    }

    public void SetData()
    {
        Dictionary<PlayerWeaponEnum, Weapon> dicWeapon = GameManager.Instance.PlayerValues.WeaponDictionary;

        if (dicWeapon.Count == 0)
        {
            for(int i = 0; i < _weaponImg.Length; i++)
            {
                _weaponImg[i].gameObject.SetActive(false);
            }
        }
        else
        {
            _weapons[0] = dicWeapon[0];

            SetWeaponLocation();
            SetRectSize();
        }
    }

    public void TakeWeapon()
    {
        for (int i = 0; i < _weaponImg.Length; i++)
        {
            _weaponImg[i].gameObject.SetActive(true);
        }

        SetWeaponLocation();
    }

    private void SetWeaponLocation()
    {
        Dictionary<PlayerWeaponEnum, Weapon> dicWeapon = GameManager.Instance.PlayerValues.WeaponDictionary;
        PlayerWeaponEnum curWeapon = GameManager.Instance.PlayerValues.CurrentSlot;

        switch (_slotNum)
        {
            // 왼쪽 무기 슬롯
            case 0:
                // 왼쪽 무기
                // 무기가 1개라면 현재 장착 무기를 보여준다.
                if (dicWeapon.Count == 1)
                {
                    _weapons[0] = dicWeapon[curWeapon];
                    _weapons[1] = dicWeapon[curWeapon];
                    _weapons[2] = dicWeapon[curWeapon];
                }
                // 무기가 2개라면 양쪽에 장착 제외 나머지 무기를 보여준다.
                else if (dicWeapon.Count == 2)
                {
                    // 현재 장착 무기가 첫번쨰 무기라면
                    if (curWeapon == PlayerWeaponEnum.First)
                    {
                        // 두번째 무기를 양옆에 보여준다.
                        _weapons[0] = dicWeapon[PlayerWeaponEnum.First];
                        _weapons[1] = dicWeapon[PlayerWeaponEnum.Second];
                        _weapons[2] = dicWeapon[PlayerWeaponEnum.First];
                    }
                    else if (curWeapon == PlayerWeaponEnum.Second)
                    {
                        // 첫번째 무기를 양옆에 보여준다.
                        _weapons[0] = dicWeapon[PlayerWeaponEnum.Second];
                        _weapons[1] = dicWeapon[PlayerWeaponEnum.First];
                        _weapons[2] = dicWeapon[PlayerWeaponEnum.Second];
                    }
                }
                // 무기가 3개 다 있다면.
                else if (dicWeapon.Count == 3)
                {
                    if (curWeapon == PlayerWeaponEnum.First)
                    {
                        _weapons[0] = dicWeapon[PlayerWeaponEnum.Third];
                        _weapons[1] = dicWeapon[PlayerWeaponEnum.Second];
                        _weapons[2] = dicWeapon[PlayerWeaponEnum.First];

                    }
                    else if (curWeapon == PlayerWeaponEnum.Second)
                    {
                        _weapons[0] = dicWeapon[PlayerWeaponEnum.First];
                        _weapons[1] = dicWeapon[PlayerWeaponEnum.Third];
                        _weapons[2] = dicWeapon[PlayerWeaponEnum.Second];
                    }
                    else if (curWeapon == PlayerWeaponEnum.Third)
                    {
                        _weapons[0] = dicWeapon[PlayerWeaponEnum.Second];
                        _weapons[1] = dicWeapon[PlayerWeaponEnum.First];
                        _weapons[2] = dicWeapon[PlayerWeaponEnum.Third];
                    }
                }
                break;
            // 가운데 무기 슬롯
            case 1:
                // 현재 장착 무기
                _weapons[1] = dicWeapon[curWeapon];

                // 왼쪽 무기
                // 무기가 1개라면 현재 장착 무기를 보여준다.
                if (dicWeapon.Count == 1)
                {
                    _weapons[0] = dicWeapon[PlayerWeaponEnum.First];
                    _weapons[2] = dicWeapon[PlayerWeaponEnum.First];
                }
                // 무기가 2개라면 양쪽에 장착 제외 나머지 무기를 보여준다.
                else if (dicWeapon.Count == 2)
                {
                    // 현재 장착 무기가 첫번쨰 무기라면
                    if(curWeapon == PlayerWeaponEnum.First)
                    {
                        // 두번째 무기를 양옆에 보여준다.
                        _weapons[0] = dicWeapon[PlayerWeaponEnum.Second];
                        _weapons[2] = dicWeapon[PlayerWeaponEnum.Second];
                    }
                    else if (curWeapon == PlayerWeaponEnum.Second)
                    {
                        // 첫번째 무기를 양옆에 보여준다.
                        _weapons[0] = dicWeapon[PlayerWeaponEnum.First];
                        _weapons[2] = dicWeapon[PlayerWeaponEnum.First];
                    }
                }
                // 무기가 3개 다 있다면.
                else if (dicWeapon.Count == 3)
                {
                    if (curWeapon == PlayerWeaponEnum.First)
                    {
                        // 왼쪽 무기는 2번째 무기
                        _weapons[0] = dicWeapon[PlayerWeaponEnum.Second];
                        // 오른쪽 무기는 3번쨰 무기
                        _weapons[2] = dicWeapon[PlayerWeaponEnum.Third];

                    }
                    else if (curWeapon == PlayerWeaponEnum.Second)
                    {
                        // 왼쪽 무기는 3번째 무기
                        _weapons[0] = dicWeapon[PlayerWeaponEnum.Third];
                        // 오른쪽 무기는 1번째 무기
                        _weapons[2] = dicWeapon[PlayerWeaponEnum.First];
                    }
                    else if (curWeapon == PlayerWeaponEnum.Third)
                    {
                        // 왼쪽 무기는 1번째 무기
                        _weapons[0] = dicWeapon[PlayerWeaponEnum.First];
                        // 오른쪽 무기는 2번째 무기
                        _weapons[2] = dicWeapon[PlayerWeaponEnum.Second];
                    }
                }
                break;
            // 오른쪽 무기 슬롯
            case 2:
                // 오른쪽 무기
                // 무기가 1개라면 현재 장착 무기를 보여준다.
                if (dicWeapon.Count == 1)
                {
                    _weapons[0] = dicWeapon[PlayerWeaponEnum.First];
                    _weapons[1] = dicWeapon[PlayerWeaponEnum.First];
                    _weapons[2] = dicWeapon[PlayerWeaponEnum.First];
                }
                // 무기가 2개라면 양쪽에 장착 제외 나머지 무기를 보여준다.
                else if (dicWeapon.Count == 2)
                {
                    // 현재 장착 무기가 첫번쨰 무기라면
                    if (curWeapon == PlayerWeaponEnum.First)
                    {
                        // 두번째 무기를 양옆에 보여준다.
                        _weapons[0] = dicWeapon[PlayerWeaponEnum.First];
                        _weapons[1] = dicWeapon[PlayerWeaponEnum.Second];
                        _weapons[2] = dicWeapon[PlayerWeaponEnum.First];
                    }
                    else if (curWeapon == PlayerWeaponEnum.Second)
                    {
                        // 첫번째 무기를 양옆에 보여준다.
                        _weapons[0] = dicWeapon[PlayerWeaponEnum.Second];
                        _weapons[1] = dicWeapon[PlayerWeaponEnum.First];
                        _weapons[2] = dicWeapon[PlayerWeaponEnum.Second];
                    }
                }
                // 무기가 3개 다 있다면.
                else if (dicWeapon.Count == 3)
                {
                    if (curWeapon == PlayerWeaponEnum.First)
                    {
                        _weapons[0] = dicWeapon[PlayerWeaponEnum.First];
                        _weapons[1] = dicWeapon[PlayerWeaponEnum.Third];
                        _weapons[2] = dicWeapon[PlayerWeaponEnum.Second];

                    }
                    else if (curWeapon == PlayerWeaponEnum.Second)
                    {
                        _weapons[0] = dicWeapon[PlayerWeaponEnum.Second];
                        _weapons[1] = dicWeapon[PlayerWeaponEnum.First];
                        _weapons[2] = dicWeapon[PlayerWeaponEnum.Third];
                    }
                    else if (curWeapon == PlayerWeaponEnum.Third)
                    {
                        _weapons[0] = dicWeapon[PlayerWeaponEnum.Third];
                        _weapons[1] = dicWeapon[PlayerWeaponEnum.Second];
                        _weapons[2] = dicWeapon[PlayerWeaponEnum.First];
                    }
                }
                break;
        }

        SetRectSize();
        WeaponView();
    }

    public void Swap(bool isLeft)
    {
        //SetWeaponLocation();
        StartCoroutine(SwapMove(isLeft));
    }

    private IEnumerator SwapMove(bool isLeft)
    {
        float xMove = _slotNum == 1 ? 150f : 180;

        xMove = isLeft ? xMove : -xMove;

        Vector2 startPos = _rt.anchoredPosition;
        Vector2 targetPos = new Vector2(xMove, startPos.y);

        float duration = 0.3f;
        float moveTime = 0;

        while (moveTime < duration)
        {
            moveTime += Time.deltaTime;

            float time = moveTime / duration;

            _rt.anchoredPosition = Vector2.Lerp(startPos, targetPos, time);

            yield return null;
        }

        _rt.anchoredPosition = Vector2.zero;
        SetWeaponLocation();
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

            if (_slotNum == 1)
            {
                rt[i].localScale = new Vector3(1f, 1f, 1f);
            }
            else
            {
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
}
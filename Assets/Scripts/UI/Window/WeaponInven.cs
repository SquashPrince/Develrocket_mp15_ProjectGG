using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInven : MonoBehaviour
{
    private bool IsWeaponChange;
    private const float SWAP_COOL_TIME = 1f;
    private float swapCoolTime;
    private RectTransform rt;
    private Animator Ani;

    [SerializeField] private WeaponSlot[] _weaponSlot;
    private bool _isLeft;

    private void Awake()
    {
        CacheComponents();
    }

    // 최초 1회
    public void SetData()
    {
        IsWeaponChange = false;
        swapCoolTime = 0f;
        _isLeft = false;
    }

    private void Update()
    {
        if (IsWeaponChange)
        {
            swapCoolTime += Time.deltaTime;

            if (swapCoolTime >= SWAP_COOL_TIME)
            {
                swapCoolTime = 0f;
                IsWeaponChange = false;
            }
        }
    }

    public void WeaponSwap()
    {
        if (!IsWeaponChange)
        {
            float wheelInput = Input.GetAxis("Mouse ScrollWheel");

            if (wheelInput > 0f)
            {
                IsWeaponChange = true;
                Ani.SetTrigger("Left");
                _isLeft = true;
                // 휠 위로
            }
            else if (wheelInput < 0f)
            {
                IsWeaponChange = true;
                Ani.SetTrigger("Right");
                _isLeft = false;
                // 휠 아래로
            }
        }
    }

    public void SwapStop()
    {
        for (int i = 0; i < _weaponSlot.Length; i++)
        {
            _weaponSlot[i].Swap(_isLeft);
        }
    }

    private void CacheComponents()
    {
        rt = GetComponent<RectTransform>();
        Ani = GetComponent<Animator>();
    }
}
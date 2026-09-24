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

    private void Awake()
    {
        CacheComponents();
    }

    // 최초 1회
    public void SetData()
    {
        IsWeaponChange = false;
        swapCoolTime = 0f;
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
                // 휠 위로
            }
            else if (wheelInput < 0f)
            {
                IsWeaponChange = true;
                Ani.SetTrigger("Right");
                // 휠 아래로
            }
        }
    }

    public void SwapStop()
    {
        rt.rotation = Quaternion.identity;
        // 무기 슬롯 스왑
        
    }

    private void CacheComponents()
    {
        rt = GetComponent<RectTransform>();
        Ani = GetComponent<Animator>();
    }
}
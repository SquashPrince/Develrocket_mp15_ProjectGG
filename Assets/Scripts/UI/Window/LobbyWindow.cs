using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.HDROutputUtils;

public class LobbyWindow : MonoBehaviour, IWindowable
{
    [Header("영구 재화 (다이아몬드)")]
    [SerializeField] private CurrencySlot Diamond;

    public void SetActive()
    {
        // 플레이어 정보 넘겨주기
        Diamond.SetData();
    }

    public void ResetUI()
    {
    }

    private void Update()
    {
        SettingBtn();
    }

    /// <summary> 세팅 버튼 </summary>
    public void SettingBtn()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.Instance.PopUp.Open(EPopUpType.Setting);
        }
    }

    /// <summary> 던전 시작 버튼 </summary>
    public void DengonBtn()
    {
        UIManager.Instance.Window.NextEWindow = EWindowType.Battle;
        UIManager.Instance.Window.OpenLoading();
    }

    public void BackBtn()
    {
    }
}
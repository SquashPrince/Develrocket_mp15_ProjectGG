using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UI;

public class WindowManager : MonoBehaviour
{
    /// <summary> 현재 윈도우 창 </summary>
    [Header("현재 윈도우 창")]
    public EWindowType EWindow;
    [Header("다음 윈도우 창")]
    public EWindowType NextEWindow;
    /// <summary> 전체 윈도우 </summary>
    [Header("전체 윈도우")]
    [SerializeField] private GameObject[] _windows = new GameObject[System.Enum.GetValues((typeof(EWindowType))).Length];

    [SerializeField] private BattleWindow _battleWindow;
    [SerializeField] private Loading _load;

    public IWindowable NowWindow { get; private set; }

    public void Start()
    {
        GameStartPadOut();
    }

    /// <summary> 윈도우 창 열기</summary>
    /// <param name="eWindow"> 창 이름</param>
    public void Open(EWindowType eWindow)
    {
        if (eWindow == EWindow) return;

        EWindow = eWindow;
        Open();
    }

    /// <summary> 윈도우 창 열기 </summary>
    private void Open()
    {
        // 전체 창 끄기
        for (int i = 0; i < _windows.Length; i++)
        {
            _windows[i].SetActive(false);
        }

        // 맞는 창 열기
        _windows[(int)EWindow].SetActive(true);

        NowWindow = _windows[(int)EWindow].GetComponent<IWindowable>();
        NowWindow.SetActive();
    }

    public bool WindowCompare()
    {
        return EWindow == NextEWindow;
    }

    public void LoadingAddAction(Func<IEnumerator> action)
    {
        _load.AddAction(action);
    }

    public void TakeWeapon(PlayerWeaponEnum weaponEnum, Weapon weapon)
    {
        _battleWindow.TakeWeapon(weaponEnum, weapon);
    }

    public void WeaponSwap(bool isLeft)
    {
        _battleWindow.WeaponSwap(isLeft);
    }

    public void GameStartPadOut()
    {
        Open();
        _load.GameStartPadOut();
    }

    public void OpenLoading()
    {
        _load.SetActive();
    }

    /// <summary> 윈도우 창 뒤로가기 - 모든 윈도우 창 닫고 메인 화면으로 돌아감</summary>
    public void BackBtn()
    {
        // 현재 윈도우창은 로비와 배틀 2개 뿐이니 현재 사용 X
        EWindow = EWindowType.Lobby;
        Open();
    }
}
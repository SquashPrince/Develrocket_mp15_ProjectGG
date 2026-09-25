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
    [SerializeField] private GameObject[] Windows = new GameObject[System.Enum.GetValues((typeof(EWindowType))).Length];
    [SerializeField] private Loading Load;

    public IWindowable NowWindow { get; private set; }

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
        for (int i = 0; i < Windows.Length; i++)
        {
            Windows[i].SetActive(false);
        }

        // 맞는 창 열기
        Windows[(int)EWindow].SetActive(true);

        NowWindow = Windows[(int)EWindow].GetComponent<IWindowable>();
        NowWindow.SetActive();
    }

    public bool WindowCompare()
    {
        return EWindow == NextEWindow;
    }

    public void LoadingAddAction(Func<IEnumerator> action)
    {
        Load.AddAction(action);
    }

    public void OpenLoading()
    {
        Load.SetActive();
    }

    /// <summary> 윈도우 창 뒤로가기 - 모든 윈도우 창 닫고 메인 화면으로 돌아감</summary>
    public void BackBtn()
    {
        // 현재 윈도우창은 로비와 배틀 2개 뿐이니 현재 사용 X
        EWindow = EWindowType.Lobby;
        Open();
    }
}
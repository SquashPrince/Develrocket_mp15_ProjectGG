using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    [SerializeField] private GameObject LoadObj;
    [SerializeField] private Image Bg;
    [SerializeField] private GameObject LoadingBar;
    [SerializeField] private Image Bar;
    [SerializeField] private TextMeshProUGUI BarText;

    public void SetActive()
    {
        LoadObj.SetActive(true);
        StartCoroutine(LoadingOn());
    }

    private IEnumerator LoadingOn()
    {
        float alpha = 0f;

        while (true)
        {
            alpha += Time.deltaTime;

            Bg.color = new Color(Bg.color.r, Bg.color.g, Bg.color.b, alpha);

            yield return null;

            if(alpha > 1f)
            {
                Bg.color = new Color(Bg.color.r, Bg.color.g, Bg.color.b, alpha);
                break;
            }
        }

        LoadingBar.SetActive(true);
        Bar.fillAmount = 1f;
        LoadScene();
    }

    private void LoadScene()
    {
        string str = string.Empty;

        switch (UIManager.Instance.Window.EWindow)
        {
            case EWindowType.Lobby:
                str = "UI_Test_Battle";
                break;
            case EWindowType.Battle:
                str = "UI_Test";
                break;
        }

        StartCoroutine(LoadingAsync(str));
    }

    private IEnumerator LoadingAsync(string name)
    {
        //로딩이 완료되는대로 씬을 활성화할것인지
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(name);
        asyncOperation.allowSceneActivation = false;

        float time = 0f;

        //isDone는 로딩이 완료되었는지 확인하는 변수
        while (!asyncOperation.isDone)
        {
            //시간을 더해줌
            time += Time.deltaTime;
            //로딩이 얼마나 완료되었는지 0~1의 값으로 보여줌

            Bar.fillAmount = asyncOperation.progress;
            BarText.text = $"{asyncOperation.progress * 100}";

            //이건 로딩이 너무 빠르면 시간 제한 두는것이 좋다. 무거운 씬 로딩할땐 시간 체크하는 부분은 생략가능
            //3초 기다림(변동가능)
            if (time > 3)
            { 
                asyncOperation.allowSceneActivation = true; //씬 활성화
            }
            yield return null;
        }

        switch (UIManager.Instance.Window.EWindow)
        {
            case EWindowType.Lobby:
                UIManager.Instance.Window.Open(EWindowType.Battle);
                break;
            case EWindowType.Battle:
                UIManager.Instance.Window.Open(EWindowType.Lobby);
                break;
        }

        LoadingBar.SetActive(false);
        StartCoroutine(LoadingEnd());
    }

    private IEnumerator LoadingEnd()
    {
        float alpha = 1f;

        while (true)
        {
            alpha -= Time.deltaTime;

            Bg.color = new Color(Bg.color.r, Bg.color.g, Bg.color.b, alpha);

            yield return null;

            if (alpha < 0f)
            {
                Bg.color = new Color(Bg.color.r, Bg.color.g, Bg.color.b, alpha);
                break;
            }
        }

        Bar.fillAmount = 0f;
    }
}
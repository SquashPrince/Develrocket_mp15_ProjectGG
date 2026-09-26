using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    [SerializeField] private GameObject _loadObj;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private GameObject _loadingBar;
    [SerializeField] private Image _bar;
    [SerializeField] private TextMeshProUGUI _barText;

    private readonly List<Func<IEnumerator>> _actions = new();

    public void AddAction(Func<IEnumerator> action)
    {
        _actions.Add(action);
    }

    public void SetActive()
    {
        _loadObj.SetActive(true);
        StartCoroutine(LoadingOn());
    }

    private IEnumerator LoadingOn()
    {
        float alpha = 0f;

        while (true)
        {
            alpha += Time.deltaTime;

            _canvasGroup.alpha = alpha;

            yield return null;

            if(alpha > 1f)
            {
                _canvasGroup.alpha = 1f;
                break;
            }
        }

        _loadingBar.SetActive(true);
        _bar.fillAmount = 1f;

        yield return null;

        LoadWindow();
    }

    private void LoadWindow()
    {
        StartCoroutine(LoadingWindow());
    }

    private IEnumerator LoadingWindow()
    {
        yield return new WaitForSeconds(1f);

        if (!UIManager.Instance.Window.WindowCompare())
        {
            switch (UIManager.Instance.Window.EWindow)
            {
                case EWindowType.Lobby:
                    UIManager.Instance.Window.Open(EWindowType.Battle);
                    break;
                case EWindowType.Battle:
                    UIManager.Instance.Window.Open(EWindowType.Lobby);
                    break;
            }
        }
        _loadingBar.SetActive(false);
        StartCoroutine(LoadingEnd());
    }

    public void GameStartPadOut()
    {
        _loadObj.SetActive(true);
        StartCoroutine(LoadingEnd());
    }

    private IEnumerator LoadingEnd()
    {
        float alpha = 1f;

        while (true)
        {
            alpha -= Time.deltaTime;

            _canvasGroup.alpha = alpha;

            yield return null;

            if (alpha < 0f)
            {
                _canvasGroup.alpha = 0;
                break;
            }
        }

        _bar.fillAmount = 0f;
        _loadObj.SetActive(false);

        foreach (var action in _actions)
        {
            yield return action();
        }

        _actions.Clear();

        yield return null;
    }
}
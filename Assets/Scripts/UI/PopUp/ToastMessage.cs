using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToastMessage : PoolObject
{
    private const float DISPLAY_TIME = 2f;
    private string _str;
    [SerializeField] private TextMeshProUGUI _txt;
    [SerializeField] private CanvasGroup _canvasGroup;

    public void SetData(string str)
    {
        _str = str;
    }

    public override void Sleep()
    {
        gameObject.SetActive(false);
    }

    public override void WakeUp()
    {
        _canvasGroup.alpha = 1f;
        gameObject.SetActive(true);
        StartCoroutine(ToastCoroutine());
    }

    private IEnumerator ToastCoroutine()
    {
        yield return new WaitUntil(() => _str != string.Empty);
        UIManager.Instance.PopUp._toast.GridLayoutView(false);
        _txt.text = _str;

        float displayTime = 0f;

        while (displayTime < DISPLAY_TIME)
        {
            displayTime += Time.deltaTime;

            _canvasGroup.alpha = (DISPLAY_TIME - displayTime) / DISPLAY_TIME;

            yield return null;
        }

        _canvasGroup.alpha = 0f;
        _str = string.Empty;
        ReturnToPool();
    }
}
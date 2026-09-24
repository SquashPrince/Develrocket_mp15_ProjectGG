using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToastPopUp : MonoBehaviour
{
    [SerializeField] private ToastMessage _toast;
    private ObjectPool<ToastMessage> _toastPool;
    [SerializeField] private GridLayoutGroup _gridLayoutGroup;

    private void Start()
    {
        _toastPool = new ObjectPool<ToastMessage>(_toast, 10, transform);
    }

    public void ToastOn(string str)
    {
        GridLayoutView(true);
        var toast = _toastPool.Pop();
        toast.SetData(str);
    }

    public void GridLayoutView(bool view)
    {
        _gridLayoutGroup.enabled = view;
    }
}
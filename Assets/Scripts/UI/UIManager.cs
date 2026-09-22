using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;

    public WindowManager Window { get; private set; }
    public PopUpManager PopUp { get; private set; }

    public static UIManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<UIManager>();
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    private void Awake()
    {
        SetSingleton();
        CacheComponents();
    }


    private void SetSingleton()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void CacheComponents()
    {
        Window = gameObject.transform.GetChild(0).GetComponent<WindowManager>();
        PopUp = gameObject.transform.GetChild(1).GetComponent<PopUpManager>();
    }
}
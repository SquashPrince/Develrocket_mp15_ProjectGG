using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpManager : MonoBehaviour
{
    public ToastPopUp _toast;
    [SerializeField] private SettingPopUp _setting;
    [SerializeField] private AbilityPopUp _ability;

    public void Open(EPopUpType ePopUp)
    {
        switch (ePopUp)
        {
            case EPopUpType.Setting:
                _setting.gameObject.SetActive(true);
                _setting.SetData();
                break;
            case EPopUpType.Ability:
                break;
        }
    }
}
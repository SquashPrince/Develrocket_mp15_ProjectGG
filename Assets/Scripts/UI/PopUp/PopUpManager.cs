using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpManager : MonoBehaviour
{
    [SerializeField] private SettingPopUp Setting;
    [SerializeField] private AbilityPopUp Ability;

    public void Open(EPopUpType ePopUp)
    {
        switch (ePopUp)
        {
            case EPopUpType.Setting:
                Setting.gameObject.SetActive(true);
                Setting.SetData();
                break;
            case EPopUpType.Ability:
                break;
        }
    }
}
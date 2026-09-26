using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;

public class PopUpManager : MonoBehaviour
{
    public ToastPopUp _toast;
    [SerializeField] private SettingPopUp _setting;
    [SerializeField] private AbilityPopUp _ability;
    [SerializeField] private BattleExitPopUp _battleExit;

    public void Open(EPopUpType ePopUp)
    {
        switch (ePopUp)
        {
            case EPopUpType.Ability:
                break;
            case EPopUpType.BattleExit:
                _battleExit.gameObject.SetActive(true);
                break;
        }
    }
}
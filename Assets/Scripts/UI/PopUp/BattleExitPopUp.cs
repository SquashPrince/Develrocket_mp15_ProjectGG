using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleExitPopUp : MonoBehaviour
{
    public void YesBtn()
    {
        UIManager.Instance.Window.NextEWindow = EWindowType.Lobby;
        UIManager.Instance.Window.OpenLoading();
        gameObject.SetActive(false);
    }

    public void NoBtn()
    {
        gameObject.SetActive(false);
    }
}   
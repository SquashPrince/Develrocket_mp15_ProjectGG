using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleExitPopUp : MonoBehaviour
{
    public void YesBtn()
    {
        GameManager.Instance.ReturnToLobby();
        gameObject.SetActive(false);
    }

    public void NoBtn()
    {
        GameManager.Instance.SetPaused(false);
        gameObject.SetActive(false);
    }
}   
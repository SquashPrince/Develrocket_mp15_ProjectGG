using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPopUp : MonoBehaviour
{
    [SerializeField] private SettingBgm BgmSetting;
    [SerializeField] private SettingSfx SfxSetting;

    public void SetData()
    {
        BgmSetting.SetBgm();
        SfxSetting.SetSfx();
    }

    public void CloseBtn()
    {
        gameObject.SetActive(false);
    }
}
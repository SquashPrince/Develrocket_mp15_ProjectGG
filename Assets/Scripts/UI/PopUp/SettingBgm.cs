using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingBgm : MonoBehaviour
{
    private Animator Ani;
    [SerializeField] private bool IsBgm;
    [SerializeField] private Image BgmBg;
    [SerializeField] private RectTransform BgmSwitchRt;
    private Color OffColor = new Color32(27, 63, 115, 255);
    private Color ONColor = new Color32(7, 255, 0, 255);

    private void Awake()
    {
        CacheComponenets();
    }
    public void SetBgm()
    {
        BgmBg.color = IsBgm ? ONColor : OffColor;
        BgmSwitchRt.anchoredPosition = new Vector2(IsBgm ? 50 : -50, 0);
    }

    public void BgmOnOff()
    {
        IsBgm = !IsBgm;
        Ani.SetTrigger(IsBgm ? "BgmOn" : "BgmOff");
    }

    public void BgmAniEnd()
    {
        SetBgm();
    }

    private void CacheComponenets()
    {
        Ani = GetComponent<Animator>();
    }
}

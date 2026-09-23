using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingSfx : MonoBehaviour
{
    private Animator Ani;
    private bool IsSfx;
    [SerializeField] private Image SfxBg;
    [SerializeField] private RectTransform SfxSwitchRt;
    private Color OffColor = new Color32(27, 63, 115, 255);
    private Color ONColor = new Color32(7, 255, 0, 255);

    private void Awake()
    {
        CacheComponenets();
    }

    public void SetSfx()
    {
        SfxBg.color = IsSfx ? ONColor : OffColor;
        SfxSwitchRt.anchoredPosition = new Vector2(IsSfx ? 50 : -50, 0);
    }

    public void SfxOnOff()
    {
        IsSfx = !IsSfx;
        Ani.SetTrigger(IsSfx ? "EffOn" : "EffOff");
    }

    public void SfxAniEnd()
    {
        SetSfx();
    }

    private void CacheComponenets()
    {
        Ani = GetComponent<Animator>();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrencySlot : MonoBehaviour
{
    public ECurrencyType ECurrency;

    [SerializeField] private Sprite[] _img;
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _cntTxt;

    public void SetData()// 플레이어 받기
    {
        // 이미지 변경
        // 현재 이미지 없음. 추가 시 주석 삭제
        _icon.sprite = _img[(int)ECurrency];

        // 플레이어한테 받은 재화
        switch (ECurrency)
        {
            // 플레이어 영구 재화 (다이아몬드)
            case ECurrencyType.Gem:
                _cntTxt.text = GameManager.Instance.PlayerValues.Gem.ToString();
                break;
            // 플레이어 던전 재화 (골드)
            case ECurrencyType.Gold:
                _cntTxt.text = GameManager.Instance.PlayerValues.Gold.ToString();
                break;
            default:
                _cntTxt.text = string.Empty;
                break;
        }
    }

    public void RefreshUI()
    {
        // 플레이어한테 받은 재화
        switch (ECurrency)
        {
            // 플레이어 영구 재화 (다이아몬드)
            case ECurrencyType.Gem:
                _cntTxt.text = GameManager.Instance.PlayerValues.Gem.ToString();
                break;
            // 플레이어 던전 재화 (골드)
            case ECurrencyType.Gold:
                _cntTxt.text = GameManager.Instance.PlayerValues.Gold.ToString();
                break;
            default:
                _cntTxt.text = string.Empty;
                break;
        }
    }
}
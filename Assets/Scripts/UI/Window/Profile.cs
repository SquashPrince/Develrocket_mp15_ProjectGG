using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Profile : MonoBehaviour
{
    [SerializeField] private Image _frameImg;
    [SerializeField] private Image _profileImg;


    // -- 플레이어 체력 --
    private int _maxHp;
    private int _hp;
    [SerializeField] private Image[] _hpImg;
    [SerializeField] private Sprite _hpSprite;
    [SerializeField] private Sprite _hpHalfSprtie;
    [SerializeField] private Sprite _hpNoneSprite;

    // 최초 1회
    public void SetData()
    {
        // 프로필에 나타낼 플레이어 정보
        // 체력
        // 최대 체력 넣기
        _maxHp = GameManager.Instance.PlayerValues.MaxHp;
        _hp = _maxHp;

        SetUI();
    }

    private void SetUI()
    {
        SetHP(_hp);
    }

    public void SetHP(int hp)
    {
        _hp = hp;
        int heartCnt = 0;
        bool isHalf = false;

        heartCnt = _hp / 2;
        isHalf = _hp % 2 == 1;

        for(int i = 0; i < _hpImg.Length; i++)
        {
            if (i < heartCnt)
            {
                _hpImg[i].sprite = _hpSprite;
            }
            else if (i == heartCnt)
            {
                _hpImg[i].sprite = isHalf ? _hpHalfSprtie : _hpNoneSprite;
            }
            else
            {
                _hpImg[i].sprite = _hpNoneSprite;
            }
        }
    }
}
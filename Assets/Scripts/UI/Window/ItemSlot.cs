using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    private int _slotNum;
    private Item _item;
    [SerializeField] private Image _itemImg;
    [SerializeField] private TextMeshProUGUI _itemCntTxt;


    private bool _isUse = true;
    [SerializeField] private Image _coolDownImg;
    [SerializeField] private float _coolDown;
    [SerializeField] private TextMeshProUGUI _coolDownTxt;

    public void SetData(int slotNum, Item item)
    {
        _slotNum = slotNum;
        _item = item;
        _isUse = true;
        // 쿨타임 정보 받기
        //_coolDown = _item.
        _coolDownImg.gameObject.SetActive(false);
        _coolDownTxt.gameObject.SetActive(false);
        SetUI();
    }

    private void SetUI()
    {
        SlotInfo();
    }

    private void SlotInfo()
    {
        // 아이템 슬롯에 따라 다름
        switch (_slotNum)
        {
            // 회복 물약 (고정)
            case 0:
                // 아이템 이미지
                //_itemImg.sprite = _item.img
                // 아이템 소지 개수
                //_itemCntTxt.text = 
                break;
            // 탄막 제거 (고정)
            case 1:
                // 아이템 이미지
                //_itemImg.sprite = _item.img
                // 아이템 소지 개수
                //_itemCntTxt.text = 
                break;
            // 그 외 소모형 아이템 (변경 가능)
            case 2:
                if (_item == null)
                {
                    // 없다면 비활성화
                    _itemCntTxt.text = "0";
                    _itemImg.gameObject.SetActive(false);
                    _itemCntTxt.gameObject.SetActive(false);
                }

                // 아이템 소지 시
                // 아이템 이미지
                //_itemImg.sprite = _item.img
                // 아이템 소지 개수
                //_itemCntTxt.text = 
                // 있으니 활성화
                _itemImg.gameObject.SetActive(true);
                _itemCntTxt.gameObject.SetActive(true);
                break;
        }
    }

    public void UseItem()
    {
        // 아이템이 없으면 사용 불가
        //if (_item == null)
        //{
        //    //UIManager.Instance.PopUp._toast.ToastOn("아이템이 없습니다.");
        //    UIManager.Instance.PopUp._toast.ToastOn("No Item.");
        //    return;
        //}

        // 아이템 소지 개수가 없으면 사용 불가
        /*
        if (_item.)
        {
            UIManager.Instance.PopUp._toast.ToastOn("아이템이 없습니다.");
            UIManager.Instance.PopUp._toast.ToastOn("아이템이 없습니다.");
            return;
        }
        */

        // 현재 사용불가
        if (!_isUse)
        {
            //UIManager.Instance.PopUp._toast.ToastOn("아직 사용할 수 없습니다.");
            UIManager.Instance.PopUp._toast.ToastOn("Use Item CoolDown.");
            return;
        }

        _isUse = false;
        // 해당 아이템 사용
        // 플레이어 아이템 사용이랑 연결

        // 사용후 UI 갱신
        ResetUI();

        StartCoroutine(CoolDownCoroutine());
    }

    private void ResetUI()
    {
        //_itemCntTxt.text = 
        if(_slotNum == 2)
        {
            // 슬롯 2번의 아이템 개수가 0이라면 아이템 삭제
            //_item = null;
        }
    }

    private IEnumerator CoolDownCoroutine()
    {
        _coolDownImg.gameObject.SetActive(true);
        _coolDownTxt.gameObject.SetActive(true);

        float coolDown = 0f;

        while (coolDown < _coolDown)
        {
            _coolDownImg.fillAmount = (_coolDown - coolDown) / _coolDown;
            if ((_coolDown - coolDown) > 1f)
            {
                _coolDownTxt.text = ((int)(_coolDown - coolDown)).ToString();
            }
            else
            {
                _coolDownTxt.text = (_coolDown - coolDown).ToString("N1");
            }

            yield return null;

            coolDown += Time.deltaTime;
        }

        _coolDownImg.gameObject.SetActive(false);
        _coolDownTxt.gameObject.SetActive(false);

        _isUse = true;
    }
}

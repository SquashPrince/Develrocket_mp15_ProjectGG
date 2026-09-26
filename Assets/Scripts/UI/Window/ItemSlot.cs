using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] private int _slotNum;
    private PlayerItemSlotState _itemState;
    [SerializeField] private Image _itemImg;
    [SerializeField] private TextMeshProUGUI _itemCntTxt;


    private bool _isUse = true;
    [SerializeField] private Image _coolDownImg;
    [SerializeField] private float _coolDown;
    [SerializeField] private TextMeshProUGUI _coolDownTxt;

    public void SetData(PlayerItemSlotState itemState)
    {
        _itemState = itemState;
        _isUse = true;
        // 쿨타임 정보 받기
        _coolDown = _itemState.CooldownDuration;
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
        // 아이템 없을 시
        if (_itemState.Item == null)
        {
            _itemImg.sprite = null;
            _itemCntTxt.text = "0";

            // 비활성화
            _itemImg.gameObject.SetActive(false);
            _itemCntTxt.gameObject.SetActive(false);
        }
        else
        {
            // 아이템 이미지
            _itemImg.sprite = _itemState.Icon;
            // 아이템 소지 개수
            _itemCntTxt.text = _itemState.Count.ToString();

            _itemImg.gameObject.SetActive(true);
            _itemCntTxt.gameObject.SetActive(true);
        }
    }

    public void UseItem()
    {
        // 아이템이 없으면 사용 불가
        if (_itemState.Item == null)
        {
            //UIManager.Instance.PopUp._toast.ToastOn("아이템이 없습니다.");
            UIManager.Instance.PopUp._toast.ToastOn("No Item.");
            return;
        }

        // 아이템 소지 개수가 없으면 사용 불가
        if (_itemState.Count <= 0)
        {
            //UIManager.Instance.PopUp._toast.ToastOn("아이템이 없습니다.");
            UIManager.Instance.PopUp._toast.ToastOn("Enough Item.");
            return;
        }

        // 쿨타임 중 이므로 사용 불가
        if (!_isUse)
        {
            //UIManager.Instance.PopUp._toast.ToastOn("아직 사용할 수 없습니다.");
            UIManager.Instance.PopUp._toast.ToastOn("Item CoolDown.");
            return;
        }

        _isUse = false;
        // 해당 아이템 사용
        StartCoroutine(CoolDownCoroutine());

        // 사용후 UI 갱신
        ResetUI();
    }

    private void ResetUI()
    {
        _itemState = GameManager.Instance.PlayerValues.ItemSlots.GetSlotState((PlayerItemEnum)_slotNum);

        // 아이템 없을 시
        if (_itemState.Item == null)
        {
            _itemImg.sprite = null;
            _itemCntTxt.text = "0";

            // 비활성화
            _itemImg.gameObject.SetActive(false);
            _itemCntTxt.gameObject.SetActive(false);
        }
        else
        {
            // 아이템 이미지
            _itemImg.sprite = _itemState.Icon;
            // 아이템 소지 개수
            _itemCntTxt.text = _itemState.Count.ToString();

            _itemImg.gameObject.SetActive(true);
            _itemCntTxt.gameObject.SetActive(true);
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

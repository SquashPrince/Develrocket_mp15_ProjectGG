using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 상호작용 확인용 임시 클래스
/// </summary>
public class TestPlayerContoller : MonoBehaviour, IInteracter
{
    [SerializeField] private MonoBehaviour _testItem;
    [SerializeField] private Transform _waeponTr;
    [SerializeField] private Transform _amuletTr;

    public Transform WeaponTR => _waeponTr;
    public Transform AmuletTR => _amuletTr;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if(_testItem != null)
                _testItem.GetComponent<IInteractable>().GetItem(this);
        }
    }

    /// <summary>
    /// 부적 아이템 습득시 인터렉션 해제용
    /// </summary>
    public void UnlinkItem()
    {
        _testItem = null;
    }
}

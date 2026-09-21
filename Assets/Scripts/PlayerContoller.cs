using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContoller : MonoBehaviour, IInteracter
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

    public void UnlinkItem()
    {
        _testItem = null;
    }
}

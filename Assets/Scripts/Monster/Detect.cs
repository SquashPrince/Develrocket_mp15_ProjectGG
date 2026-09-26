using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detect : MonoBehaviour
{
    [SerializeField] private MoveMonster _moveMonster;
    [SerializeField] private BombMonster _bombMonster;
    [SerializeField] private TurretMonster _turretMonster;
    [SerializeField] private ChargeMonster _chargeMonster;
    
    private void OnTriggerEnter(Collider other)
    {
        if (_moveMonster != null)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                _moveMonster.IsplayerInsight = true;
            }
        }

        if (_turretMonster != null)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                _turretMonster.IsplayerInsight = true;
            }
        }

        if (_chargeMonster != null)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                _chargeMonster.IsplayerInsight = true;
            }
        }
        
        if (_bombMonster != null)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                _bombMonster.IsplayerInsight = true;
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (_turretMonster != null)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                _turretMonster.IsplayerInsight = false;
            }
        }
        
    }

}

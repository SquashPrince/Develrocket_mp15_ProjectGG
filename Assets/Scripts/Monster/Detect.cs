using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detect : MonoBehaviour
{
    [SerializeField] private MoveMonster _moveMonster;
    [SerializeField] private BombMonster _bombMonster; 
    
    private void OnTriggerEnter(Collider other)
    {
        if (_moveMonster != null)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                _moveMonster.IsplayerInsight = true;
            }
        }
        
        /*if (_bombMonster != null)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                _bombMonster.IsplayerInsight = true;
            }
        }*/
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovement : MonoBehaviour
{
    [SerializeField] private Monster _monster;
    //[SerializeField] private Transform _player;
    private Transform _player;
    
    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        MoveToPlayer();
    }

    private void MoveToPlayer()
    {
        transform.LookAt(_player.position);
        float distance = Vector3.Distance(transform.transform.position, _player.transform.position);
        if (distance > 5)
        {
            Vector3 direction =
                (_player.position - transform.position).normalized;

            transform.position +=
                direction * _monster.MoveSpeed * Time.deltaTime;
        }
        
    }
}

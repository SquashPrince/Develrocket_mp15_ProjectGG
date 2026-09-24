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
        Vector3 direction =
            (_player.position - transform.position).normalized;

        transform.position +=
            direction * _monster.MoveSpeed * Time.deltaTime;
    }
}

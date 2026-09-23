using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern : MonoBehaviour
{
    protected BossControl _bossControl; 
    [SerializeField] protected float _cooldown;
    protected Transform _player;

    public void Awake()
    {
        _bossControl = GetComponent<BossControl>();
        _player = GameObject.FindGameObjectWithTag("Player").transform;
    }
}

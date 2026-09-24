using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern : MonoBehaviour
{
    protected BossControl _bossControl; 
    [SerializeField] protected float _cooldown;
    [SerializeField] protected int _damage;
    public int _maxtimes;
    
    protected Transform _player;

    public void Awake()
    {
        _bossControl = GetComponent<BossControl>();
        //Debug.Log(_bossControl == null);
        _player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void Start()
    {
        StartCoroutine(CoolDown());
    }
    
    public IEnumerator CoolDown()
    {
        
        while (true)
        {
            // 쿨타임 기다리기
            //Debug.Log("_cooldown");
            yield return new WaitForSeconds(_cooldown);
            
            // 쿨타임이 끝나면 Queue에 패턴 추가
            //Debug.Log(_bossControl == null);
            
            _bossControl.AddPattern(PatternRoutine);
            _bossControl.AddPattern(_maxtimes);
        }
        
    }

    protected virtual IEnumerator PatternRoutine()
    {
        yield return null;
    }
    
    
    
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("Monster Status")]
    [SerializeField] private int _maxHealth;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _attackCooldown;
    [SerializeField] private int _dropGold;
    [SerializeField] private bool _hasDeathEffect;
    [SerializeField] private bool _invincibility = false;

    // 무적 판정 구현예정
    public bool Invincibility
    {
        get
        {
            return _invincibility;
        }
        set
        {
            _invincibility = value;
        }
    }

    // 체력이 변경됐을 때 알림
    public event Action<int, int> OnHealthChanged;

    private int _currentHealth;

    public int MaxHealth => _maxHealth;
    public int CurrentHealth => _currentHealth;
    public float MoveSpeed
    {
        get
        {
            return _moveSpeed;
        }
        set
        {
            _moveSpeed = value;
        }
    }

    public float AttackCooldown => _attackCooldown;
    public int DropGold => _dropGold;
    public bool HasDeathEffect => _hasDeathEffect;

    private float _initSpeed;

    private void Awake()
    {
        _currentHealth = _maxHealth;
        _initSpeed = _moveSpeed;
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;

        if (_currentHealth < 0)
        {
            _currentHealth = 0;
        }

        // 현재 체력, 최대 체력을 구독 전달
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

        Debug.Log($"몬스터 체력 : {_currentHealth} / {_maxHealth}");

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("몬스터 사망");
        Destroy(gameObject);
    }

    public void InitSpeed()
    {
        _moveSpeed = _initSpeed;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Monster : MonoBehaviour, IDamagable
{
    [Header("Monster Status")]
    [SerializeField] private int _maxHP;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _attackCooldown;
    [SerializeField] private int _dropGold;
    [SerializeField] private bool _hasDeathEffect;
    [SerializeField] private bool _invincibility = false;
    //[SerializeField] private ParticleSystem targetEffect;
    

    // bool 값으로 아이템 하나만 떨어뜨리도록
    // 지금 총알에서 충돌 여러번 일어나서 3개씩 떨어뜨리는거 같음
    public GameObject GameObject => gameObject;
    
    private Animator _animator;
    private BossControl _bossControl;

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

    private int _currentHp;

    public int MaxHp => _maxHP;
    public int CurrentHp => _currentHp;
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
        _currentHp = _maxHP;
        _initSpeed = _moveSpeed;
        _animator = GetComponentInChildren<Animator>();
        _bossControl = GetComponent<BossControl>();
        //targetEffect = GetComponentInChildren<ParticleSystem>();
    }

    [SerializeField] public bool isDead = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            collision.gameObject.GetComponent<IDamagable>().TakeDamage(1);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        
        Debug.Log($"데미지 받음 : {damage}");
        _currentHp -= damage;

        if (_currentHp < 0)
        {
            _currentHp = 0;
        }

        // 현재 체력, 최대 체력을 구독 전달
        OnHealthChanged?.Invoke(_currentHp, _maxHP);

         Debug.Log($"몬스터 체력 : {_currentHp} / {_maxHP}");

        if (_currentHp <= 0)
        {
            StartCoroutine(Die());
        }
    }


    [SerializeField] private TurretMonster _turretMonster;
    private IEnumerator Die()
    {
        isDead = true;
        Debug.Log("몬스터 사망 코루틴 실행");
        if (_turretMonster != null)
        {
            _turretMonster.Dead();
        }

        if (_bossControl != null)
        {
            _bossControl.StopPattern();
            _animator.SetTrigger("IsDead");
            yield return new WaitForSeconds(1.5f);
        }

        /*if (targetEffect != null)
        {
            Debug.Log("이펙트");
            targetEffect.transform.SetParent(null);
            targetEffect.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
        }
        Debug.Log(targetEffect == null);*/
        
        // 아이템 혹은 골드 드랍
        ItempDrop();
        GoldDrop();
        
        
        Destroy(gameObject);
    }

    public void InitSpeed()
    {
        _moveSpeed = _initSpeed;
    }


    private int ran = 0;
    [SerializeField] private Item[] _items = new Item[4];
    public void ItempDrop()
    {
        Debug.Log("Itemp Drop");
        ran = Random.Range(0, 100);

        if (ran < 50)
        {
            Debug.Log("아이템 안나옴");
            return;
        }
        
        ran = Random.Range(0, _items.Length);
        Instantiate(_items[ran], transform.position, Quaternion.identity);
        Debug.Log($"아이템 나옴 : {_items[ran].name}");
        // 아이템 드랍
    }

    
    [SerializeField] private float _gold;
    public void GoldDrop()
    {
        Debug.Log($"Gold Drop : {_gold}");
    }

    public void DieTest()
    {
        Destroy(gameObject);
        Debug.Log("제발 로그 뜨고 죽어줘 끝남");
    }
}

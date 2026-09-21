using UnityEngine;

namespace Player
{
    public class PlayerValues : MonoBehaviour
    {
        private const int BaseMaxHp = 10;
        private const int BaseMoveSpeed = 5;
        private const float BaseAttackSpeed = 0.5f;
        private const float BaseDamageDelay = 0.3f;
        private const float BaseDodgeTime = 0.5f;
        private const int BaseStartGold = 100;
        private const int BaseStartGem = 0;
    
        // 시작 스탯 상수
        // ============================================================

        private int _currentHealth;
        private int _maxHealth;
        private int _moveSpeed;
        private float _attackSpeed;
        private float _damagedDelay;
        private float _dodgeTime;
        private int _gold;
        private int _gem;
    
        // 기본 플레이어 변수
        // ============================================================
        
        
    
    
        // 시작 보너스 리스트
        // ============================================================
        
        public int CurrentHealth
        {
            get => _currentHealth;
            set
            {
                if (value > _maxHealth)
                {
                    _currentHealth = _maxHealth;
                }
                else if (value < 0)
                {
                    _currentHealth = 0;
                }
                else
                {
                    _currentHealth = value;
                }
            }
        }
        // 최대체력 초과시 
    
        // 프로퍼티
        // ============================================================
        
        private bool IsHealthFull => _currentHealth == _maxHealth;
        
        // 추가변수
        // ============================================================

        private void Start()
        {
            Init();
        }
    
        // ============================================================

        private void Init()
        {
            SetValuesDefault();
        }
    
        // ============================================================

        private void SetValuesDefault()
        {
            _maxHealth = BaseMaxHp;
            _currentHealth = _maxHealth;
            _moveSpeed = BaseMoveSpeed;
            _attackSpeed = BaseAttackSpeed;
            _damagedDelay = BaseDamageDelay;
            _dodgeTime = BaseDodgeTime;
            _gold = BaseStartGold;
            _gem = BaseStartGem;
        }

        private void SetStatBonus()
        {
            // 특성 리스트 돌면서 스탯 변경
        }

    }
}

using UnityEngine;

namespace Player
{
    public struct PlayerValueStruct
        {
        private const int BaseMaxHp = 10;
        private const int BaseMoveSpeed = 5;
        private const int BaseMinMoveSpeed = 1;
        private const float BaseAttackSpeed = 0.5f;
        private const float BaseMinAttackSpeed = 0.1f;
        private const float BaseDamageDelay = 0.3f;
        private const float BaseMinDamageDelay = 0.1f;
        private const float BaseDodgeTime = 0.5f;
        private const float BaseMinDodgeTime = 0.1f;
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
        
        // 플레이어 스탯 변수 
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
        
        public int MaxHealth
        {
            get => _maxHealth;
            set
            {
                if (value <= 0) _maxHealth = 0;
                else _maxHealth = value;
            }
        }

        public int MoveSpeed
        {
            get => _moveSpeed;
            set
            {
                if (value < BaseMinMoveSpeed)
                {
                    _moveSpeed = BaseMinMoveSpeed;
                }
                else
                {
                    _moveSpeed = value;
                }
            }
        }

        public float AttackSpeed
        {
            get => _attackSpeed;
            set
            {
                if (value < BaseMinAttackSpeed)
                {
                    _attackSpeed = BaseMinAttackSpeed;
                }
                else
                {
                    _attackSpeed = value;
                }
            }
        }

        public float DamagedDelay
        {
            get => _damagedDelay;
            set
            {
                if (value < BaseMinDamageDelay)
                {
                    _damagedDelay = BaseMinDamageDelay;
                }
                else
                {
                    _damagedDelay = value;
                }
            }
        }

        public float DodgeTime
        {
            get => _dodgeTime;
            set
            {
                if (value < BaseMinDodgeTime)
                {
                    _dodgeTime = BaseMinDodgeTime;
                }
                else
                {
                    _dodgeTime = value;
                }
            }
        }

        public int Gold
        {
            get => _gold;
            set
            {
                if (value < 0)
                {
                    _gold = 0;
                }
                else
                {
                    _gold = value;
                }
            }
        }

        public int Gem
        {
            get => _gem;
            set
            {
                if (value < 0)
                {
                    _gem = 0;
                }
                else
                {
                    _gem = value;
                }
            }
        }
        
        // 프로퍼티
        // ============================================================

        public void SetValuesDefault()
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
    }
    
}

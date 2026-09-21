using UnityEngine.Rendering;

namespace Player
{
    public struct PlayerValueStruct
    {
        private const int BaseRate = 100; // 백분율 기본값
        private const int BaseMaxHp = 10; // 최대HP
        private const int BaseMoveSpeed = 5; // 이동속도
        private const int BaseMinMoveSpeed = 1; // 최소이동속도
        private const float BaseAttackRate = 0.5f; // 공격주기
        private const float BaseMinAttackRate = 0.1f; // 최소공격주기
        private const float BaseDamageDelay = 0.3f; // 피격무적시간
        private const float BaseMinDamageDelay = 0.1f; // 최소 피격무적시간
        private const float BaseDodgeTime = 0.5f; // 회피무적시간
        private const float BaseMinDodgeTime = 0.1f; // 최소 회피무적시간
        private const int BaseStartGold = 100; // 시작골드
        private const int BaseStartGem = 0; // 시작보석
        private const int BaseShield = 0; // 보호막
        private const int BaseAttackDamage = 10; // 공격력
        
        // 상수 (기본값)
        // ============================================================
        
        private int _currentHealth; // 현재체력
        private int _moveSpeed; // 이동속도
        private int _attackSpeed; // 공격속도 증가율
        private float _attackRate; // 실제공격주기
        private float _damagedDelay; // 피격무적시간
        private float _dodgeTime; // 회피무적시간
        private bool _isNewChara; // 신규 캐릭터 여부

        // 변수
        // ============================================================
        
        public int CurrentHealth
        {
            get => _currentHealth;
            set
            {
                if (value > MaxHealth)
                {
                    _currentHealth = MaxHealth;
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
            get;
            set;
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

        public float AttackRate
        {
            get => _attackRate;
            set
            {
                if (value < BaseMinAttackRate)
                {
                    _attackRate = BaseMinAttackRate;
                }
                else
                {
                    _attackRate = value;
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

        public int Gold { get; set; }
        public int Gem { get; set; }
        public int GoldGainRate { get; set; }
        public int AttackSpeedRate { get; set; }
        public int Shield { get; set; }
        public int AttackDamage { get; set; }

        // 프로퍼티
        // ============================================================
        
        /** 기본값 설정 */
        public void SetDefaultValues()
        {
            MaxHealth = BaseMaxHp;
            _currentHealth = MaxHealth;
            _moveSpeed = BaseMoveSpeed;
            _attackRate = BaseAttackRate;
            _damagedDelay = BaseDamageDelay;
            _dodgeTime = BaseDodgeTime;
            Gold = BaseStartGold;
            AttackSpeedRate = BaseRate;
            GoldGainRate = BaseRate;
            Shield = BaseShield;
            AttackDamage = BaseAttackDamage;
            // 신규 캐릭터 설정
            if (!_isNewChara) return;
            Gem = BaseStartGem;
            _isNewChara = false;
        }

        public void SetAttackSpeed()
        {
            
        }
        
        // 퍼블릭 메서드
        // ===========================================================
    }
}

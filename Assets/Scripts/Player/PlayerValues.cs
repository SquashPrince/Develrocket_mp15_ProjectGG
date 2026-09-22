using UnityEngine;

namespace Player
{
    public class PlayerValues : MonoBehaviour, IInteracter
    {
        private const float DefaultHitTime = 0.3f; // 피격무적시간
        private const float DefaultDodgeTime = 0.5f; // 회피무적시간
        private const float DefaultAttackSpeed = 0.5f; // 공격속도
        private const float DefaultMinAttackSpeed = 0.1f; // 최소공격속도
        private const int DefaultMoveSpeed = 5; // 이동속도
        private const int DefaultMinMoveSpeed = 1; // 최소이동속도
        private const int DefaultMaxHp = 10; // 최대HP
        private const int DefaultStartGold = 100; // 시작골드
        private const int DefaultStartGem = 0; // 시작보석
        private const int DefaultShield = 0; // 보호막
        
        // 상수 (기본값)
        // ============================================================
        public float BaseHitTime { get; set; }
        public float BaseDodgeTime { get; set; }
        public float BaseAttackSpeed { get; set; }
        public int BaseMoveSpeed { get; set; }
        public int BaseMaxHp { get; set; }
        public int BaseGold {get; set;}
        public int BaseGem { get; set; }
        public int BaseHp { get; set; }
        public int BaseShield { get; set; }
        
        // 프로퍼티 (연산X)
        // ============================================================
        public int rateHitTime; // 무적시간 배율
        public int rateDodgeTime; // 회피무적 배율
        public int rateAttackSpeed; // 공격속도 배율
        public int rateMoveSpeed; // 이동속도 배율
        public int rateHpIncrease; // 회복량 배율
        public int rateMaxHp; // 최종체력 배율
        public int rateGainGold; // 골드획득 배율
        public int rateGainGem; // 보석획득 배율
        
        // 변수 (배율)
        // ============================================================
        public int Hp
        {
            get => BaseHp;
            set
            {
                int amount = (value - BaseHp);
                int hp = amount > 0
                    ? BaseHp + (amount * rateHpIncrease)/100
                    : BaseHp + amount;
                if (hp > MaxHp) BaseHp = MaxHp;
                else if (hp < 0) BaseHp = 0;
                else BaseHp = hp;
            }
        }
        public int MaxHp => (BaseMaxHp * rateMaxHp)/100;
        public int MoveSpeed
        {
            get
            {
                int moveSpeed = (BaseMoveSpeed * rateMoveSpeed)/100;
                return (moveSpeed < DefaultMinMoveSpeed)? DefaultMinMoveSpeed : moveSpeed;
            }
        }
        public float AttackSpeed
        {
            get
            {
                float attackSpeed = (BaseAttackSpeed * 100) / rateAttackSpeed;
                return (attackSpeed < DefaultMinAttackSpeed) ? DefaultMinAttackSpeed : attackSpeed;
            }
        }
        public float DodgeTime
        {
            get
            {
                float dodgeTime = (BaseDodgeTime * rateDodgeTime)/100;
                return dodgeTime;
            }
        }
        public float HitTime
        {
            get
            {
                float hitTime = (BaseHitTime * rateHitTime)/100;
                return hitTime;
            }
        }
        public int Gold
        {
            get => BaseGold;
            set
            {
                int amount = (value - BaseGold);
                BaseGold = amount > 0 ? BaseGold + (amount*rateGainGold)/100 : value;
            }
        }
        public int Gem
        {
            get => BaseGem;
            set
            {
                int amount = (value - BaseGem);
                BaseGem = amount > 0 ? BaseGem + (amount*rateGainGem)/100 : value;
            }
        }

        // 프로퍼티 (연산O)
        // ============================================================
        private IInteractable _interactable;
        private Transform _weaponTransform;
        private Weapon _Equipweapon;
        
        private bool _isNewChara = true;
        private bool _hasWeapon => _Equipweapon != null;
        
        // 인스턴스 & 변수
        // ==================================================
        private void Start()
        {
            SetDefault(); 
        }
        // 이벤트 함수
        // ==================================================
        
        /** 시작시 플레이어 데이터를 초기화 하는 함수 */
        private void SetDefault()
        {
            SetToStartValues();
            SetToStartRate();
            if (_isNewChara) SetNewChara();
            // 델리게이트로 특성 불러오기
        }
        
        /** 플레이어 데이터 - 값 초기화 */
        private void SetToStartValues()
        {
            BaseMaxHp = DefaultMaxHp;
            BaseHp = MaxHp;
            BaseMoveSpeed = DefaultMoveSpeed;
            BaseAttackSpeed = DefaultAttackSpeed;
            BaseHitTime = DefaultHitTime;
            BaseDodgeTime = DefaultDodgeTime;
            BaseGold = DefaultStartGold;
            BaseShield = DefaultShield;
        }

        /** 플레이어 데이터 - 배율 초기화 */
        private void SetToStartRate()
        {
            rateHitTime = 100;
            rateDodgeTime = 100;
            rateAttackSpeed = 100;
            rateMoveSpeed = 100;
            rateHpIncrease = 100;
            rateMaxHp = 100;
            rateGainGold = 100;
            rateGainGem = 100;
        }
        
        /** 신규 캐릭터 함수 */
        private void SetNewChara()
        {
            Gem = DefaultStartGem;
            _isNewChara = false;
        }

        public void SetWeapon(Weapon weapon)
        {
            _Equipweapon = weapon;
        }
        
    }
}

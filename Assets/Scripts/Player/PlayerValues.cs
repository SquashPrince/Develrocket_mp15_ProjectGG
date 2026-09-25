using UnityEngine;
using System;
using System.Collections.Generic;

namespace Player
{
    public class PlayerValues : MonoBehaviour, IInteractor
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
        private const int DefaultMaxShield = 0;
        
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
        public int BaseMaxShield { get; set; }
        
        // 프로퍼티 (연산X)
        // ============================================================
        public int rateHitTime; // 무적시간 배율
        public int rateDodgeTime; // 회피무적 배율
        public int rateAttackSpeed; // 공격속도 배율
        public int rateMoveSpeed { get; set; } // 이동속도 배율
        public int rateHpIncrease { get; set; } // 회복량 배율
        public int rateMaxHp; // 최종체력 배율
        public int rateGainGold { get; set; } // 골드획득 배율
        public int rateGainGem; // 보석획득 배율
        public float DamageMultiplier { get; set; } // 데미지 배율
        
        // 변수 (배율)
        // ============================================================
        public int Hp
        {
            get => BaseHp;
            set
            {
                Debug.Log(value);
                int amount = (value - BaseHp);
                int hp = amount > 0
                    ? BaseHp + (amount * rateHpIncrease)/100
                    : BaseHp + amount;
                if (hp > MaxHp) BaseHp = MaxHp;
                else if (hp < 0) BaseHp = 0;
                else BaseHp = hp;
                Debug.Log(BaseHp);
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

        public int Shield
        {
            get => BaseShield;
            set
            {
                int amount = (value - BaseShield);
                if (amount >= 0)
                {
                    BaseShield = value > BaseMaxShield ? BaseMaxShield : value;
                }
                else
                {
                    BaseShield = value < 0 ?  0 : value;
                }
            }
        }

        // 프로퍼티 (연산O)
        // ============================================================
        private static PlayerInputManager PlayerInput => PlayerInputManager.Instance;
        private PlayerItemSlot _itemSlot;
        public Transform Transform { get => _bodyTransform; }
        public bool _hasSuccessInteract;
        private bool _isNewChara = true;
        private bool _isDefaultWeapon => _currentSlot == _firstSlot;
        private int _maxWeaponSlot = 3;
        [SerializeField] private Transform _bodyTransform;
        public Weapon EquippedWeapon => _currentWeapon;

        private PlayerWeaponEnum _firstSlot;
        private PlayerWeaponEnum _secondSlot;
        private PlayerWeaponEnum _thirdSlot;

        
        private PlayerWeaponEnum _currentSlot;
        private Weapon _currentWeapon;
        private Collider _weaponCollider;
        private Dictionary<PlayerWeaponEnum, Weapon> _weaponDictionary = new();
        
        // 인스턴스 & 변수
        // ==================================================
        private void Start()
        {
            SetDefault(); 
        }

        private void Awake() => _itemSlot = GetComponent<PlayerItemSlot>();
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
            BaseMaxShield = DefaultMaxShield;
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
            DamageMultiplier = 1f;
        }
        
        /** 신규 캐릭터 함수 */
        private void SetNewChara()
        {
            Gem = DefaultStartGem;
            _isNewChara = false;
        }
        
        /// <summary>
        /// 이미 소유한 무기면 false, 아니면 true
        /// </summary>
        public bool TrySetWeapon(Weapon weapon)
        {
            return (!_weaponDictionary.ContainsValue(weapon));
        }
        
        public void SwapNextWeapon()
        {
            if (_weaponDictionary.Count<2) return;
            _currentWeapon.SetUnEquip();
            _currentSlot = (PlayerWeaponEnum)(((int)_currentSlot + 1 )%_weaponDictionary.Count);
            CurrentWeaponOff();
            _currentWeapon = _weaponDictionary[_currentSlot];
            _currentWeapon.SetEquip(Transform);
            CurrentWeaponOn();
        }
        
        public void SetWeapon(Weapon weapon)
        {
            if (_weaponDictionary.Count < _maxWeaponSlot) // 빈 무기슬롯이 있으면
            {
                if (_weaponDictionary.Count == 0) // 가진 무기가 없으면
                {
                    _currentSlot = _firstSlot; // 현재 슬롯 = 1번째
                    _currentWeapon = weapon; // 현재무기 = 집어든 무기
                    _currentWeapon.SetEquip(Transform); // 현재 무기 장착
                    AddDictionary(_firstSlot, weapon); // 1번 슬롯에 집어든 무기 추가
                    _hasSuccessInteract = true; // 상호작용 성공
                    return ;
                }
                AddDictionary((PlayerWeaponEnum)_weaponDictionary.Count, weapon); // 딕셔너리 맨 앞쪽 빈 슬롯에 무기 추가
                weapon.gameObject.SetActive(false); // 무기 오브젝트 비활성화
                _hasSuccessInteract = true; // 상호작용 성공
                return;
            }
            // if (_isDefaultWeapon) return; // 기본무기 들고 있을 시 return
            _currentWeapon.SetUnEquip(); // 기존 무기 해제
            RemoveDictionary(_currentSlot); // 현재 슬롯에서 기존무기 해제
            
            _currentWeapon = weapon; // // 현재 무기를 집어든 무기로 변경
            AddDictionary(_currentSlot,_currentWeapon); // 집어든 무기 현재 슬롯에 추가
            _currentWeapon.SetEquip(Transform); // 현재 무기 장착
            CurrentWeaponOn(); // 무기 오브젝트 활성화
            _hasSuccessInteract = true; // 상호작용 성공
        }

        private void AddDictionary(PlayerWeaponEnum slot, Weapon weapon)
        {
            _weaponDictionary.Add(slot,weapon);
            WeaponColliderOff();
        }
        
        private void RemoveDictionary(PlayerWeaponEnum slot)
        {
            _weaponDictionary.Remove(slot);
            WeaponColliderOn();
        }
        

        private void WeaponColliderOff()
        {
            _weaponCollider = _currentWeapon.gameObject.GetComponentInChildren<Collider>();
            _weaponCollider.enabled = false;
        }
        private void WeaponColliderOn()
        {
            _weaponCollider = _currentWeapon.gameObject.GetComponentInChildren<Collider>();
            _weaponCollider.enabled = true;
        }

        private void CurrentWeaponOff()
        {
            _currentWeapon.gameObject.SetActive(false);
            
        }
        private void CurrentWeaponOn()
        {
            _currentWeapon.gameObject.SetActive(true);
        }
        
        public void SetItem(Item item, PlayerItemEnum slot)
        {
            _itemSlot.SetItem(item, slot);
        }

        public bool CanInteractItem(PlayerItemEnum targetslot)
        
        {
            return _itemSlot.CanInteract(targetslot);
        }
    }
}

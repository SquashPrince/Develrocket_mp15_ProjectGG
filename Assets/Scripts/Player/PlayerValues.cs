using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

namespace Player
{
    public class PlayerValues : MonoBehaviour, IInteractor
    {
        private const float DefaultHitTime = 3f; // 피격무적시간
        private const float DefaultDodgeTime = 0.5f; // 회피무적시간
        private const float DefaultAttackSpeed = 0.5f; // 공격속도
        private const float DefaultMinAttackSpeed = 0.05f; // 최소공격속도
        private const int DefaultMoveSpeed = 5; // 이동속도
        private const int DefaultMinMoveSpeed = 1; // 최소이동속도
        private const int DefaultMaxHp = 10; // 최대HP
        private const int DefaultMaxShield = 10; // 최대HP
        private const int DefaultStartGold = 100; // 시작골드
        private const int DefaultStartGem = 0; // 시작보석
        private const int DefaultShield = 0; // 보호막
        
        // 상수 (기본값)
        // ============================================================
        public float DodgeCoolDown {  get; set; }
        public float BaseHitTime { get; set; }
        public float BaseDodgeTime { get; set; }
        public float BaseAttackSpeed { get; set; }
        public int BaseMoveSpeed { get; set; }
        public int BaseMaxHp { get; set; }
        public int BaseGold {get; set;}
        public int BaseGem { get; set; }
        public int BaseHp { get; set; }
        private int _baseShield;
        public int BaseShield
        {
            get => _baseShield;
            set => _baseShield = Mathf.Clamp(value, 0, Mathf.Max(0, BaseMaxShield));
        }
        private int _damageShieldCharges;
        public int DamageShieldCharges
        {
            get => _damageShieldCharges;
            set => _damageShieldCharges = Mathf.Clamp(value, 0, 1);
        }
        public bool TryConsumeDamageShield()
        {
            if (DamageShieldCharges == 0) return false;
            DamageShieldCharges = 0;
            return true;
        }
        private int _baseMaxShield;
        public int BaseMaxShield
        {
            get => _baseMaxShield;
            set
            {
                _baseMaxShield = Mathf.Max(0, value);
                _baseShield = Mathf.Min(_baseShield, _baseMaxShield);
            }
        }
        
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
        public bool _canWeaponSwap = true;
        private bool _isNewChara = true;
        private int _maxWeaponSlot = 3;
        [SerializeField] private Transform _bodyTransform;
        [SerializeField] private Transform _weaponTransform;
        public Weapon EquippedWeapon => _currentWeapon;
        public PlayerItemSlot ItemSlots => _itemSlot;
        public int CurrentMagazine => _currentWeapon != null ? _currentWeapon.CurrentMagazine : 0;
        public int MaxMagazine => _currentWeapon != null ? _currentWeapon.MaxMagazine : 0;
        // UI는 구독 후 현재 프로퍼티로 최초 표시하고 이후 이벤트로 갱신한다.
        public event Action<int, int> OnAmmoChanged;
        private Weapon _lastAmmoWeapon;
        private int _lastCurrentMagazine = -1;
        private int _lastMaxMagazine = -1;

        private void LateUpdate()
        {
            int current = CurrentMagazine;
            int maximum = MaxMagazine;
            if (_lastAmmoWeapon == _currentWeapon && _lastCurrentMagazine == current
                && _lastMaxMagazine == maximum) return;
            _lastAmmoWeapon = _currentWeapon;
            _lastCurrentMagazine = current;
            _lastMaxMagazine = maximum;
            OnAmmoChanged?.Invoke(current, maximum);
        }
        public bool IsReloading => _currentWeapon != null
            && _currentWeapon.isActiveAndEnabled && _currentWeapon.IsReloading;
        public float ReloadProgress => IsReloading ? _currentWeapon.ReloadProgress : 0f;

        private PlayerWeaponEnum _firstSlot;
        private PlayerWeaponEnum _secondSlot;
        private PlayerWeaponEnum _thirdSlot;
        private PlayerTalent _playerTalent;

        
        private PlayerWeaponEnum _currentSlot;
        private Weapon _currentWeapon;
        private Collider _weaponCollider;
        private Dictionary<PlayerWeaponEnum, Weapon> _weaponDictionary = new();

        public PlayerWeaponEnum CurrentSlot => _currentSlot;
        public Dictionary<PlayerWeaponEnum, Weapon> WeaponDictionary => _weaponDictionary;
        
        // 인스턴스 & 변수
        // ==================================================
        private void Start()
        {
            SetDefault();
        }

        private void Awake()
        {
            _itemSlot = GetComponent<PlayerItemSlot>();
            _playerTalent = GetComponent<PlayerTalent>();
        }

        // 이벤트 함수
        // ==================================================
        
        /** 시작시 플레이어 데이터를 초기화 하는 함수 */
        private void SetDefault()
        {
            SetToStartRate();
            SetToStartValues();
            if (_isNewChara) SetNewChara();
            else _playerTalent.TalentsLoad();
        }
        
        /** 플레이어 데이터 - 값 초기화 */
        private void SetToStartValues()
        {
            BaseMaxHp = DefaultMaxHp;
            BaseMaxShield = DefaultMaxShield;
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
            DamageMultiplier = 1f;
            DodgeCoolDown = 2f;
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
        
        public void SwapNextWeapon(int direction)
        {
            if (_weaponDictionary.Count<2 || !_canWeaponSwap) return;
            _currentWeapon.SetUnEquip();

            int nextSlotNum = (int)_currentSlot + -direction;
            nextSlotNum = nextSlotNum < 0 ? _weaponDictionary.Count - 1 : nextSlotNum;

            _currentSlot = (PlayerWeaponEnum)(nextSlotNum % _weaponDictionary.Count);
            CurrentWeaponOff();
            _currentWeapon = _weaponDictionary[_currentSlot];
            _currentWeapon.SetEquip(_weaponTransform);
            UIManager.Instance.Window.WeaponSwap(direction < 0);
            CurrentWeaponOn();

            StartCoroutine(WeaponSwapCoolDownRoutine());
        }
        
        private IEnumerator WeaponSwapCoolDownRoutine()
        {
            _canWeaponSwap = false;

            yield return new WaitForSeconds(1f);

            _canWeaponSwap = true;
        }

        public void SetWeapon(Weapon weapon)
        {
            if (_weaponDictionary.Count < _maxWeaponSlot) // 빈 무기슬롯이 있으면
            {
                if (_weaponDictionary.Count == 0) // 가진 무기가 없으면
                {
                    _currentSlot = _firstSlot; // 현재 슬롯 = 1번째
                    _currentWeapon = weapon; // 현재무기 = 집어든 무기
                    _currentWeapon.SetEquip(_weaponTransform); // 현재 무기 장착
                    AddDictionary(_firstSlot, weapon); // 1번 슬롯에 집어든 무기 추가
                    UIManager.Instance.Window.TakeWeapon();
                    _hasSuccessInteract = true; // 상호작용 성공
                    return ;
                }
                AddDictionary((PlayerWeaponEnum)_weaponDictionary.Count, weapon); // 딕셔너리 맨 앞쪽 빈 슬롯에 무기 추가
                UIManager.Instance.Window.TakeWeapon();
                weapon.gameObject.SetActive(false); // 무기 오브젝트 비활성화
                _hasSuccessInteract = true; // 상호작용 성공
                return;
            }
            _currentWeapon.SetUnEquip(); // 기존 무기 해제
            RemoveDictionary(_currentSlot); // 현재 슬롯에서 기존무기 해제
            
            _currentWeapon = weapon; // // 현재 무기를 집어든 무기로 변경
            AddDictionary(_currentSlot,_currentWeapon); // 집어든 무기 현재 슬롯에 추가
            UIManager.Instance.Window.TakeWeapon();
            _currentWeapon.SetEquip(_weaponTransform); // 현재 무기 장착
            CurrentWeaponOn(); // 무기 오브젝트 활성화
            _hasSuccessInteract = true; // 상호작용 성공
        }

        private void AddDictionary(PlayerWeaponEnum slot, Weapon weapon)
        {
            WeaponColliderOff();
            _weaponDictionary.Add(slot,weapon);
        }
        
        private void RemoveDictionary(PlayerWeaponEnum slot)
        {
            WeaponColliderOn();
            _weaponDictionary.Remove(slot);
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
            TrySetItem(item, slot);
        }

        public bool TrySetItem(Item item, PlayerItemEnum slot)
        {
            return _itemSlot != null && _itemSlot.TrySetItem(item, slot);
        }

        public bool CanInteractItem(PlayerItemEnum targetslot)
        
        {
            return _itemSlot != null && _itemSlot.CanInteract(targetslot);
        }
    }
}

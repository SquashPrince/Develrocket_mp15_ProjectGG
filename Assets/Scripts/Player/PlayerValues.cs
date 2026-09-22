using UnityEngine;

namespace Player
{
    public interface IInteracter
    {
        // 임시 인터페이스, PR시 삭제
    }
    
    public class PlayerValues : MonoBehaviour, IInteracter
    {
        private PlayerInteractor _interactor;
        private PlayerValueStruct _value;
        
        // 인스턴스 & 변수
        // ==================================================
        
        private void Awake()
        {
            CacheComponents();
        }
        
        // 이벤트 함수
        // ==================================================

        private void CacheComponents()
        {
            SetValue();
            _interactor = GetComponent<PlayerInteractor>();
            // 델리게이트로 특성 불러오기
        }

        private void SetValue()
        {
            _value = new PlayerValueStruct();
            _value.SetDefaultValues();
        }
        
        // 내부 함수
        // ==================================================

        
        /** 현재체력 변경 */
        public void AddCurrentHp(int hp)
        {
            _value.CurrentHealth += hp;
        }
        
        /** 최대체력 변경 */
        public void AddMaxHp(int maxHp)
        {
            _value.MaxHealth += maxHp;
        }

        /** 이동속도 변경 */
        public void AddMoveSpeed(int moveSpeed)
        {
            _value.MoveSpeed += moveSpeed;
        }

        /** 공격주기 변경 */
        public void AddAttackSpeed(int attackSpeed)
        {
            _value.AttackSpeedRate += attackSpeed;
        }

        /** 피격무적시간 변경 */
        public void AddDamageDelay(float damageDelay)
        {
            _value.DamagedDelay +=  damageDelay;
        }

        /** 회피무적시간 변경 */
        public void AddDodgeTime(float dodgeTime)
        {
            _value.DodgeTime += dodgeTime;
        }

        /** 소지골드 변경 */
        public void AddGold(int gold)
        {
            if (gold < 0 || gold == gold * _value.GoldGainRate)
            {
                _value.Gold += gold;
            }
            else
            {
                _value.Gold += gold * _value.GoldGainRate / 100;
            }
        }

        /** 소지보석 변경 */
        public void AddGem(int gem)
        {
            _value.Gem += gem;
        }
        
        /** 골드 획득량 변경 */
        public void AddGoldGainRate(int goldGainRate)
        {
            _value.GoldGainRate += goldGainRate;
        }

        /** 쉴드량 변경 */
        public void AddShield(int shield)
        {
            _value.Shield += shield;
        }

        public void AddAttackDamage(int damage)
        {
            _value.AttackDamage += damage;
        }
        // 퍼블릭 메서드
        // ==================================================
    }
    
}

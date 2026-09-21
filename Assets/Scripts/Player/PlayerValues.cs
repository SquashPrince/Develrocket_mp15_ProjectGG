using UnityEngine;

namespace Player
{
    
    
    public class PlayerValues : MonoBehaviour
    {
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
            _value = new PlayerValueStruct();
            _value.SetDefaultValues();
            // 델리게이트로 특성 불러오기
        }
        
        // 내부 함수
        // ==================================================

        
        /** 현재체력 변경 */
        public void SetCurrentHp(int hp)
        {
            _value.CurrentHealth += hp;
        }
        
        /** 최대체력 변경 */
        public void SetMaxHp(int maxHp)
        {
            _value.MaxHealth += maxHp;
        }

        /** 이동속도 변경 */
        public void SetMoveSpeed(int moveSpeed)
        {
            _value.MoveSpeed += moveSpeed;
        }

        /** 공격주기 변경 */
        public void SetAttackRate(float attackRate)
        {
            _value.AttackRate += attackRate;
        }

        /** 피격무적시간 변경 */
        public void SetDamageDelay(float damageDelay)
        {
            _value.DamagedDelay +=  damageDelay;
        }

        /** 회피무적시간 변경 */
        public void SetDodgeTime(float dodgeTime)
        {
            _value.DodgeTime += dodgeTime;
        }

        /** 소지골드 변경 */
        public void SetGold(int gold)
        {
            _value.Gold += gold;
        }

        /** 소지보석 변경 */
        public void SetGem(int gem)
        {
            _value.Gem += gem;
        }
        
        // 퍼블릭 메서드
        // ==================================================
    }
    
}

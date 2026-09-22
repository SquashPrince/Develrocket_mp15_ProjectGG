using UnityEngine;

namespace Player
{
    public interface IInteracter
    {
        // 임시 인터페이스, PR시 삭제
    }
    public interface IInteractable
    {
        // 임시 인터페이스, PR시 삭제
    }
    public class Weapon
    {
        // 임시 클래스, PR시 삭제
    }
    
    public partial class PlayerValues : MonoBehaviour, IInteracter
    {
        private IInteractable _interactable;
        private Transform _weaponTransform;
        private Weapon _weaponEquip;

        private bool _isNewChara = true;
        
        
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
            
        }

        private void SetDefault()
        {
            SetToStartValues();
            SetToStartRate();
            if (_isNewChara) SetNewChara();
            // 델리게이트로 특성 불러오기
        }
        
        /** 기본값 설정 */
        public void SetToStartValues()
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

        public void SetToStartRate()
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

        public void SetNewChara()
        {
            Gem = DefaultStartGem;
            _isNewChara = false;
        }
        
        // 내부 함수
        // ==================================================
        
        
        
        // ==================================================
    }
    
}

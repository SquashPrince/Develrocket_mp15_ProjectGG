using UnityEngine;

namespace Player
{
    public abstract class PlayerTalent
    { 
        public abstract void SetTalentData(PlayerValues playerValues);
    }
    
    // 특성 상속클래스
    // ==================================================

    public class IncreaseGainGold : PlayerTalent
    {
        public override void SetTalentData(PlayerValues playerValues)
        {
            playerValues.AddGoldGainRate(20);
        }
    }
    public class IncreaseAtk : PlayerTalent
    {
        public override void SetTalentData(PlayerValues playerValues)
        {
            playerValues.AddAttackDamage(1);
        }
    }
    public class IncreaseHp : PlayerTalent
    {
        public override void SetTalentData(PlayerValues playerValues)
        {
            playerValues.AddMaxHp(1);
        }
    }
    public class IncreaseMoveSpeed : PlayerTalent
    {
        public override void SetTalentData(PlayerValues playerValues)
        {
            playerValues.AddMoveSpeed(1);
        }
    }
    public class IncreaseAttackSpeed : PlayerTalent
    {
        public override void SetTalentData(PlayerValues playerValues)
        {
            playerValues.AddAttackSpeed(20);
        }
    }
    // 특성
}

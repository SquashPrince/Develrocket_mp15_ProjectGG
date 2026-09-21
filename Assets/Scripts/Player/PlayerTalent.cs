using UnityEngine;

namespace Player
{
    public abstract class PlayerTalent
    { 
        public abstract void SetTalentData(PlayerValues playerValues);
    }

    public class IncreaseGainGold : PlayerTalent
    {
        public override void SetTalentData(PlayerValues playerValues)
        {
            playerValues.SetGoldGainRate(20);
        }
    }
    
    
    
    
}

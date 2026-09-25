using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class PlayerTalent : MonoBehaviour
{
    private PlayerValues _playerValues;
    private List<PlayerTalentEnum> _activeTalents = new();
    
    private void Awake() => _playerValues = GetComponent<PlayerValues>();

    
    /// <summary>
    /// 특성과 가격을 입력하면 가격이상의 보석을 소유 중일때 보석을 차감하여 특성획득
    /// </summary>
    /// <param name="특성enum "></param>
    /// <param name="특성가격"></param>
    public void GetNewTalent(PlayerTalentEnum talent, int gemPrice)
    {
        if (gemPrice > _playerValues.Gem) return;
        _playerValues.Gem -= gemPrice;
        _activeTalents.Add(talent);
    }

    /// <summary>
    /// 특성리스트를 읽어와 보유한 특성대로 스탯을 증감시킴
    /// </summary>
    public void TalentsLoad()
    {
        foreach (PlayerTalentEnum talent in _activeTalents)
        {
            SetTalent(talent);
        }
    }
    
    /// <summary>
    /// 특성 적용
    /// </summary>
    /// <param name="특성"></param>
    private void SetTalent(PlayerTalentEnum talent)
    {
        
        switch (talent)
        {
            case PlayerTalentEnum.IncreaseGoldRate:
                _playerValues.rateGainGold += 50;
                break;
            case PlayerTalentEnum.IncreaseShield:
                _playerValues.BaseMaxShield += 2;
                break;
            case PlayerTalentEnum.IncreaseMaxHp:
                _playerValues.BaseMaxHp += 4;
                break;
            case PlayerTalentEnum.IncreaseDamageMultiplier:
                _playerValues.DamageMultiplier += 0.5f;
                break;
            case PlayerTalentEnum.IncreaseMoveSpeed:
                _playerValues.rateMoveSpeed += 20;
                break;
            case PlayerTalentEnum.IncreaseAllStats:
                _playerValues.BaseMaxHp += 2;
                _playerValues.BaseMaxShield += 2;
                _playerValues.BaseHitTime += 0.5f;
                _playerValues.BaseDodgeTime += 0.2f;
                _playerValues.BaseGold += 100;
                _playerValues.rateMoveSpeed += 20;
                _playerValues.rateAttackSpeed += 20;
                break;
        }
    }
}
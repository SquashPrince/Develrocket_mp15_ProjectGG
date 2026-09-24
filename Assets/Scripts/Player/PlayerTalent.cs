using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class PlayerTalent : MonoBehaviour
{
    [SerializeField] private PlayerValues _playerValues;
    private List<PlayerTalentEnum> _activeTalents = new();
    
    // private void Awake() => _playerValues = GetComponent<PlayerValues>;

    public void GetNewTalent(PlayerTalentEnum talent)
    {
        _activeTalents.Add(talent);
    }

    public void TalentsLoad()
    {
        foreach (PlayerTalentEnum talent in _activeTalents)
        {
            SetTalent(talent);
        }
    }
    private void SetTalent(PlayerTalentEnum talent)
    {
        
        /*switch (talent)
        {
            case PlayerTalentEnum.gold:
                _playerValues.rateGainGold += 50;
                break;
            case PlayerTalentEnum.shield:
                _playerValues.BaseMaxShield += 2;
                break;
            case PlayerTalentEnum.hp:
                _playerValues.BaseMaxHp += 4;
                break;
            case PlayerTalentEnum.damage:
                _playerValues.DamageMultiplier += 0.5f;
                break;
        }
        */
        
    }
}

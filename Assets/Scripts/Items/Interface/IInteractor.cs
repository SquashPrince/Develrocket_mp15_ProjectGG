using UnityEngine;

public interface IInteractor
{

    /// <summary>
    /// 부적이나 버프가 플레이어에게 붙어있게 하기 위헤 선언
    /// </summary>
    Transform Transform { get; }
    /// <summary>
    /// 현재 장착중인 무기
    /// </summary>
    Weapon EquippedWeapon { get; }

    // TODO: PlayerValues에 기본값 1f인 공격력 배율을 구현. 무기 교체 시 유지.
    // 예시) 공격력 효과 5는 배율에 0.05f를 더함.
    float DamageMultiplier { get; set; }

    bool TrySetWeapon(Weapon weapon);
    void SetWeapon(Weapon weapon) { }
    bool CanInteractItem(PlayerItemEnum targetSlot);
    void SetItem(Item item, PlayerItemEnum targetSlot);

    bool TrySetItem(Item item, PlayerItemEnum targetSlot);
    int DamageShieldCharges { get; set; }
    bool TryConsumeDamageShield();
    int Shield { get; set; }
    int Hp { get; set; }
    int BaseShield { get; set; }
    int BaseMaxHp { get; set; }

    // TODO: PlayerValues에 최대 방어막 수 BaseMaxShield 프로퍼티 구현 필요.
    int BaseMaxShield { get; set; }

    // TODO: PlayerValues의 동명 필드를 프로퍼티로 변경 필요
    int rateMoveSpeed { get; set; }
    int rateGainGold { get; set; }
}

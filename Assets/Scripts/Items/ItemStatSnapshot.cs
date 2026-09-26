using System.Text;
using UnityEngine;

// 실제 적용 전후 값을 비교하므로 회복/보호막 상한도 로그에 반영된다.
public readonly struct ItemStatSnapshot
{
    private readonly int _hp, _shield, _damageShield, _baseMaxHp, _maxShield, _moveRate, _goldRate;
    private readonly float _damageMultiplier;

    public ItemStatSnapshot(IInteractor owner)
    {
        _hp = owner.Hp;
        _shield = owner.Shield;
        _damageShield = owner.DamageShieldCharges;
        _baseMaxHp = owner.BaseMaxHp;
        _maxShield = owner.BaseMaxShield;
        _moveRate = owner.rateMoveSpeed;
        _goldRate = owner.rateGainGold;
        _damageMultiplier = owner.DamageMultiplier;
    }

    public void LogChanges(IInteractor owner, string itemName, Object context)
    {
        var changes = new StringBuilder();
        Append(changes, "HP", _hp, owner.Hp);
        Append(changes, "보호막", _shield, owner.Shield);
        Append(changes, "1회 무효화 쉴드", _damageShield, owner.DamageShieldCharges);
        Append(changes, "기본 최대 HP", _baseMaxHp, owner.BaseMaxHp);
        Append(changes, "최대 보호막", _maxShield, owner.BaseMaxShield);
        Append(changes, "이동속도 배율(%)", _moveRate, owner.rateMoveSpeed, "%p");
        Append(changes, "골드 획득 배율(%)", _goldRate, owner.rateGainGold, "%p");
        Append(changes, "공격력 배율", _damageMultiplier, owner.DamageMultiplier);
        Debug.Log($"[아이템 사용: {itemName}] " +
            (changes.Length > 0 ? changes.ToString() : "스탯 변화 없음 (상한 도달 또는 비스탯 효과)"), context);
    }

    private static void Append(StringBuilder text, string name, float before, float after, string unit = "")
    {
        if (Mathf.Approximately(before, after)) return;
        if (text.Length > 0) text.Append(" / ");
        float delta = after - before;
        text.Append($"{name}: {before:0.###} → {after:0.###} ({(delta > 0 ? "+" : "")}{delta:0.###}{unit})");
    }
}

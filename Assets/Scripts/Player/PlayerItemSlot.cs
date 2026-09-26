using System;
using Player;
using UnityEngine;

public readonly struct PlayerItemSlotState
{
    public PlayerItemEnum Slot { get; }
    public Item Item { get; }
    public Sprite Icon { get; }
    public int Count { get; }
    public float CooldownDuration { get; }
    public float CooldownRemaining { get; }
    public bool CanUse { get; }

    public PlayerItemSlotState(PlayerItemEnum slot, Item item, int count, float duration, float remaining, bool canUse)
    {
        Slot = slot;
        Item = item;
        Icon = item != null ? item.Icon : null;
        Count = count;
        CooldownDuration = duration;
        CooldownRemaining = remaining;
        CanUse = canUse;
    }
}

public class PlayerItemSlot : MonoBehaviour
{
    private PlayerInputManager _input;
    private PlayerValues _playerValues;
    private const int MaxItems = 3;
    private int _primaryCount;
    private int _secondaryCount;
    [SerializeField] private Item[] _primaryList;
    [SerializeField] private Item[] _secondaryList;
    [SerializeField] private Item _singleUseItem;
    private readonly float[] _cooldownEnds = new float[2];
    private readonly float[] _cooldownDurations = new float[2];
    private readonly bool[] _cooldownActive = new bool[2];
    public event Action<PlayerItemSlotState> OnSlotChanged;

    private void Awake()
    {
        _playerValues = GetComponent<PlayerValues>();
        _input = GetComponent<PlayerInputManager>();
        _primaryList = new Item[MaxItems];
        _secondaryList = new Item[MaxItems];
    }

    private void OnEnable()
    {
        if (_input == null) return;
        _input.OnItem1 += UsePrimary;
        _input.OnItem2 += UseSecondary;
        _input.OnItem3 += UseThird;
    }

    private void OnDisable()
    {
        if (_input == null) return;
        _input.OnItem1 -= UsePrimary;
        _input.OnItem2 -= UseSecondary;
        _input.OnItem3 -= UseThird;
    }

    private void Update()
    {
        // UI의 자체 타이머 없이 같은 남은 시간을 전달한다. 일시정지 중에는 진행하지 않는다.
        if (Time.timeScale <= 0f) return;
        for (int i = 0; i < 2; i++)
        {
            if (!_cooldownActive[i]) continue;
            if (Time.time >= _cooldownEnds[i]) _cooldownActive[i] = false;
            Notify((PlayerItemEnum)i);
        }
    }

    private void UsePrimary() => TryUseItem(PlayerItemEnum.Primary);
    private void UseSecondary() => TryUseItem(PlayerItemEnum.Secondary);
    private void UseThird() => TryUseItem(PlayerItemEnum.SingleUse);

    // 조회는 아이템 개수나 쿨다운을 변경하지 않는다.
    public bool CanInteract(PlayerItemEnum slot)
    {
        switch (slot)
        {
            case PlayerItemEnum.Primary: return _primaryCount < MaxItems;
            case PlayerItemEnum.Secondary: return _secondaryCount < MaxItems;
            case PlayerItemEnum.SingleUse: return _singleUseItem == null;
            default: return false;
        }
    }

    public void SetItem(Item item, PlayerItemEnum slot) => TrySetItem(item, slot);

    public bool TrySetItem(Item item, PlayerItemEnum slot)
    {
        if (item == null || !CanInteract(slot)) return false;
        if (Array.IndexOf(_primaryList, item) >= 0 || Array.IndexOf(_secondaryList, item) >= 0
            || _singleUseItem == item) return false;
        switch (slot)
        {
            case PlayerItemEnum.Primary: _primaryList[_primaryCount++] = item; break;
            case PlayerItemEnum.Secondary: _secondaryList[_secondaryCount++] = item; break;
            case PlayerItemEnum.SingleUse: _singleUseItem = item; break;
            default: return false;
        }
        Notify(slot);
        return true;
    }

    public PlayerItemSlotState GetSlotState(PlayerItemEnum slot)
    {
        Item item = null;
        int count = 0;
        switch (slot)
        {
            case PlayerItemEnum.Primary:
                count = _primaryCount;
                item = count > 0 ? _primaryList[count - 1] : null;
                break;
            case PlayerItemEnum.Secondary:
                count = _secondaryCount;
                item = count > 0 ? _secondaryList[count - 1] : null;
                break;
            case PlayerItemEnum.SingleUse:
                item = _singleUseItem;
                count = item != null ? 1 : 0;
                break;
        }
        int index = (int)slot;
        float remaining = index >= 0 && index < 2 ? Mathf.Max(0f, _cooldownEnds[index] - Time.time) : 0f;
        float duration = index >= 0 && index < 2 ? _cooldownDurations[index] : 0f;
        return new PlayerItemSlotState(slot, item, count, duration, remaining,
            isActiveAndEnabled && item != null && count > 0 && remaining <= 0f && Time.timeScale > 0f);
    }

    public bool TryUseItem(PlayerItemEnum slot)
    {
        var state = GetSlotState(slot);
        if (!state.CanUse || _playerValues == null) return false;

        // 성공한 사용만 소모한다. 다른 슬롯의 타이머에는 영향을 주지 않는다.
        switch (slot)
        {
            case PlayerItemEnum.Primary: _primaryList[--_primaryCount] = null; break;
            case PlayerItemEnum.Secondary: _secondaryList[--_secondaryCount] = null; break;
            case PlayerItemEnum.SingleUse: _singleUseItem = null; break;
            default: return false;
        }
        int index = (int)slot;
        if (index < 2)
        {
            float duration = state.Item is ActiveItem active ? active.Cooldown : 0f;
            _cooldownDurations[index] = duration;
            _cooldownEnds[index] = Time.time + duration;
            _cooldownActive[index] = duration > 0f;
        }
        state.Item.Use(_playerValues);
        Notify(slot);
        return true;
    }

    private void Notify(PlayerItemEnum slot) => OnSlotChanged?.Invoke(GetSlotState(slot));
}

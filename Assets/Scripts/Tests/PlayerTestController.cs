using UnityEngine;
using Player;

// 기존 플레이어 코드를 바꾸지 않고 아이템 계약과 조작을 검증하는 전용 컴포넌트.
[DefaultExecutionOrder(100)]
[RequireComponent(typeof(PlayerValues), typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerTestController : MonoBehaviour, IInteractor
{
    [SerializeField] private Transform _weaponTransform;
    [SerializeField] private Weapon _startingWeapon;
    [SerializeField] private float _interactRadius = 1.8f;
    [SerializeField] private bool _showStatus = true;

    private PlayerValues _values;
    private Rigidbody _body;
    private bool _previousCameraEnabled;
    private Vector3 _direction;
    private bool _ready;
    private int _maxShield = 3;
    private string _lastItem = "없음";

    public Transform Transform => transform;
    public Weapon EquippedWeapon { get; private set; }
    public float DamageMultiplier { get; set; } = 1f;
    public int Hp { get => _values.Hp; set => _values.Hp = value; }
    public int BaseShield { get => _values.BaseShield; set => _values.BaseShield = Mathf.Clamp(value, 0, BaseMaxShield); }
    public int BaseMaxHp { get => _values.BaseMaxHp; set => _values.BaseMaxHp = Mathf.Max(1, value); }
    public int BaseMaxShield { get => _maxShield; set => _maxShield = Mathf.Max(0, value); }
    public int rateMoveSpeed { get => _values.rateMoveSpeed; set => _values.rateMoveSpeed = value; }
    public int rateGainGold { get => _values.rateGainGold; set => _values.rateGainGold = value; }

    private void Awake()
    {
        _values = GetComponent<PlayerValues>();
        _body = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        // PlayerValues.Start 이후 테스트 체력을 초기화해 기존 초기화 순서의 영향을 피함.
        _values.BaseHp = _values.MaxHp;

        if (_startingWeapon != null) SetWeapon(Instantiate(_startingWeapon));
        _ready = true;
    }

    private void Update()
    {
        if (!_ready) return;
        _direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;
        if (Input.GetKeyDown(KeyCode.E)) InteractNearest();
        if (Input.GetMouseButton(0) && EquippedWeapon != null)
            EquippedWeapon.Fire(DamageMultiplier);
        if (Input.GetKeyDown(KeyCode.H)) Hp -= 1;
    }

    private void FixedUpdate()
    {
        if (!_ready) return;
        // 배율 변화가 작은 경우에도 테스트 이동에 소수 속도를 반영.
        float speed = Mathf.Max(1f, _values.BaseMoveSpeed * rateMoveSpeed / 100f);
        _body.velocity = _direction * speed;
    }

    private void InteractNearest()
    {
        Item nearest = null;
        float best = float.PositiveInfinity;
        // 기존 무기 프리팹에는 콜라이더가 없어, E키를 누를 때만 주변 아이템을 직접 검색.
        foreach (Item item in FindObjectsOfType<Item>())
        {
            if (item == null || !item.CanInteract || item.transform.IsChildOf(transform)) continue;
            float distance = (item.transform.position - transform.position).sqrMagnitude;
            if (distance > _interactRadius * _interactRadius || distance >= best) continue;
            nearest = item;
            best = distance;
        }
        if (nearest == null) return;
        _lastItem = nearest.Name;
        nearest.Interact(this);
    }

    public bool TrySetWeapon(Weapon weapon) => weapon != null && _weaponTransform != null;

    public void SetWeapon(Weapon weapon)
    {
        if (!TrySetWeapon(weapon) || EquippedWeapon == weapon) return;
        if (EquippedWeapon != null)
        {
            EquippedWeapon.SetUnEquip();
            EquippedWeapon.transform.position = transform.position - transform.forward * 1.5f;
        }
        EquippedWeapon = weapon;
        _values.SetWeapon(weapon);
        weapon.SetEquip(_weaponTransform);
    }

    private void OnDisable()
    {
        if (_body != null) _body.velocity = Vector3.zero;
    }

    private void OnGUI()
    {
        if (!_ready || !_showStatus) return;
        GUI.Box(new Rect(10, 10, 420, 160), "플레이어 테스트");
        GUI.Label(new Rect(22, 38, 400, 24), "WASD 이동 / 마우스 조준 / 좌클릭 발사 / E 획득");
        GUI.Label(new Rect(22, 62, 400, 24), $"HP {Hp}/{_values.MaxHp} / 방어막 {BaseShield}/{BaseMaxShield} / H: 체력 -1");
        GUI.Label(new Rect(22, 86, 400, 24), $"공격력 배율 {DamageMultiplier:F3} / 이동 배율 {rateMoveSpeed}% / 골드 {rateGainGold}%");
        GUI.Label(new Rect(22, 110, 400, 24), $"무기: {(EquippedWeapon != null ? EquippedWeapon.Name : "없음")}");
        GUI.Label(new Rect(22, 134, 400, 24), $"최근 상호작용: {_lastItem}");
    }
}

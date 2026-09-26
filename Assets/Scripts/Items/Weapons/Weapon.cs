using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : Item, IAttackable
{
    [Header("무기 설정")]
    [SerializeField] protected GunBullet _bullet;
    [SerializeField] protected ObjectPool<GunBullet> _gunbullet;
    [SerializeField] protected Transform _muzzle;
    [SerializeField] protected EWeaponType _EWeaponType;

    [SerializeField] protected int _damage;
    [SerializeField] protected int _maxMagazine;
    [SerializeField] protected float _fireTime;
    [SerializeField] protected float _attackRange;
    [SerializeField] protected float _bulletSpeed;
    [SerializeField] protected float _reloadDelay;

    private int _shotDamage;
    private float _elapseTime;
    private bool _isCanFire => _elapseTime >= _fireTime && _currentMagazine > 0;

    protected GunBullet[] bullets;
    protected int _currentMagazine;
    protected bool _canReload;

    public EWeaponType EWeaponType => _EWeaponType;
    public int CurrentMagazine => _currentMagazine;
    public int MaxMagazine => _maxMagazine;
    private float _reloadStartedAt;
    public bool IsReloading => !CanInteract && !_canReload;
    public float ReloadProgress => _reloadDelay > 0f
        ? Mathf.Clamp01((Time.time - _reloadStartedAt) / _reloadDelay)
        : 1f;

    protected virtual void Awake() => CacheComponent();
    private void Start() => InitBullets();
    private void Update()
    {
        if (CanInteract) return;

        Tick();
        Reload();
    }

    private void Tick()
    {
        if (_isCanFire) return;

        _elapseTime += Time.deltaTime;
    }

    // TODO: 플레이어가 클릭 입력을 읽고 장착 무기의 Fire(DamageMultiplier)를 호출.
    // 연사는 버튼을 누르는 동안 호출하며, 발사 간격과 탄창 검사는 무기에서 처리.
    public void Fire(float damageMultiplier)
    {
        if (CanInteract || _gunbullet == null || !_isCanFire) return;

        // 풀에서 꺼낼 때 즉시 발사되므로, 먼저 이번 탄환의 피해량을 확정.
        _shotDamage = Mathf.Max(0, Mathf.RoundToInt(_damage * damageMultiplier));
        _bullet = _gunbullet.Pop();
        _currentMagazine--;
        _elapseTime = 0f;
    }

    protected virtual void Reload()
    {
        if (_currentMagazine > 0) return;

        if (!_canReload) return;

        StartCoroutine(ReloadRoutine());
    }
    protected IEnumerator ReloadRoutine()
    {
        _reloadStartedAt = Time.time;
        _canReload = false;

        yield return new WaitForSeconds(_reloadDelay);

        _currentMagazine = _maxMagazine;

        _canReload = true;
    }

    public override void Interact(IInteractor owner)
    {
        if (!CanInteract || owner == null) return;
        if (!owner.TrySetWeapon(this)) return;
        owner.SetWeapon(this);
        CanInteract = false;
    }

    /// <summary>
    /// 장착 메서드
    /// </summary>
    public virtual void SetEquip(Transform equipTR)
    {
        Debug.Log($"{Name} 장착됨");
        CanInteract = false;

        transform.SetParent(equipTR, false);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    /// <summary>
    /// 장착 해제 메서드
    /// </summary>
    public virtual void SetUnEquip()
    {
        Debug.Log($"{Name} 장착해제 됨");

        CanInteract = true;

        Vector3 unEquipRot = transform.root.eulerAngles;
        unEquipRot.z = 0f;

        transform.position = transform.root.position;
        transform.eulerAngles = unEquipRot;

        transform.SetParent(null);
    }

    private void CacheComponent() { }

    /// <summary>
    /// 총알 생성 및 초기화
    /// </summary>
    private void InitBullets()
    {
        _gunbullet = new ObjectPool<GunBullet>(
            _bullet,
            _maxMagazine,
            _muzzle,
            bullet =>
            {
                SetBulletData(bullet);
                bullet.SetDamageSource(() => _shotDamage);
            }
            );

        //bullets = new GunBullet[_maxMagazine];
        //for (int i = 0; i < bullets.Length; i++)
        //{
        //    SpawnBullet(i);
        //}

        _canReload = true;

        _currentMagazine = _maxMagazine;
        _elapseTime = _fireTime;
    }

    protected void SpawnBullet(int index)
    {
        GunBullet bullet = Instantiate(_bullet, _muzzle);

        SetBulletData(bullet);
        bullet.gameObject.SetActive(false);

        bullets[index] = bullet;
    }

    protected virtual void SetBulletData(GunBullet bullet)
    {
        bullet.SetData(_damage, _attackRange, _bulletSpeed);
    }
}

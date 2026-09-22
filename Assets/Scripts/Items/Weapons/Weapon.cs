using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : Item, IAttackable
{
    [Header("무기 설정")]
    [SerializeField] protected GunBullet _bullet;
    [SerializeField] protected ObjectPool<GunBullet> _gunbullet;
    [SerializeField] protected Transform _muzzle;

    [SerializeField] protected int _damage;
    [SerializeField] protected int _maxMagazine;
    [SerializeField] protected float _fireTime;
    [SerializeField] protected float _attackRange;
    [SerializeField] protected float _bulletSpeed;
    [SerializeField] protected float _reloadDelay;

    private float _elapseTime;
    private bool _isCanFire => _elapseTime >= _fireTime && _currentMagazine > 0;

    protected GunBullet[] bullets;
    protected int _currentMagazine;
    protected bool _canReload;

    protected virtual void Awake() => CacheComponent();
    private void Start() => InitBullets();
    private void Update()
    {
        if (CanInteract) return;

        Tick();
        Fire();
        Reload();
    }

    private void Tick()
    {
        if (_isCanFire) return;

        _elapseTime += Time.deltaTime;
    }

    protected void Fire()
    {
        if (!_isCanFire || !Input.GetMouseButton(0)) return ;

        _bullet = _gunbullet.Pop();
        //bullets[_currentMagazine - 1].OnBulletFire();
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
        _canReload = false;

        yield return new WaitForSeconds(_reloadDelay);

        _currentMagazine = _maxMagazine;

        _canReload = true;
    }

    public override void GetItem(IInteracter owner)
    {
        if (!(owner is TestPlayerContoller)) return;

        TestPlayerContoller player = (TestPlayerContoller)owner;

        // 플레이어 장착 메서드 + 장착 가능한지 판단 추가 필요. 추가후 bool isSuccess = true 제거
        // bool isSuccess = player.AddWeapon(this);
        bool isSuccess = true;
        if (!isSuccess) return;

        Debug.Log("Player 장착 이벤트");

        CanInteract = false;
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));

        // 이미 장착되어 있으면 해제하는 이벤트 임시 구현
        if (player.WeaponTR.childCount != 0)
            player.WeaponTR.GetComponentInChildren<Weapon>().SetUnEquip();

        //player.WeaponTR은 무기가 장착될 Transform
        SetEquip(player.WeaponTR);
    }

    /// <summary>
    /// 장착 메서드
    /// </summary>
    public virtual void SetEquip(Transform equipTR)
    {
        Debug.Log($"{Name} 장착됨");

        CanInteract = false;

        transform.SetParent(equipTR);
    }

    /// <summary>
    /// 장착 해제 메서드
    /// </summary>
    public virtual void SetUnEquip()
    {
        Debug.Log($"{Name} 장착해제 됨");

        CanInteract = true;

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
            _bullet => _bullet.SetData(_damage, _attackRange, _bulletSpeed)
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

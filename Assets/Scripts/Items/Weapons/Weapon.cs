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
    [SerializeField] protected Transform _muzzle;

    [SerializeField] protected int _damage;
    [SerializeField] protected int _maxMagazine;
    [SerializeField] protected float _fireTime;
    [SerializeField] protected float _attackRange;
    [SerializeField] protected float _bulletSpeed;
    [SerializeField] protected float _reloadDelay;

    private float _elapseTime;
    private bool _isCanFire => _elapseTime >= _fireTime && _curretMagazine > 0;

    protected GunBullet[] bullets;
    protected int _curretMagazine;
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

        bullets[_curretMagazine - 1].OnBulletFire();
        _curretMagazine--;

        _elapseTime = 0f;
    }

    protected virtual void Reload()
    {
        if (_curretMagazine > 0) return;

        if (!_canReload) return;

        StartCoroutine(ReloadRoutine());
    }
    protected IEnumerator ReloadRoutine()
    {
        _canReload = false;

        yield return new WaitForSeconds(_reloadDelay);

        _curretMagazine = _maxMagazine;

        _canReload = true;
    }

    public override void GetItem(IInteracter owner)
    {
        if (!(owner is PlayerContoller)) return;

        PlayerContoller player = (PlayerContoller)owner;

        // 플레이어 장착 메서드 + 장착 가능한지 판단
        // bool isSuccess = player.AddWeapon(this);
        bool isSuccess = true;
        if (!isSuccess) return;

        Debug.Log("Player 장착 이벤트");

        CanInteract = false;
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));

        // 이미 장착되어 있으면 해제하는 이벤트
        if (player.WeaponTR.childCount != 0)
            player.WeaponTR.GetComponentInChildren<Weapon>().SetUnEquip();

        SetEquip(player.WeaponTR);
    }

    public virtual void SetEquip(Transform equipTR)
    {
        // 장착시 메서드
        Debug.Log($"{Name} 장착됨");

        CanInteract = false;

        transform.SetParent(equipTR);
    }

    public virtual void SetUnEquip()
    {
        // 장착 해제시 메서드
        Debug.Log($"{Name} 장착해제 됨");

        CanInteract = true;

        transform.SetParent(null);
    }

    private void CacheComponent() { }

    private void InitBullets()
    {
        bullets = new GunBullet[_maxMagazine];

        for (int i = 0; i < bullets.Length; i++)
        {
            SpawnBullet(i);
        }

        _canReload = true;

        _curretMagazine = _maxMagazine;
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

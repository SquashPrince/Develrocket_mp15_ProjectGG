using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GunBullet : PoolObject, IPoolable
{
    [SerializeField] protected LayerMask _damagableMask;
    [SerializeField] private LayerMask _destroyMask;
    [SerializeField] private float _radius;

    [SerializeField] protected int _damage;
    [SerializeField] protected float _range;
    [SerializeField] protected float _bulletSpeed;
    [SerializeField] private Transform _tr;

    private Vector3 _startPos;
    private Func<int> _damageSource;

    public void SetDamageSource(Func<int> damageSource) => _damageSource = damageSource;

    protected void RefreshDamage()
    {
        if (_damageSource != null) _damage = _damageSource();
    }

    public virtual void SetData(int damage, float range, float bulletSpeed)
    {
        _damage = damage;
        _range = range;
        _bulletSpeed = bulletSpeed;

        _tr = transform.parent;
    }

    //private void OnEnable() => ResetState();
    private void FixedUpdate() => MoveFoward();
    public override void WakeUp() => OnBulletFire();
    public override void Sleep() => OnBulletDest();

    private void MoveFoward()
    {
        Vector3 currentPosition = transform.position;
        Vector3 move = transform.forward * (_bulletSpeed * Time.fixedDeltaTime);
        Vector3 nextPosition = currentPosition + move;
        LayerMask detctTarget = _damagableMask | _destroyMask;
        float moveDistance = move.magnitude;

        if (moveDistance > 0f &&
            Physics.SphereCast(currentPosition, _radius, transform.forward, out RaycastHit hit, moveDistance, detctTarget))
        {
            transform.position = hit.point;
            DetectTaraget(hit);
            return;
        }

        transform.position = nextPosition;

        Vector3 diff = _startPos - transform.position;
        if (diff.sqrMagnitude >= _range * _range)
        {
            ReturnToPool();
        }
    }

    protected virtual void DetectTaraget(RaycastHit hit)
    {
        if (hit.transform.TryGetComponent<IDamagable>(out var damageable))
        {
            Hit(damageable);
        }

        ReturnToPool();
    }

    protected void Hit(IDamagable damageable)
    {
        damageable.TakeDamage(_damage);
    }

    public virtual void OnBulletFire() 
    {
        RefreshDamage();
        gameObject.SetActive(true);
        
        _startPos = transform.position;

        transform.parent = null;
    }

    public virtual void OnBulletDest()
    {
        transform.gameObject.SetActive(false);
        transform.parent = _tr;
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
#endif
}

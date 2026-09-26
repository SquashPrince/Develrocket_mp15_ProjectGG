using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonBullet : GunBullet
{
    [SerializeField] private GameObject _explotionEffect;
    [SerializeField] private float _explosionRange;

    protected override void DetectTaraget(RaycastHit hit)
    {
        CanonExplosion(hit);
    }

    private void CanonExplosion(RaycastHit hit)
    {
        GameObject ex = Instantiate(_explotionEffect, hit.point, Quaternion.identity);
        Destroy(ex, 1f);

        ExplosionOverlapSphere();

        ReturnToPool();
    }

    private void ExplosionOverlapSphere()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, _explosionRange, _damagableMask);

        foreach (Collider col in cols)
        {
            if (!(col.GetComponent<IDamagable>() is IDamagable damageable)) continue;

            damageable.TakeDamage(_damage);
        }
    }
}
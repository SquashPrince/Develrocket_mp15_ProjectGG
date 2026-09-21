using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon
{
    protected override void SetBulletData(GunBullet bullet)
    {
        (bullet as ShotgunBullet).SetData(_damage, _attackRange, _bulletSpeed);
    }
}

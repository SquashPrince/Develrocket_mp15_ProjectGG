using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon
{
    /// <summary>
    /// ¼¦°ÇÀº ¼¦°Ç Àü¿ë ÃÑ¾Ë·Î ¼¼ÆÃ
    /// </summary>
    /// <param name="bullet"></param>
    protected override void SetBulletData(GunBullet bullet)
    {
        (bullet as ShotgunBullet).SetData(_damage, _attackRange, _bulletSpeed);
    }
}

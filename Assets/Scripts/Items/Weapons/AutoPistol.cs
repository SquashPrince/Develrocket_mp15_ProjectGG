using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoPistol : Weapon
{
    /// <summary>
    /// 기본 무기는 재장전 시간없이 발사
    /// </summary>
    protected override void Reload()
    {
        if (_currentMagazine > 0) return;

        _currentMagazine = _maxMagazine;
    }
}

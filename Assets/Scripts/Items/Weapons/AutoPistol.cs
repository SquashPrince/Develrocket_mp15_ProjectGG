using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoPistol : Weapon
{
    protected override void Reload()
    {
        if (_curretMagazine > 0) return;

        _curretMagazine = _maxMagazine;
    }
}

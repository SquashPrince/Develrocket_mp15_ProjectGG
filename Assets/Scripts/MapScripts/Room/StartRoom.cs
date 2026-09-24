using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartRoom : RoomBase
{
    private void Awake() => CheckClear();
    public override void CheckClear()
    {
        _isClear = true;
    }
}

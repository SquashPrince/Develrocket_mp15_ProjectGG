using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRoom : RoomBase
{
    public override void OnEnter()
    {
        base.OnEnter();
        _isClear = true;
    }

    public override void OnRunning()
    {
        base.OnRunning();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}

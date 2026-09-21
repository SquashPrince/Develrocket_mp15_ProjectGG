using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class RoomBase : MonoBehaviour
{
    public RoomData Data;

    public void ClearRoom()
    {
        Data.IsCleared = true;
    }

    private void CloseDoors()
    {
        foreach (DoorBase door in Data.Doors)
        {
            door.CloseDoor();
        }
    }
    private void OpenDoors()
    {
        foreach (DoorBase door in Data.Doors)
        {
            door.OpenDoor();
        }
    }

    private void Init()
    {
        Data.IsCleared = false;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomData : MonoBehaviour
{
    public Vector2 RoomSize;
    public RoomType Type;
    public DoorBase[] Doors = new DoorBase[4];
    public bool IsCleared;
    public bool IsVisited;
}

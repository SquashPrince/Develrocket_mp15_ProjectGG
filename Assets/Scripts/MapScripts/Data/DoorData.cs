using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorData : MonoBehaviour
{
    public bool IsOpened = false;
    public DoorType Type;
    public Vector2 _direction;
    public Vector2 Direction
    {
        get => _direction;
        set
        {
            _direction = Type switch
            {
                DoorType.UP => Vector2.up,
                DoorType.DOWN => Vector2.down,
                DoorType.LEFT => Vector2.left,
                DoorType.RIGHT => Vector2.right,
                _ => Vector2.zero
            };
        }
    }
    public RoomBase NextRoom;
}

using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private Room _roomPrefab;
    [SerializeField] private Transform _roomRoot;
    [SerializeField] private float _roomSizeOffset = 15f;

    private Dictionary<Vector2Int, Room> _rooms = new();

    private RoomType[,] map =
    {
        {
            RoomType.NONE, RoomType.BASIC, RoomType.START, RoomType.NONE, RoomType.NONE
        },

        {
            RoomType.NONE, RoomType.BASIC, RoomType.BATTLE, RoomType.STORE, RoomType.NONE
        },

        {
            RoomType.NONE, RoomType.NONE, RoomType.TREASURE, RoomType.NONE, RoomType.NONE
        },

        {
            RoomType.NONE, RoomType.NONE, RoomType.BOSS, RoomType.NONE, RoomType.NONE
        }
    };

    private void Start()
    {
        GenerateMap();
    }

    private void GenerateMap()
    {
        for (int width = 0; width < map.GetLength(0); width++)
        {
            for (int height = 0; height < map.GetLength(1); height++)
            {
                if (map[width, height] == RoomType.NONE)
                    continue;

                CreateRoom(new Vector2Int(width, height), map[width, height]);
            }
        }

        SetupDoors();
    }

    private void CreateRoom(Vector2Int position, RoomType roomType)
    {
        Vector3 grid = GetGrid(position);

        Room room = Instantiate(_roomPrefab, grid, _roomRoot.rotation, _roomRoot);

        room.Initialize(position, roomType);

        _rooms.Add(position, room);
    }

    private void SetupDoors()
    {
        foreach (KeyValuePair<Vector2Int, Room> pair in _rooms)
        {
            Vector2Int position = pair.Key;
            Room room = pair.Value;

            SetupDoor(room, position, DoorDirection.UP, Vector2Int.up);
            SetupDoor(room, position, DoorDirection.RIGHT, Vector2Int.right);
            SetupDoor(room, position, DoorDirection.DOWN, Vector2Int.down);
            SetupDoor(room, position, DoorDirection.LEFT, Vector2Int.left);
        }
    }

    private void SetupDoor(Room room, Vector2Int position, DoorDirection direction, Vector2Int offset)
    {
        Vector2Int nextPosition = position + offset;

        bool hasNextRoom = _rooms.ContainsKey(nextPosition);

        room.SetDoor(direction, hasNextRoom);

        if (!hasNextRoom) return;

        DoorController doorController = room.GetDoor(direction).GetComponentInChildren<DoorController>();

        if (doorController == null) return;

        doorController.Initialize(this, position, direction);
    }

    public void EnterRoom(Vector2Int currentPosition, DoorDirection direction, Transform player)
    {
        Vector2Int nextPosition = currentPosition + GetDirection(direction);
        Debug.Log(nextPosition);

        if (!_rooms.TryGetValue(nextPosition, out Room nextRoom)) return;

        DoorDirection opposite = GetOpposite(direction);
        Debug.Log(opposite);

        Transform targetDoor = nextRoom.GetDoor(opposite);

        if (targetDoor == null) return;

        player.transform.position = (targetDoor.transform.position + targetDoor.forward * 2f + targetDoor.up);
    }

    private Vector2Int GetDirection(DoorDirection direction)
    {
        return direction switch
        {
            DoorDirection.UP => Vector2Int.up,
            DoorDirection.RIGHT => Vector2Int.right,
            DoorDirection.DOWN => Vector2Int.down,
            DoorDirection.LEFT => Vector2Int.left,
            _ => Vector2Int.zero
        };
    }

    private DoorDirection GetOpposite(DoorDirection direction)
    {
        return direction switch
        {
            DoorDirection.UP => DoorDirection.DOWN,
            DoorDirection.RIGHT => DoorDirection.LEFT,
            DoorDirection.DOWN => DoorDirection.UP,
            DoorDirection.LEFT => DoorDirection.RIGHT,
            _ => DoorDirection.UP
        };
    }

    private Vector3 GetGrid(Vector2Int position)
    {
        return new Vector3(position.x * _roomSizeOffset, 0, position.y * _roomSizeOffset);
    }
}

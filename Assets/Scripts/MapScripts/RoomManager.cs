using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private Room roomPrefab;
    [SerializeField] private Transform roomRoot;

    [SerializeField] private float roomWidth = 20f;
    [SerializeField] private float roomDepth = 12f;

    private readonly Dictionary<Vector2Int, Room> rooms = new();

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
        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                if (map[y, x] == RoomType.NONE)
                    continue;

                CreateRoom(
                    new Vector2Int(x, y),
                    map[y, x]
                );
            }
        }

        SetupDoors();
    }

    private void CreateRoom(Vector2Int position, RoomType roomType)
    {
        Vector3 Position = GetGrid(position);

        Room room = Instantiate(roomPrefab, Position, Quaternion.identity, roomRoot);

        room.Initialize(position, roomType);

        rooms.Add(position, room);
    }

    private void SetupDoors()
    {
        foreach (KeyValuePair<Vector2Int, Room> pair in rooms)
        {
            Vector2Int position = pair.Key;
            Room room = pair.Value;

            SetupDoor(room, position, DoorDirection.UP, Vector2Int.up);
            SetupDoor(room, position, DoorDirection.RIGHT, Vector2Int.right);
            SetupDoor(room, position, DoorDirection.DOWN, Vector2Int.down);
            SetupDoor(room, position, DoorDirection.LEFT, Vector2Int.left);
        }
    }

    private void SetupDoor(
        Room room,
        Vector2Int position,
        DoorDirection direction,
        Vector2Int offset)
    {
        Vector2Int nextPosition = position + offset;

        bool hasNextRoom = rooms.ContainsKey(nextPosition);

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

        if (!rooms.TryGetValue(nextPosition, out Room nextRoom)) return;

        DoorDirection opposite = GetOpposite(direction);

        Transform targetDoor = nextRoom.GetDoor(opposite);

        if (targetDoor == null) return;

        Debug.Log(player.name);

        player.Translate(targetDoor.position + targetDoor.forward * 5);
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
        return new Vector3(
            position.x * roomWidth,
            0f,
            position.y * roomDepth
        );
    }
}

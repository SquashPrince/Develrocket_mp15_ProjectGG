using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private Room _roomPrefab;
    [SerializeField] private Transform _roomRoot;
    [SerializeField] private float _roomSizeOffset = 15f;

    //TODO: 싱글톤 패턴으로 구현하여 플레이어가 자신을 참조시킬 수 있도록 한다.

    private Dictionary<Vector2Int, Room> _rooms = new();

    // 하드 코딩하여 작성한 맵입니다.
    private RoomType[, ] map =
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

    private void Start() => GenerateMap();

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

    private void CreateRoom(Vector2Int grid, RoomType roomType)
    {
        Vector3 worldPosition = GetWorldPosition(grid);

        Room room = Instantiate(_roomPrefab, worldPosition, _roomRoot.rotation, _roomRoot);

        room.Initialize(grid, roomType);

        _rooms.Add(grid, room);
    }

    private void SetupDoors()
    {
        foreach (KeyValuePair<Vector2Int, Room> roomData in _rooms)
        {
            Vector2Int position = roomData.Key;
            Room room = roomData.Value;

            SetupDoor(room, position, DoorDirection.UP, Vector2Int.up);
            SetupDoor(room, position, DoorDirection.RIGHT, Vector2Int.right);
            SetupDoor(room, position, DoorDirection.DOWN, Vector2Int.down);
            SetupDoor(room, position, DoorDirection.LEFT, Vector2Int.left);
        }
    }

    private void SetupDoor(Room room, Vector2Int grid, DoorDirection direction, Vector2Int offset)
    {
        Vector2Int nearRoom = grid + offset;

        bool isActive = _rooms.ContainsKey(nearRoom);

        room.SetDoor(direction, isActive);

        if (!isActive) return;

        DoorController doorController = room.GetDoor(direction).GetComponent<DoorController>();
        doorController.Initialize(this, grid, direction);
    }

    public void EnterRoom(Vector2Int currentGrid, DoorDirection direction, Transform player)
    {
        Vector2Int nextGrid = currentGrid + GetDirection(direction);
        Debug.Log(nextGrid);

        if (!_rooms.TryGetValue(nextGrid, out Room nextRoom)) return;

        DoorDirection opposite = GetOpposite(direction);
        Debug.Log(opposite);

        Transform targetDoor = nextRoom.GetDoor(opposite);

        //TODO: 땅에 끼는 것을 방지하기 위해 targetDoor.up을 사용하고 있는데, 추후 플레이어의 콜라이더를 잘 옮겨
        //수정하면 될 것 같습니다.
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

    private Vector3 GetWorldPosition(Vector2Int position)
    {
        return new Vector3(position.x * _roomSizeOffset, 0, position.y * _roomSizeOffset);
    }
}

using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    private static RoomManager _instance;
    public static RoomManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindAnyObjectByType<RoomManager>();
            }

            return _instance;
        }
    }

    //TODO: 구조 설명 후 룸 컨트롤러는 추후 지울 예정입니다.
    [SerializeField] private RoomController _roomPrefab;

    [SerializeField] private Transform _playerTransform;

    [SerializeField] private BasicRoom _basicRoomPrefab;
    [SerializeField] private BattleRoom _battleRoomPrefab;
    [SerializeField] private StoreRoom _storeRoomPrefab;
    [SerializeField] private TreasureRoom _treasureRoomPrefab;
    [SerializeField] private BossRoom _bossRoomPrefab;

    [SerializeField] private Transform _roomRoot;

    
    private Dictionary<Vector2Int, RoomBase> _rooms = new();

    private float _doorOffset => _roomPrefab.transform.localScale.x;
    private float _roomSizeOffset => _roomPrefab.transform.localScale.x * 15.0f;
    private RoomBase _currentRoom;

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
    private void Awake() => SetSingleton();
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
        _currentRoom.OnEnter();
        _playerTransform.position = _currentRoom.transform.position;
    }

    private void CreateRoom(Vector2Int grid, RoomType roomType)
    {
        Vector3 worldPosition = GetWorldPosition(grid);

        RoomBase room = roomType switch
        {
            RoomType.START => Instantiate(_basicRoomPrefab, worldPosition, _roomRoot.rotation, _roomRoot),
            RoomType.BASIC => Instantiate(_basicRoomPrefab, worldPosition, _roomRoot.rotation, _roomRoot),
            RoomType.BATTLE => Instantiate(_battleRoomPrefab, worldPosition, _roomRoot.rotation, _roomRoot),
            RoomType.STORE => Instantiate(_storeRoomPrefab, worldPosition, _roomRoot.rotation, _roomRoot),
            RoomType.TREASURE => Instantiate(_treasureRoomPrefab, worldPosition, _roomRoot.rotation, _roomRoot),
            RoomType.BOSS => Instantiate(_bossRoomPrefab, worldPosition, _roomRoot.rotation, _roomRoot),
            _ => null
        };

        room.Initialize(grid, roomType);

        if (roomType == RoomType.START) _currentRoom = room;

        _rooms.Add(grid, room);
    }

    private void SetupDoors()
    {
        foreach (KeyValuePair<Vector2Int, RoomBase> roomData in _rooms)
        {
            Vector2Int position = roomData.Key;
            RoomBase room = roomData.Value;

            SetupDoor(room, position, DoorDirection.UP, Vector2Int.up);
            SetupDoor(room, position, DoorDirection.RIGHT, Vector2Int.right);
            SetupDoor(room, position, DoorDirection.DOWN, Vector2Int.down);
            SetupDoor(room, position, DoorDirection.LEFT, Vector2Int.left);

        }
    }

    private void SetupDoor(RoomBase room, Vector2Int grid, DoorDirection direction, Vector2Int offset)
    {
        Vector2Int nearRoom = grid + offset;

        bool isActive = _rooms.ContainsKey(nearRoom);

        room.SetDoor(direction, isActive);

        if (!isActive) return;

        DoorController door = room.GetDoor(direction).GetComponent<DoorController>();
        door.Initialize(this, grid, direction);
    }

    public void EnterRoom(Vector2Int currentGrid, DoorDirection direction, Transform target)
    {
        Vector2Int nextGrid = currentGrid + GetDirection(direction);
        Debug.Log(nextGrid);

        if (!_rooms.TryGetValue(nextGrid, out RoomBase nextRoom)) return;

        DoorDirection opposite = GetOpposite(direction);
        Debug.Log(opposite);

        Transform targetDoor = nextRoom.GetDoor(opposite);
        
        target.position = (targetDoor.transform.position + targetDoor.forward * _doorOffset + targetDoor.up);
        
        _currentRoom.OnExit();
        _currentRoom = nextRoom;
        _currentRoom.OnEnter();
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

    private void SetSingleton()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(_instance);
        }
        else
        {
            _instance = this;
        }
    }
}

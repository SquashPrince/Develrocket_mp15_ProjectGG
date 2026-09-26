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

    [SerializeField] private BasicRoom _basicRoomPrefab;
    [SerializeField] private BattleRoom _battleRoomPrefab;
    [SerializeField] private StoreRoom _storeRoomPrefab;
    [SerializeField] private TreasureRoom _treasureRoomPrefab;
    [SerializeField] private BossRoom _bossRoomPrefab;

    [SerializeField] private Transform _roomRoot;
    [SerializeField] private float _customRoomOffset;

    private Transform _playerTransform;

    /// <summary>
    /// 배열에서의 좌표를 Key로, 방에 대한 참조를 Value로 가집니다. KeyValuePair로 접근하시면 됩니다.
    /// </summary>
    private Dictionary<Vector2Int, RoomBase> _rooms = new();

    /// <summary>
    /// 방에서 문으로부터 플레이어를 소환할 오프셋을 정의하고 있습니다.
    /// </summary>
    private float _doorOffset => _roomPrefab.transform.localScale.x;

    /// <summary>
    /// 방과 방이 소환될 때 그 사이 크기에 대한 오프셋을 정의하고 있습니다. 뒤의 실수부를 수정하면, 더 크게 방을 띄울 수 있습니다.
    /// </summary>
    private float _roomSizeOffset => _roomPrefab.transform.localScale.x * _customRoomOffset;

    /// <summary>
    /// 현재 방에 대한 참조를 가집니다.
    /// </summary>
    private RoomBase _currentRoom;

    /// <summary>
    /// 하드 코딩되어 작성된 맵에 대한 정수형(열거형) 정보입니다.
    /// </summary>
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
    private void Start()
    {
        Initialize();
        GenerateMap();
    }

    /// <summary>
    /// 그리드에 적혀있는 규칙대로 방을 생성하는 메서드 입니다. 사실상 방 생성 메서드에 정보를 넘겨주는 역할, 여러 초기화 기능들을 수행하고 있습니다.
    /// </summary>
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

        SetDoorAll();
        _currentRoom.OnEnter();
        // 인스펙터에서 끌어온 플레이어 참조를 통해 플레이어 정보를 가져오고, 여기서 START 씬으로 이동 시킵니다.
        _playerTransform.position = _currentRoom.transform.position;
    }

    /// <summary>
    /// 방을 생성하는 메서드 입니다.
    /// </summary>
    /// <param name="grid">방의 (x, y) 좌표</param>
    /// <param name="roomType">방의 종류 (열거형 파일 참고)</param>
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

    /// <summary>
    /// 방에 있는 4개의 문을 설정하는 메서드 입니다. 한 문에 대한 자세한 설정 메서드는 SetDoor() 메서드에서 진행합니다.
    /// </summary>
    private void SetDoorAll()
    {
        foreach (KeyValuePair<Vector2Int, RoomBase> roomData in _rooms)
        {
            Vector2Int position = roomData.Key;
            RoomBase room = roomData.Value;

            SetDoor(room, position, DoorDirection.UP, Vector2Int.up);
            SetDoor(room, position, DoorDirection.RIGHT, Vector2Int.right);
            SetDoor(room, position, DoorDirection.DOWN, Vector2Int.down);
            SetDoor(room, position, DoorDirection.LEFT, Vector2Int.left);

        }
    }

    /// <summary>
    /// 방을 받아와 다음으로 이동할 방과 그 방으로 플레이어가 이동 될 위치를 설정 합니다.
    /// </summary>
    /// <param name="room">방에 대한 참조</param>
    /// <param name="grid">방의 좌표</param>
    /// <param name="direction">문의 방향(상, 하, 좌, 우)</param>
    /// <param name="offset">근처에 있는 문</param>
    private void SetDoor(RoomBase room, Vector2Int grid, DoorDirection direction, Vector2Int gridOffset)
    {
        Vector2Int nearRoom = grid + gridOffset;

        bool isActive = _rooms.ContainsKey(nearRoom);

        room.SetDoor(direction, isActive);

        if (!isActive) return;

        DoorController door = room.GetDoor(direction).GetComponent<DoorController>();
        door.Initialize(this, grid, direction);
    }

    /// <summary>
    /// 실질적으로 문에서 다른 문으로 이동시키는 메서드입니다. doorController에서 만족한 조건대로 수행합니다.
    /// 또한, 방마다 구현되어야 하는 기능들을 수행 시킵니다.
    /// </summary>
    /// <param name="currentGrid">현재 방 좌표</param>
    /// <param name="direction">상호작용 한 문의 방향(상, 하, 좌, 우)</param>
    /// <param name="target">최종적으로 이동할 위치</param>
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

        if (_currentRoom.RoomType == RoomType.BOSS)
        {
            // TODO: 페이드 인 페이드 아웃..? 나중에 통합 이후 테스트 해보고 정상 작동하면 활성화 할 예정입니다.
            // 당장 필요하다고 판단하지는 않으시는 것 같으셔서 주석처리 해두었습니다.
            //UIManager.Instance.Window.OpenLoading();
        }

        _currentRoom.OnEnter();
    }

    /// <summary>
    /// 문에 따라 이동할 그리드 방향을 받아오는 메서드 입니다.
    /// </summary>
    /// <param name="direction">현재 방 문 위치</param>
    /// <returns></returns>
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

    /// <summary>
    /// 이동해서 나올 문의 위치를 받아오는 메서드입니다. (ex. 위로 가면 다음 방의 아래 문에서 나옵니다)
    /// </summary>
    /// <param name="direction">현재 방 문 위치</param>
    /// <returns></returns>
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

    /// <summary>
    /// 정수 기반 배열에서 실제 방이 배치되어야 하는 위치를 가져옵니다.
    /// </summary>
    /// <param name="position">방 좌표</param>
    /// <returns></returns>
    private Vector3 GetWorldPosition(Vector2Int position)
    {
        return new Vector3(position.x * _roomSizeOffset, 0, position.y * _roomSizeOffset);
    }

    private void Initialize()
    {
        //TODO: 게임 매니저 실제로 사용하게 된다면 그 쪽에서 참조 넘겨줄 수 있도록 하려고 합니다.
        _playerTransform = GameManager.Instance.PlayerTransform;
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

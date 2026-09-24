using UnityEngine;

public abstract class RoomBase: MonoBehaviour
{
    [SerializeField] protected DoorController _upDoor;
    [SerializeField] protected DoorController _leftDoor;
    [SerializeField] protected DoorController _downDoor;
    [SerializeField] protected DoorController _rightDoor;
    
    public Vector2Int GridPosition { get; private set; }
    public RoomType RoomType { get; private set; }
    protected bool _isClear;

    public void Initialize(Vector2Int gridPosition ,RoomType roomType)
    {
        GridPosition = gridPosition;
        RoomType = roomType;

        if(roomType == RoomType.START || roomType == RoomType.BASIC || roomType == RoomType.STORE)
        {
            _isClear = true;
            OpenDoor();
        }
        else
        {
            _isClear = false;
            CloseDoor();
        }
    }

    public void OpenDoor()
    {
        if (!_isClear) return;

        _upDoor.Open();
        _downDoor.Open();
        _leftDoor.Open();
        _rightDoor.Open();
    }

    public void CloseDoor()
    {
        _upDoor.Close();
        _downDoor.Close();
        _leftDoor.Close();
        _rightDoor.Close();
    }

    public virtual void CheckClear()
    {
        //TODO: 방 마다 다른 클리어 조건 판단 로직 수행할 것
    }

    public void SetDoor(DoorDirection direction, bool active)
    {
        switch (direction)
        {
            case DoorDirection.UP:
                _upDoor.gameObject.SetActive(active);
                break;

            case DoorDirection.LEFT:
                _leftDoor.gameObject.SetActive(active);
                break;

            case DoorDirection.DOWN:
                _downDoor.gameObject.SetActive(active);
                break;

            case DoorDirection.RIGHT:
                _rightDoor.gameObject.SetActive(active);
                break;
        }
    }

    public Transform GetDoor(DoorDirection direction)
    {
        return direction switch
        {
            DoorDirection.UP => _upDoor.transform,
            DoorDirection.LEFT => _leftDoor.transform,
            DoorDirection.DOWN => _downDoor.transform,
            DoorDirection.RIGHT => _rightDoor.transform,
            _ => null
        };
    }
}

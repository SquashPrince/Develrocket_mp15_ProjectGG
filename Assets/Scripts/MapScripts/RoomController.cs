using UnityEngine;

public class RoomController : MonoBehaviour
{
    [SerializeField] private DoorController _upDoor;
    [SerializeField] private DoorController _leftDoor;
    [SerializeField] private DoorController _downDoor;
    [SerializeField] private DoorController _rightDoor;
    
    public Vector2Int GridPosition { get; private set; }
    public RoomType RoomType { get; private set; }
    private bool _isClear;

    public void Initialize(Vector2Int gridPosition ,RoomType roomType)
    {
        GridPosition = gridPosition;
        RoomType = roomType;

        if(roomType == RoomType.START || roomType == RoomType.BASIC)
        {
            _isClear = true;
            OpenDoor();
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

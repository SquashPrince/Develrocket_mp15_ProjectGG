using Unity.VisualScripting;
using UnityEngine;

public abstract class RoomBase : MonoBehaviour
{
    [SerializeField] protected DoorController _upDoor;
    [SerializeField] protected DoorController _leftDoor;
    [SerializeField] protected DoorController _downDoor;
    [SerializeField] protected DoorController _rightDoor;

    public Vector2Int GridPosition { get; private set; }
    public RoomType RoomType { get; private set; }
    protected bool _isClear;

    public void Initialize(Vector2Int gridPosition, RoomType roomType)
    {
        GridPosition = gridPosition;
        RoomType = roomType;

        if (roomType == RoomType.START || roomType == RoomType.BASIC || roomType == RoomType.STORE || roomType == RoomType.TREASURE)
        {
            _isClear = true;
            OpenDoor();
        }
        else
        {
            _isClear = false;
        }
    }

    public virtual void OnEnter()
    {
        Debug.Log($"RoomType: {RoomType} 입장");
    }

    public virtual void OnRunning()
    {
        Debug.Log($"RoomType: {RoomType} 로직 진행 중");
    }

    public virtual void OnExit()
    {
        Debug.Log($"RoomType: {RoomType} 퇴장");
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

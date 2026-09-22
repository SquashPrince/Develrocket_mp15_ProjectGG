using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private GameObject _upDoor;
    [SerializeField] private GameObject _leftDoor;
    [SerializeField] private GameObject _downDoor;
    [SerializeField] private GameObject _rightDoor;

    public Vector2Int GridPosition { get; private set; }
    public RoomType RoomType { get; private set; }

    public void Initialize(
        Vector2Int gridPosition,
        RoomType roomType)
    {
        GridPosition = gridPosition;
        RoomType = roomType;
    }

    public void SetDoor(DoorDirection direction, bool active)
    {
        switch (direction)
        {
            case DoorDirection.UP:
                _upDoor.SetActive(active);
                break;

            case DoorDirection.LEFT:
                _leftDoor.SetActive(active);
                break;

            case DoorDirection.DOWN:
                _downDoor.SetActive(active);
                break;

            case DoorDirection.RIGHT:
                _rightDoor.SetActive(active);
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

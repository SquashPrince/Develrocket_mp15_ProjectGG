using UnityEditor.EditorTools;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private RoomManager _roomManager;
    private Vector2Int _roomPosition;
    private DoorDirection _direction;

    public void Initialize(
        RoomManager manager,
        Vector2Int roomPosition,
        DoorDirection direction)
    {
        _roomManager = manager;
        _roomPosition = roomPosition;
        _direction = direction;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log("플레이어 인식");

        _roomManager.EnterRoom(
            _roomPosition,
            _direction,
            other.transform
        );
    }
}

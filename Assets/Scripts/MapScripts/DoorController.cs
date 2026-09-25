using Player;
using UnityEditor.EditorTools;
using UnityEngine;

public class DoorController : MonoBehaviour, IInteractable
{
    private RoomManager _roomManager;
    private Vector2Int _roomPosition;
    private DoorDirection _direction;
    private Collider _player;
    private bool _isOpen;

    public string Name => name;
    public string Info => "door";

    public void Initialize(RoomManager manager, Vector2Int roomPosition, DoorDirection direction)
    {
        _roomManager = manager;
        _roomPosition = roomPosition;
        _direction = direction;
    }

    public void Open()
    {
        _isOpen = true;
    }

    public void Close()
    {
        _isOpen = false;
    }

    public void Interact(IInteractor owner)
    {
        //_roomManager.EnterRoom(_roomPosition, _direction, owner.Transform);
    }

    private void Enter()
    {
        if (!_isOpen)
        {
            Debug.Log("DoorController: 잠긴 문, 개방 불가");
            return;
        }

        _roomManager.EnterRoom(_roomPosition, _direction, _player.transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IInteractor>() == null) return;
        Debug.Log("플레이어 검사 성공");
        _player = other;
        PlayerInputManager.Instance.OnInteract += Enter;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<IInteractor>() == null) return;

        PlayerInputManager.Instance.OnInteract -= Enter;
        _player = null;
    }
}

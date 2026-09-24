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

    public void Initialize(RoomManager roomManager, Vector2Int roomPosition, DoorDirection direction)
    {
        _roomManager = roomManager;
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
        // - InputManager에 직접 구독 시켜야 정상 작동하고 있는 것 같아 그렇게 하였습니다.
        //_roomManager.EnterRoom(_roomPosition, _direction, owner.Transform);
    }

    private void Enter()
    {
        if (!_isOpen)
        {
            return;
        }

        _roomManager.EnterRoom(_roomPosition, _direction, _player.transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IInteractor>() == null) return;

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

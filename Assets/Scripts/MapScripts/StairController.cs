using Player;
using UnityEngine;

public class StairController : MonoBehaviour
{
    private PlayerInputManager _input;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Spawn()
    {
        gameObject.SetActive(true);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<IInteractor>() != null)
        {
            _input = PlayerInputManager.Instance;
            _input.OnInteract -= ExitFloor;
            _input.OnInteract += ExitFloor;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<IInteractor>()!= null)
        {
            if (_input != null) _input.OnInteract -= ExitFloor;
        }
    }

    private void OnDisable()
    {
        if (_input != null) _input.OnInteract -= ExitFloor;
    }

    private void ExitFloor()
    {
        if (GameManager.Instance == null || !GameManager.Instance.CanPlay) return;
        GameManager.Instance.CompleteGame();
    }
}

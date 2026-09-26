using Player;
using UnityEngine;

public class StairController : MonoBehaviour
{
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
            PlayerInputManager.Instance.OnInteract += ExitFloor;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<IInteractor>()!= null)
        {
            PlayerInputManager.Instance.OnInteract -= ExitFloor;
        }
    }

    private void ExitFloor()
    {
        UIManager.Instance.Window.NextEWindow = EWindowType.Lobby;
        UIManager.Instance.Window.OpenLoading();
    }
}

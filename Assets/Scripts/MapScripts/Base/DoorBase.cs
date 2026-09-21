using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBase : MonoBehaviour
{
    public DoorData Data;

    private void Awake() => CacheComponents();

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && Data.IsOpened)
        {

        }
    }

    public void CloseDoor()
    {
        Data.IsOpened = false;
        gameObject.SetActive(false);
    }

    public void OpenDoor()
    {
        Data.IsOpened = true;
        gameObject.SetActive(true);
    }

    public void SetNextRoom(RoomBase nextRoom)
    {
        Data.NextRoom = nextRoom;
    }

    private void CacheComponents()
    {
        Data = GetComponent<DoorData>();
    }
}

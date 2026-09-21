using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    private static FloorManager _instance;
    public static FloorManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<FloorManager>();
            }

            return _instance;
        }
    }

    public RoomBase RoomPrefab;
    private RoomBase _currentRoom;
    public event Action OnRoomChanged;

    public int[,] IntFloor =
    {
        {0, -1, -1, -1, -1 },
        {1, -1, 3, -1, -1 },
        {1, 4, 1, -1, -1 },
        {-1, -1, 4, 4, 5 },
        {-1, -1, -1, 2, -1 }
    };

    public RoomBase[,] CurrentFloor;

    private void Awake() => SetSingleton();
    private void Start() => SetRoom();

    public void MoveRoom()
    {
        OnRoomChanged?.Invoke();
    }

    public void SetRoom()
    {
        CurrentFloor = new RoomBase[IntFloor.GetLength(0), IntFloor.GetLength(1)];

        for(int w = 0; w < IntFloor.GetLength(0); w++)
        {
            for(int h = 0; h < IntFloor.GetLength(1); h++)
            {
                if (IntFloor[w, h] == -1)
                {
                    CurrentFloor[w, h] = null;
                }

                //TODO: 방 종류에 따라 프리팹 구분하는 로직 구현 필요
                RoomBase room = Instantiate(RoomPrefab);
                room.Data.Type = (RoomType)IntFloor[w, h];

                if (IntFloor[w, h] == 0)
                {
                    _currentRoom = room;
                }
                else
                {
                    room.gameObject.SetActive(false);
                }

                CurrentFloor[w, h] = room;
            }
        }
    }

    private void SetSingleton()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(_instance);
        }
        else
        {
            _instance = this;
        }
    }
}

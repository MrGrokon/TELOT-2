using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLoader : MonoBehaviour
{
    public GameObject[] Rooms;
    public int currentRoom = 0;
    public int previousRoom;

    public void ActivateRoom(int nextRoomId)
    {
        print("Activation !");
        Rooms[nextRoomId].SetActive(true);
        currentRoom = nextRoomId;
        previousRoom = currentRoom - 1;
    }

    public void DesactivateRoom()
    {
        print("Desactivation !");
        Rooms[previousRoom].SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class RoomItem : MonoBehaviour
{
    public TextMeshProUGUI roomName;
    CreateAndJoinRooms manager;

    private void Start()
    {
        manager = FindObjectOfType<CreateAndJoinRooms>();
    }

    public void SetRoomName(string _roomName)
    {
        roomName.text = _roomName;
    }

    public void OnClickItem()
    {
        manager.JoinRoom(roomName.text);
    }
}

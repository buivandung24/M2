using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class RoomData : MonoBehaviourPunCallbacks
{
    public string RoomName;
    public bool IsOpen;
    public int MaxPlayers;

    public RoomData(string RoomName, bool IsOpen, int MaxPlayers){
        this.RoomName = RoomName;
        this.IsOpen = IsOpen;
        this.MaxPlayers = MaxPlayers;
    }

    public void SetRoomName(string name)
    {
        RoomName = name;
    }

    public void OnClick_JoinRoom()
    {
        PhotonNetwork.JoinRoom(RoomName);
    }
}

[Serializable]
public class RoomsList
{
    public List<RoomData> Rooms;
}

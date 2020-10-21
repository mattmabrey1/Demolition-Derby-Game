using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using Photon.Pun;

public class RoomListing : MonoBehaviour
{
    [SerializeField]
    private TMP_Text roomName = null;
    [SerializeField]
    private TMP_Text playerCount = null;


    public RoomInfo _RoomInfo { get; private set; }
    
    public void SetRoomInfo(RoomInfo roomInfo)
    {
        _RoomInfo = roomInfo;

        roomName.text = roomInfo.Name;
        playerCount.text = roomInfo.PlayerCount + "/" + roomInfo.MaxPlayers;
    }

    public void OnClick_Button()
    {
        PhotonNetwork.JoinRoom(_RoomInfo.Name);
    }
}

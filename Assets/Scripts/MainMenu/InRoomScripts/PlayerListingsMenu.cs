using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class PlayerListingsMenu : MonoBehaviourPunCallbacks, IPunCallbacks
{
    [SerializeField]
    private Button startButton = null;
    [SerializeField]
    private GameObject Panel_HostGame = null;
    [SerializeField]
    private GameObject Panel_MainMenu = null;
    [SerializeField]
    private GameObject Panel_CurrentRoom = null;

    [SerializeField]
    private TMP_Text lobbyName = null, lobbyCapacity = null;

    [SerializeField]
    private List<PlayerListing> playerListings = new List<PlayerListing>();

    private int numberOfPlayers = 0, maxPlayers = 8;


    public override void OnJoinedRoom()
    {
        Panel_HostGame.SetActive(false);
        Panel_CurrentRoom.SetActive(true);

        lobbyName.text = PhotonNetwork.CurrentRoom.Name;
        maxPlayers = PhotonNetwork.CurrentRoom.MaxPlayers;
        
        lobbyCapacity.text = "" + numberOfPlayers + " / " + maxPlayers;

        GetCurrentRoomPlayers();
    }

    void Update()
    {
        if(PhotonNetwork.InRoom)
        {
            if(PhotonNetwork.CurrentRoom.PlayerCount > 1)
            {

                startButton.interactable = PhotonNetwork.IsMasterClient;
            }
        }
        
    }

    void GetCurrentRoomPlayers()
    {
        if (!PhotonNetwork.IsConnected)
            return;
        if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.Players == null)
            return;

        
        foreach (KeyValuePair<int, Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
        {
            AddPlayerListing(playerInfo.Value);
        }
    }

    void AddPlayerListing(Player player)
    {
        int index = playerListings.FindIndex(x => x._player == player);

        if (index == -1)
        {
            playerListings[numberOfPlayers].SetPlayerInfo(player);
            numberOfPlayers++;
        }


        lobbyCapacity.text = "" + numberOfPlayers + " / " + maxPlayers;

    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddPlayerListing(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        int index = playerListings.FindIndex(x => x._player == otherPlayer);

        if (index != -1)
        {
            playerListings[index].ClearListing();
        }

        numberOfPlayers--;


        lobbyCapacity.text = "" + numberOfPlayers + " / " + maxPlayers;
    }

    public void OnClick_LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }


    public override void OnLeftRoom()
    {
        foreach (PlayerListing playerList in playerListings)
        {
            playerList.ClearListing();
        }

        numberOfPlayers = 0;

        Panel_MainMenu.SetActive(true);
        Panel_CurrentRoom.SetActive(false);
        
        
        //StartCoroutine("LeaveLobbyIE");
    }

    IEnumerator LeaveLobbyIE()
    {
        yield return new WaitForSeconds(.5f);
        PhotonNetwork.JoinLobby();
       
    }

    public void OnClick_StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.LoadLevel("Game");
        }
        
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
       
        int index = playerListings.FindIndex(x => x._player == newMasterClient);
        if(index != -1)
        {
            playerListings[index].SetPlayerInfo(newMasterClient);
        }
        

    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class CreateRoomMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject createdRoom = null;

    [SerializeField]
    private TMP_Text roomName = null;

    [SerializeField]
    private Button createRoomButton = null;
    
    private byte maxPlayers = 8;

    [SerializeField]
    private TMP_InputField maxPlayersInput = null, roomNameInput = null;

    [SerializeField]
    private Image[] checkMarks = new Image[0];

    private bool[] allowedAbilities = new bool[3] { true, true, true };

    [SerializeField]
    private Button[] allAvailableButtons = new Button[1];
    
    void Start()
    {
        maxPlayersInput.text = "" + maxPlayers;
    }

    void OnEnable()
    {
        allowedAbilities[0] = allowedAbilities[1] = allowedAbilities[2] = true;
        checkMarks[0].enabled = checkMarks[1].enabled = checkMarks[2].enabled = true;

        
        foreach (Button b in allAvailableButtons)
        {
            b.interactable = true;
        }
    }

    public void CreateRoom()
    {
        if (!PhotonNetwork.IsConnectedAndReady)
            return;

        foreach(Button b in allAvailableButtons)
        {
            b.interactable = false;
        }

        RoomOptions roomOptions = new RoomOptions();
        
        // Keys should be as short as possible according to Photon documentation so rocketThrust = r, rocketJump = j, and teleport = t
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
        roomOptions.CustomRoomProperties.Add("r", allowedAbilities[0]);
        roomOptions.CustomRoomProperties.Add("j", allowedAbilities[1]);
        roomOptions.CustomRoomProperties.Add("t", allowedAbilities[2]);

        roomOptions.MaxPlayers = maxPlayers;
        PhotonNetwork.CreateRoom(roomName.text, roomOptions, TypedLobby.Default);

        StartCoroutine(WaitForRoomCreation());
    }

    // OnCreateRoomFail sometimes doesn't call so this is a safety measure to make sure we unlock the buttons for user after 6 seconds and rename their lobby 
    IEnumerator WaitForRoomCreation()
    {
        yield return new WaitForSeconds(6);

        roomNameInput.text = roomNameInput.text + "(1)";
        OnCreateRoomFailed(0, "IEnumerator called room failed");
    }

    public void SetRoomName(string name)
    {
        createRoomButton.interactable = !string.IsNullOrEmpty(name);
    }

    public override void OnCreatedRoom()
    {
        //StopCoroutine(WaitForRoomCreation());

        Debug.Log("Created room succesfully");
        createdRoom.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room creation failed!!!!!!!!");

        foreach (Button b in allAvailableButtons)
        {
            b.interactable = true;
        }
    }


    public void DecreaseMaxPlayers()
    {
        maxPlayers--;
        maxPlayers = (byte)Mathf.Clamp(maxPlayers, 2, 8);
        maxPlayersInput.text = "" + maxPlayers;
    }

    public void IncreaseMaxPlayers()
    {
        maxPlayers++;
        maxPlayers = (byte)Mathf.Clamp(maxPlayers, 2, 8);
        maxPlayersInput.text = "" + maxPlayers;
    }

    public void ClampMaxPlayers()
    {
        maxPlayers = (byte)maxPlayersInput.text[0];
        maxPlayers = (byte)Mathf.Clamp(maxPlayers, 2, 8);
        maxPlayersInput.text = "" + maxPlayers;
    }

    public void CheckAbility(int abilityNumber)
    {
        if(abilityNumber == 0)
        {
            checkMarks[0].enabled = !checkMarks[0].enabled;
            allowedAbilities[0] = !allowedAbilities[0];
        }
        else if(abilityNumber == 1)
        {
            checkMarks[1].enabled = !checkMarks[1].enabled;
            allowedAbilities[1] = !allowedAbilities[1];
        }
        else
        {
            checkMarks[2].enabled = !checkMarks[2].enabled;
            allowedAbilities[2] = !allowedAbilities[2];
        }
    }

}


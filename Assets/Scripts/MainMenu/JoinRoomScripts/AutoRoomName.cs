using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AutoRoomName : MonoBehaviour
{
    [SerializeField] private TMP_InputField roomNameInput = null;
    [SerializeField] private TMP_Text roomNameField = null;

    
    void OnEnable()
    {
        roomNameInput.text = "" + PhotonNetwork.LocalPlayer.NickName + "'s Room"; 
        roomNameField.text = "" + PhotonNetwork.LocalPlayer.NickName + "'s Room";
    }
}

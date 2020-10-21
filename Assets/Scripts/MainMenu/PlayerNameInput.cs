using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PhotonTutorial.Menus
{
    public class PlayerNameInput : MonoBehaviour
    {

        [SerializeField] private TMP_InputField nameInputField = null;
        [SerializeField] private Button continueButton = null;
        
        private const string PlayerPrefsNameKey = "PlayerName";

        private void OnEnable()
        {
            SetUpInputField();
        }
        

        private void SetUpInputField()
        {
            if (!PlayerPrefs.HasKey(PlayerPrefsNameKey)) { return; }

            string defaultName = PlayerPrefs.GetString(PlayerPrefsNameKey);

            nameInputField.text = defaultName;

            SetPlayerName(defaultName);
            
        }

        public void SetPlayerName(string name)
        {
            if(PhotonNetwork.IsConnected)
            {
                continueButton.interactable = !string.IsNullOrEmpty(name);
            }
        }

        public void SavePlayerName()
        {
            string playerName = nameInputField.text;
            
            PhotonNetwork.NickName = playerName;

            PlayerPrefs.SetString(PlayerPrefsNameKey, playerName);

            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();  //Join lobby after entering username so that the lobby's room list is immediately updated with correct rooms
            }
                
                
        }
    }
}

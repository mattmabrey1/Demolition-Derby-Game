using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PhotonTutorial.Menus
{
    public class MainMenu : MonoBehaviourPunCallbacks
    {
        // Panel References to activate/deactivate when connected
        [SerializeField]
        private GameObject startScreen = null;
        [SerializeField]
        private GameObject nameInput = null;
        [SerializeField]
        private GameObject mainMenu = null;
        [SerializeField]
        private Button playButton = null;
        [SerializeField]
        private GameObject connectingText = null;

        // Key to hold user's name
        private const string PlayerPrefsNameKey = "PlayerName";

        // Game version to prevent different game versions from playing together
        private const string GameVersion = "1.3";

        private void Awake() => PhotonNetwork.AutomaticallySyncScene = true;

        void Start()
        {
            // if user is not connected start connecting
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = GameVersion;

                
                // if player is not first time user, get last username and ignore PLAY button (used on first time load up only)
                if (PlayerPrefs.HasKey(PlayerPrefsNameKey))
                {
                    playButton.gameObject.SetActive(false);
                    connectingText.SetActive(true);
                }
                
                
            }
            else
            {
                startScreen.SetActive(false);
                mainMenu.SetActive(true);
            }
        }

        // Called when the player has successfully connected to the master server
        public override void OnConnectedToMaster()
        {
            // If the start screen is active (e.g. you just started up the game, and you're not reconnecting from leaving a room)
            if (startScreen.activeSelf)
            {
                Debug.Log("Connected to Photon server");

                playButton.interactable = true;

                
                // Try to get players username if it exists
                if (PlayerPrefs.HasKey(PlayerPrefsNameKey))
                {
                    SavePlayerName();
                    mainMenu.SetActive(true);
                    startScreen.SetActive(false);
                }
                
            }
            else if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();  //Join lobby after reconnecting from leaving a room
            }

        }

        // Move to name input screen from start screen
        public void Play()
        {
            startScreen.SetActive(false);
            nameInput.SetActive(true);
        }

        public void OnClick_Quit()
        {
            Application.Quit();
        }

        // Saving player name from previous gameplay session
        private void SavePlayerName()
        {
            string playerName = PlayerPrefs.GetString(PlayerPrefsNameKey);

            PhotonNetwork.NickName = playerName;

            PlayerPrefs.SetString(PlayerPrefsNameKey, playerName);

            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();  //Join lobby after entering username so that the lobby's room list is immediately updated with correct rooms
            }


        }

        public override void OnJoinedLobby()
        {
            Debug.Log("Connected to Photon LOBBY");
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("Disconnected due to: " + cause);
        }
    }
}

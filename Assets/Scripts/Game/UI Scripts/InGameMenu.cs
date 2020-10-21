using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.VFX;

public class InGameMenu : MonoBehaviourPunCallbacks
{
    bool escapemenu = false;

    [SerializeField]
    private GameObject mainCanvas = null;

    [SerializeField]
    private GameObject Panel_GameOver = null;

    [SerializeField]
    private GameObject Panel_EscapeMenu = null;

    private bool gameOver = false;
    private bool goingBackToLobby = false;


    // Controls references 
    [SerializeField]
    private CameraController camController = null;
    private Car_Control carController = null;

    // Particle System references for quality settings
    [SerializeField]
    private VisualEffect[] dustStormVFX = new VisualEffect[2];

    [SerializeField]
    private ParticleSystem[] fireBrazierVFX = new ParticleSystem[2];

    void Start()
    {
        GameObject playerObj = (GameObject)PhotonNetwork.LocalPlayer.TagObject;

        carController = playerObj.GetComponent<Car_Control>();

        // 0 low, 1 medium, 2 high

        if (QualitySettings.GetQualityLevel() == 0)
        {
            // Turn off all VFX for low quality 
            foreach (VisualEffect vfx in dustStormVFX)
            {
                vfx.Stop();
                vfx.gameObject.SetActive(false);
            }

            foreach (ParticleSystem vfx in fireBrazierVFX)
            {
                vfx.Stop();
                vfx.gameObject.SetActive(false);
            }

        }
        else if (QualitySettings.GetQualityLevel() == 1)
        {
            // Turn off some VFX for medium quality 
            dustStormVFX[0].gameObject.SetActive(true);
            dustStormVFX[0].Play();
            dustStormVFX[1].gameObject.SetActive(true);
            dustStormVFX[1].Play();
            dustStormVFX[2].Stop();
            dustStormVFX[2].gameObject.SetActive(false);
            dustStormVFX[3].Stop();
            dustStormVFX[3].gameObject.SetActive(false);

            foreach (ParticleSystem vfx in fireBrazierVFX)
            {
                vfx.gameObject.SetActive(true);
                vfx.Play();
            }

        }
        else if (QualitySettings.GetQualityLevel() == 2)
        {
            // Turn on all VFX for high quality 

            foreach (VisualEffect vfx in dustStormVFX)
            {
                vfx.gameObject.SetActive(true);
                vfx.Play();
            }

            foreach (ParticleSystem vfx in fireBrazierVFX)
            {
                vfx.gameObject.SetActive(true);
                vfx.Play();
            }
        }
        
    }


    // Update is called once per frame
    void Update()
    {
        if (!gameOver)
        {
            // Once the game has gotten down to one player left, turn off UI and set gameOver
            if (PhotonNetwork.PlayerList.Length <= 1 || World.currentWorld.playersAlive <= 1) 
            {
                mainCanvas.SetActive(false);
                Panel_GameOver.SetActive(true);
                Panel_EscapeMenu.SetActive(false);
                gameOver = true;

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

            }
            else if(Input.GetKeyDown("escape"))
            {
                // Turning in game escape menu on and off 

                escapemenu = !escapemenu;

                if (escapemenu)
                {
                    camController.SetInputsActive(false);
                    carController.SetInputsActive(false);
                    
                    Panel_EscapeMenu.SetActive(true);

                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
                else
                {
                    camController.SetInputsActive(true);
                    carController.SetInputsActive(true);
                    
                    Panel_EscapeMenu.SetActive(false);
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }

            

        }


    }

    public void OnClick_ReturnToMenu()
    {
        goingBackToLobby = true;
        gameOver = true;

        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            PhotonNetwork.Disconnect();
        }

    }

    public override void OnLeftRoom()
    {
        if (goingBackToLobby)
        {
            PhotonNetwork.LoadLevel("MainMenu");
        }
        

    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (goingBackToLobby)
        {
            PhotonNetwork.LoadLevel("MainMenu");
        }
        
    }

    public void OnClick_Quit()
    {
        gameOver = true;
        Application.Quit();
    }


}

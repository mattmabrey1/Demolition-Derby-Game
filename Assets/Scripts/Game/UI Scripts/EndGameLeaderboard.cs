using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameLeaderboard : MonoBehaviour
{
    
   
    private void OnEnable()
    {
        GameObject localPlayer = (GameObject)PhotonNetwork.LocalPlayer.TagObject;
        
        if (localPlayer != null)
        {

            if (localPlayer.GetPhotonView().ViewID == World.currentWorld.playerList[0].playerID)
            {
                GameObject winnerObj = World.currentWorld.playerList[0].playerGameObject;

                Car winnerCar = winnerObj.GetComponent<Car>();

                winnerCar.RemoveBomb();
                
                CameraController cameraRig = GameObject.FindGameObjectWithTag("CameraRig").GetComponent<CameraController>();
                
                cameraRig.SetGameOverPosition();

                Car_Control winnerControl = winnerObj.GetComponent<Car_Control>();

                winnerControl.enabled = false;
                winnerControl.MotorForce = 0;
                winnerControl.SteerForce = 0;

                byte evCode = 4;                                                                                    // Custom Event 4: Used as "DisplayLeaderboard" event
                object[] content = new object[] { };                                                                // Name of random car to recieve the bomb after past bomb holder blows up
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                SendOptions sendOptions = new SendOptions { Reliability = true };
                PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);

            }
        }
        


       
        
    }
    
    
}

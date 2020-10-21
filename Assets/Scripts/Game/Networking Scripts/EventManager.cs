using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : MonoBehaviourPunCallbacks, IOnEventCallback, IPunCallbacks
{

    private readonly byte CarCollision = 1;
    private readonly byte BombHolderDies = 2;
    private readonly byte FirstBombHolder = 3;
    private readonly byte DisplayLeaderboard = 4;
    private readonly byte GameStart = 5;

    private Stack<Player> playerRanks;

    [SerializeField]
    private LeaderboardListing[] leaderboardListings = new LeaderboardListing[1];
    
    [SerializeField]
    private GameObject chargeMeter = null, waitingText = null;

    public override void OnEnable()
    {
        playerRanks = new Stack<Player>(PhotonNetwork.PlayerList.Length);

        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable() => PhotonNetwork.RemoveCallbackTarget(this);

    // Events called in other scripts are processed here. One client will send an event and all players will call it
    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        // Event for when cars collide and bomb must transfer from one car to another
        if (eventCode == CarCollision)
        {
            object[] data = (object[])photonEvent.CustomData;

            // Get the Photon ID's of both cars involved in car collision
            short bombHolderID = (short)data[0];
            short targetID = (short)data[1];

            // Get the Gameobjects of both cars 
            GameObject bombHolder = PhotonNetwork.GetPhotonView(bombHolderID).gameObject;
            GameObject target = PhotonNetwork.GetPhotonView(targetID).gameObject;

            // Getting the car components of the gameobjects so we can set and remove bombs
            Car bombHolderCar = bombHolder.GetComponent<Car>();
            Car targetCar = target.GetComponent<Car>();

            // If the target car isn't dead (so the bomb doesn't get lost) 
            if (bombHolderCar.hasBomb && !targetCar.dead)
            {
                bombHolderCar.hasBomb = false;
                targetCar.hasBomb = true;                           // transfer bomb to car collided with

                bombHolderCar.RemoveBomb();
                targetCar.SetBomb();

                bombHolderCar.timeHeld = targetCar.timeHeld = 0;    // set timeHeld back to zero so when it gets bomb again it has to wait to transfer

                // Timer decrease is increased everytime the bomb transfers, e.g. (maxTime - timerDecrease)
                World.currentWorld.timerDecrease += 1;

                World.currentWorld.startTime = PhotonNetwork.Time;

                // Decrease timer's max time everytime transfer occurs
                World.currentWorld.bombTimer = World.currentWorld.maxBombTimer = Mathf.Clamp(30 - World.currentWorld.timerDecrease, 5, 30);
            }


        }
        else if (eventCode == BombHolderDies)
        {
            object[] data = (object[])photonEvent.CustomData;

            short bombHolderID = (short)data[0];
            short targetID = (short)data[1];
            bool isGameOver = (bool)data[2];

            GameObject bombHolder = PhotonNetwork.GetPhotonView(bombHolderID).gameObject;
            GameObject target = PhotonNetwork.GetPhotonView(targetID).gameObject;

            Car bombHolderCar = bombHolder.GetComponent<Car>();
            Car targetCar = target.GetComponent<Car>();

            if (World.currentWorld.playerList.Exists(t => t.playerGameObject == bombHolder))
            {
                World.UnityPlayer bombPlayer = World.currentWorld.playerList.Find(t => t.playerGameObject == bombHolder);
                World.currentWorld.playerList.Remove(bombPlayer);
            }
            

            bombHolderCar.RemoveBomb();

            bombHolderCar.SetCarDead();

            playerRanks.Push(PhotonNetwork.GetPhotonView(bombHolderID).Owner);

            if (!isGameOver)
            {
                targetCar.SetBomb();                                // transfer bomb to car collided with

                bombHolderCar.timeHeld = targetCar.timeHeld = 0;    // set timeHeld back to zero so when it gets bomb again it has to wait to transfer

                // Timer decrease is increased everytime the bomb transfers, e.g. (maxTime - timerDecrease)
                World.currentWorld.timerDecrease += 1;

                World.currentWorld.startTime = PhotonNetwork.Time;

                // Decrease timer's max time everytime transfer occurs
                World.currentWorld.bombTimer = World.currentWorld.maxBombTimer = Mathf.Clamp(30 - World.currentWorld.timerDecrease, 5, 30);
            }
            else
            {
                targetCar.RemoveBomb();
            }



        }
        else if(eventCode == FirstBombHolder)
        {
            object[] data = (object[])photonEvent.CustomData;
            
            short targetID = (short)data[0];

            GameObject target = PhotonNetwork.GetPhotonView(targetID).gameObject;

            Car targetCar = target.GetComponent<Car>();

            targetCar.SetBomb();

            targetCar.timeHeld = 0;    // set timeHeld back to zero so when it gets bomb again it has to wait to transfer

            World.currentWorld.startTime = PhotonNetwork.Time;

            // Decrease timer's max time everytime transfer occurs
            World.currentWorld.bombTimer = World.currentWorld.maxBombTimer = Mathf.Clamp(30 - World.currentWorld.timerDecrease, 5, 30);

            // Timer decrease is increased everytime the bomb transfers, e.g. (maxTime - timerDecrease)
            World.currentWorld.timerDecrease += 1;

        }
        else if (eventCode == DisplayLeaderboard)
        {
            // Add the last living player (the winner) to the player ranks stack
            playerRanks.Push(PhotonNetwork.GetPhotonView(World.currentWorld.playerList[0].playerID).Owner);

            int stackSize = playerRanks.Count;
            
            // List the end game leaderboard based on when the player was pushed to the playerRank stack
            for (int i = 0; i < stackSize; i++)
            {
                leaderboardListings[i].SetPlayerInfo(playerRanks.Pop());
            }

            
        }
        else if (eventCode == GameStart)
        {
            // Allow player control of their car and show ability UI once everyone has connected 
            if(PhotonNetwork.LocalPlayer.TagObject != null)
            {
                GameObject playerObj = (GameObject)PhotonNetwork.LocalPlayer.TagObject;

                chargeMeter.SetActive(true);
                waitingText.SetActive(false);
                
                Car_Control carControl = playerObj.GetComponent<Car_Control>();

                carControl.SetInputsActive(true);
            }
            
        }

    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        
        // Getting the left players object still on our local client 
        GameObject playerObj = (GameObject)otherPlayer.TagObject;

        // Check that the players object still exists on our local client before doing anything
        if (playerObj != null && World.currentWorld.playerList.Exists(t => t.playerGameObject == playerObj))
        {
            Car playerCar = playerObj.GetComponent<Car>();

            World.UnityPlayer leavingPlayer = World.currentWorld.playerList.Find(t => t.playerGameObject == playerObj);

            World.currentWorld.playerList.Remove(leavingPlayer);                                                    //Remove this player from the list so to not target them with the next bomb

            playerCar.RemoveBomb();

            playerCar.SetCarDead();

            playerRanks.Push(otherPlayer);

            if (playerCar.hasBomb == true && PhotonNetwork.IsMasterClient && World.currentWorld.playerList.Count > 1)
            {

                int randomInt = Random.Range(0, World.currentWorld.playerList.Count);                               // Choice for new target accounting for one less player alive when this car is removed
                int targetID = World.currentWorld.playerList[randomInt].playerID;

                byte evCode = 3;                                                                                    // Custom Event 3: Used as "FirstBombHolder" event
                object[] content = new object[] { (short)targetID };                                                       // current bomb holder and a boolean to tell whether the car died or not
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                SendOptions sendOptions = new SendOptions { Reliability = true };
                PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);

                PhotonNetwork.SendAllOutgoingCommands();                                                            //Send outgoing event immediately before the application closes and it fails

            }
        }


    }
}

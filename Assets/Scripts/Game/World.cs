using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class World : MonoBehaviourPun
{
    
    public List<UnityPlayer> playerList = new List<UnityPlayer>();

    public static World currentWorld;

    public double startTime;
    public double bombTimer = 30, maxBombTimer = 30;

    public int timerDecrease = -1;  // value used to decrease timer every time bomb successfully blows up

    public int playersAlive = 0;

    private bool allPlayersReady = false, bombExists = false;
    
    void Awake()
    {
        
        currentWorld = this;

        playersAlive = PhotonNetwork.PlayerList.Length;

        StartCoroutine(CheckIfReady());

        
    }

    // Check if all players are loaded in and ready to start the game before enabling car control
    IEnumerator CheckIfReady()
    {
        while (!allPlayersReady)
        {
            allPlayersReady = true;

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                GameObject playerObj = (GameObject)p.TagObject;
                
                if (p.TagObject == null)
                {
                    allPlayersReady = false;
                }
                else if(playerObj == null)
                {
                    allPlayersReady = false;
                }
            }

            yield return new WaitForSeconds(1f);
        }

        // Call Game Start
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            byte evCode = 5;                                   // Custom Event 5: Used as "GameStart" event
            object[] content = new object[] { };               // No parameters sent
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            SendOptions sendOptions = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);
        }

        StartCoroutine(LateStart());    //// Wait to apply the first bomb
    }

    // Wait for a few seconds to send out the first bomb
    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(5f);

        if (PhotonNetwork.IsMasterClient && PhotonNetwork.PlayerList.Length > 1 )
        {
            firstBombTarget();
        }
    }

    // Make sure there is always an active bomb out so the game can continue
    IEnumerator CheckForActiveBomb()
    {

        while (true)
        {

            if (PhotonNetwork.LocalPlayer.IsMasterClient && PhotonNetwork.PlayerList.Length > 1 && allPlayersReady)
            {
                bombExists = false;

                foreach(UnityPlayer p in playerList)
                {
                    if (p.playerCar.hasBomb)
                    {
                        bombExists = true;
                    }
                }

                if (!bombExists)
                {
                    firstBombTarget();
                }
            }

            yield return new WaitForSeconds(5f);
        }
        

        
    }

    /*
    IEnumerator CheckForDuplicateBombs()
    {
        UnityPlayer chosenTarget;
        float lowestTimeHeld;

        List<UnityPlayer> bombHolders = new List<UnityPlayer>();

        while (true)
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient && PhotonNetwork.PlayerList.Length > 1 && allPlayersReady)
            {
                bombHolders.Clear();

                foreach (UnityPlayer p in playerList)
                {
                    if (p.playerCar.hasBomb)
                    {
                        bombHolders.Add(p);
                    }
                }

                if(bombHolders.Count > 1)
                {
                    lowestTimeHeld = 100;

                    foreach (UnityPlayer p in bombHolders)
                    {
                        if(p.playerCar.timeHeld < lowestTimeHeld)
                        {
                            chosenTarget = p;
                            lowestTimeHeld = p.playerCar.timeHeld;
                        }
                    }



                }

            }

            yield return new WaitForSeconds(1f);
        }
        
    }
    */

    // Update is called once per frame
    void FixedUpdate()
    {
        // Decreasing the bomb timer every update based on PhotonNetwork time
        bombTimer = maxBombTimer - (PhotonNetwork.Time - startTime);
    }


    void firstBombTarget()
    {
            int randomInt = Random.Range(0, playerList.Count);

            UnityPlayer target = playerList[randomInt];
            
            byte evCode = 3;                                                                                  // Custom Event 3: Used as "FirstBombHolder" event
            object[] content = new object[] { (short)target.playerGameObject.GetPhotonView().ViewID };               // Current bomb holder, random new target choice, and if the car is dead
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            SendOptions sendOptions = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);
            

    }
    
    public struct UnityPlayer
    {
        public string playerName;
        public GameObject playerGameObject;
        public int playerID;
        public Car playerCar;
    }

    public void AddPlayerToList(GameObject playerObj, string playerName, int ID, Car car)
    {
        UnityPlayer p = new UnityPlayer
        {
            playerGameObject = playerObj,
            playerName = playerName,
            playerID = ID,
            playerCar = car
        };

        playerList.Add(p);

    }
}

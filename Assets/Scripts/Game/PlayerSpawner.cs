using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviourPun
{
    [SerializeField] private GameObject[] playerPrefabs = new GameObject[6];

    public GameObject[] spawnpoints = new GameObject[8];




    public void Awake()
    {

        Cursor.visible = false;
        int carChoice = (int)PhotonNetwork.LocalPlayer.CustomProperties["carChoice"];
        int abilityChoice = (int)PhotonNetwork.LocalPlayer.CustomProperties["abilityChoice"];




        int spot = PhotonNetwork.LocalPlayer.ActorNumber % 10;

        Vector3 spawnLocation = spawnpoints[spot].transform.position;

        var player = PhotonNetwork.Instantiate(playerPrefabs[carChoice].name,
                                  spawnLocation,
                                  Quaternion.identity,
                                  0);



        player.GetComponent<Car_Special_Ability>().SetAbility(abilityChoice);

        player.GetPhotonView().isRuntimeInstantiated = true;


    }
    
    
}

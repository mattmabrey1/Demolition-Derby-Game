using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CarSelect : MonoBehaviour
{
    private Transform currentCar;

    [SerializeField]
    private GameObject[] carPrefabs = new GameObject[6];

    [SerializeField]
    private TMP_Text carTitle = null;

    private int i = 0;

    private void Start()
    {
        currentCar = carPrefabs[i].transform;
        setModel(i);
    }

    void Update()
    {
        currentCar.Rotate(0f, 25 * Time.deltaTime, 0f, Space.Self);
    }

    public void nextModel()
    {
        carPrefabs[i].SetActive(false);

        if(i >= 5)
        { i = 0;  }
        else 
        { i++;   }


        carPrefabs[i].transform.rotation = currentCar.rotation;
        currentCar = carPrefabs[i].transform;

        carPrefabs[i].SetActive(true);
        setModel(i);

    }

    public void pastModel()
    {
        carPrefabs[i].SetActive(false);

        if (i <= 0)
        { i = 5; }
        else
        { i--; }


        carPrefabs[i].transform.rotation = currentCar.rotation;
        currentCar = carPrefabs[i].transform;

        carPrefabs[i].SetActive(true);
        setModel(i);
        
    }

    private void setModel(int i)    //Sets model for this player across the network
    {
        int carChoice = i;
        Hashtable hash = new Hashtable();
        hash.Add("carChoice", carChoice);
        PhotonNetwork.SetPlayerCustomProperties(hash);


        if (i == 0 || i == 1)
        {
            carTitle.text = "Heavy";
        }
        else if (i == 2 || i == 3)
        {
            carTitle.text = "Standard";
        }
        else
        {
            carTitle.text = "Special";
        }
    }
    
}

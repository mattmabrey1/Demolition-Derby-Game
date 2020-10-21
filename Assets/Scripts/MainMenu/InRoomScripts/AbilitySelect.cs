using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.UI;
using Photon.Realtime;
using TMPro;

public class AbilitySelect : MonoBehaviour
{

    [SerializeField]
    private Image leftArrow = null, rightArrow = null;

    [SerializeField]
    private GameObject[] abilities = new GameObject[3];

    private GameObject currentObj;
    
    private List<int> enabledAbilities = new List<int>();

    public int i = 0, maxSprites;


    [SerializeField]
    private TMP_Text abilityName = null;

    [SerializeField]
    private GameObject abilityTitle = null;
    
    private void OnEnable()
    {
        abilityTitle.SetActive(true);
        leftArrow.enabled = rightArrow.enabled = true;
        
        Hashtable roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;

        bool rocketThrust = (bool)roomProperties["r"];
        bool rocketJump = (bool)roomProperties["j"];
        bool teleport = (bool)roomProperties["t"];

        if (rocketThrust || rocketJump || teleport)
        {

            if (rocketThrust)
            {
                enabledAbilities.Add(0);
            }

            if (rocketJump)
            {
                enabledAbilities.Add(1);
            }

            if (teleport)
            {
                enabledAbilities.Add(2);
            }


            maxSprites = enabledAbilities.Count - 1;

            abilities[enabledAbilities[0]].SetActive(true);

            currentObj = abilities[enabledAbilities[0]];
            
            abilityName.text = abilities[enabledAbilities[0]].name;

            SetAbility(enabledAbilities[0]);

            if(maxSprites < 2)
            {   leftArrow.enabled = rightArrow.enabled = false;     }
        }
        else
        {
            abilityTitle.SetActive(false);
            leftArrow.enabled = rightArrow.enabled = false;
            SetAbility(-1);
        }

        
    }

    private void OnDisable()
    {
        enabledAbilities.Clear();
        i = maxSprites = 0;
        currentObj = null;


        abilities[0].SetActive(false);
        abilities[1].SetActive(false);
        abilities[2].SetActive(false);
    }
    

    public void nextAbility()
    {
        if (i >= maxSprites)
        { i = 0; }
        else
        { i++; }


        SetImage(enabledAbilities[i]);
        SetAbility(enabledAbilities[i]);
    }

    public void pastAbility()
    {
        if (i <= 0)
        { i = maxSprites; }
        else
        { i--; }


        SetImage(enabledAbilities[i]);
        SetAbility(enabledAbilities[i]);
    }

    private void SetImage(int x)
    {
        abilities[x].SetActive(true);

        abilityName.text = abilities[x].name;

        currentObj.SetActive(false);

        currentObj = abilities[x];
    }

    private void SetAbility(int y)    //Sets ability choice for this player across the network
    {
        int abilityChoice = y;
        Hashtable hash = new Hashtable();
        hash.Add("abilityChoice", abilityChoice);
        PhotonNetwork.SetPlayerCustomProperties(hash);
    }
}

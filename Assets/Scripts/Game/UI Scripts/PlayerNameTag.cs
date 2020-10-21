using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNameTag : MonoBehaviourPun
{
    [SerializeField]
    private TextMeshProUGUI nameText = null;

    void Start()
    {
        gameObject.name = photonView.Owner.NickName;

        if (photonView.IsMine) { return; }

        SetName();
    }

    private void SetName()
    {
        
        nameText.text = photonView.Owner.NickName;
        
    }

    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerListing : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _text = null;

    public Player _player = null;

    public RoomInfo _RoomInfo { get; set; }

    [SerializeField]
    private RawImage listingBackground = null;

    [SerializeField]
    private Color emptyColor = new Color(), fullColor = new Color(), textFullColor = new Color(), textEmptyColor = new Color();

    
    public void SetPlayerInfo(Player player)
    {
        _player = player;

        if (player.IsMasterClient)
        { _text.text = player.NickName + " [Room Owner]"; }
        else
        {
            _text.text = player.NickName;
        }

        _text.color = textFullColor;
        listingBackground.color = fullColor;
    }
    

    public void ClearListing()
    {
        _player = null;

        _text.text = "Empty";

        _text.color = textEmptyColor;
        listingBackground.color = emptyColor;
    }
   
}

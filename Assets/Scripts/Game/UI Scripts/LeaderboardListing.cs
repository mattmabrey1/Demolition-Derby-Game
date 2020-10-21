using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;

public class LeaderboardListing : MonoBehaviour
{
    [SerializeField]
    private TMP_Text rankText = null;
    [SerializeField]
    private TMP_Text nameText = null;
    public Player _player = null;

    public RoomInfo _RoomInfo { get; set; }

    private RawImage listingBackground;

    [SerializeField]
    private Color fullBackgroundColor = new Color();

    [SerializeField]
    private Color fullTextColor = new Color();

    void Start()
    {
        listingBackground = GetComponent<RawImage>();
    }

    public void SetPlayerInfo(Player player)
    {
        _player = player;
        nameText.text = player.NickName;

        listingBackground.color = fullBackgroundColor;

        nameText.color = fullTextColor;
        rankText.color = fullTextColor;
    }

}

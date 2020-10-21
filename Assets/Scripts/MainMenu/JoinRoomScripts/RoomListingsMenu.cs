using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomListingsMenu : MonoBehaviourPunCallbacks, ILobbyCallbacks
{


    [SerializeField]
    private Transform content = null;
    [SerializeField]
    private RoomListing roomListing = null;
    [SerializeField]
    private GameObject Panel_RoomListings = null;
    [SerializeField]
    private GameObject Panel_CurrentRoom = null;

    List<RoomListing> listings = new List<RoomListing>();

    

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        
        
        foreach (RoomInfo info in roomList)
        {
            if (info.RemovedFromList)
            {
                int index = listings.FindIndex(x => x._RoomInfo.Name == info.Name);  
                if (index != -1)
                {
                    Destroy(listings[index].gameObject);
                    listings.RemoveAt(index);
                }
                
            }
            else
            {
                int index = listings.FindIndex(x => x._RoomInfo.Name == info.Name);
                if (index == -1)
                {
                    RoomListing listing = Instantiate(roomListing, content);
                    if (listing != null)
                    {
                        listing.SetRoomInfo(info);
                        listings.Add(listing);
                    }
                }
                else
                {
                    listings[index].SetRoomInfo(info);
                }
                
            }
        }
        
    }
    
    
    public override void OnJoinedRoom()
    {
        Debug.Log("On Joined Room");
        Panel_RoomListings.SetActive(false);
        Panel_CurrentRoom.SetActive(true);

        foreach(RoomListing roomListing in listings)
        {
            Destroy(roomListing.gameObject);
        }

        listings.Clear();

    }


    


}

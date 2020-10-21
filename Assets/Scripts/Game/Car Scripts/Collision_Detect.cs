using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Collision_Detect : MonoBehaviourPun
{
    private Rigidbody thisBody;

    private AbilityChargeMeter chargeMeter;

    private PhotonView pView;

    private Car thisCar;

    private bool isHeavy = false;

    void Start()
    {
        pView = GetComponent<PhotonView>();
        thisBody = GetComponent<Rigidbody>();

        chargeMeter = GameObject.FindGameObjectWithTag("ChargeMeter").GetComponent<AbilityChargeMeter>();

        int carChoice = (int)PhotonNetwork.LocalPlayer.CustomProperties["carChoice"];

        if(carChoice < 2)
        {
            isHeavy = true;
        }

        thisCar = this.GetComponent<Car>();
    }


    void OnCollisionEnter(Collision col)  //when rigidbody collides with rigidbody
    {
        if (col.gameObject.tag.Equals("ThickCar") || col.gameObject.tag.Equals("ThinCar"))// if the hit rigidbody has component <Car> script
        {
            if (col.relativeVelocity.magnitude > 15 && pView.IsMine)
            {
                if ((thisBody.velocity.magnitude > col.rigidbody.velocity.magnitude) || (isHeavy))
                {
                    chargeMeter.AddCharge(col.relativeVelocity.magnitude);
                }
            }

            // if thisCar's <Car> script hasBomb = true, and it's held it for 2+ seconds, and it has collided with another rigidbody with <Car> script
            if (thisCar.hasBomb && (thisCar.timeHeld >= 2)) 
            {
                Car hitCar = col.collider.GetComponent<Car>();

                if (!hitCar.dead && World.currentWorld.playerList.Count > 1 && pView.IsMine)
                {

                    byte evCode = 1; // Custom Event 1: Used as "CarCollision" event
                    object[] content = new object[] { (short)this.gameObject.GetPhotonView().ViewID, (short)col.gameObject.GetPhotonView().ViewID }; // Names of both cars in collision
                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                    SendOptions sendOptions = new SendOptions { Reliability = true };
                    PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);
                }
                

            }

        }
  

    }

}

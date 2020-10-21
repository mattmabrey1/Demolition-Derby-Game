using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro.Examples;
using UnityEngine;

public class Car : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    
    public bool dead = false;
    public bool hasBomb = false;
    public bool winner = false;
    public float timeHeld = 0f;
    
    public ParticleSystem fire;

    private CameraController cameraRig;

    [SerializeField]
    private GameObject centerOfCar = null;

    private Rigidbody thisBody;

    private GameObject chargeMeterImg;

    void Start()
    {
        thisBody = GetComponent<Rigidbody>();
        chargeMeterImg = GameObject.FindGameObjectWithTag("ChargeMeter");
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        // e.g. store this gameobject as this player's charater in Player.TagObject
        info.Sender.TagObject = this.gameObject;


        if (info.photonView.IsMine)
        {
            cameraRig = GameObject.FindGameObjectWithTag("CameraRig").GetComponent<CameraController>();
            cameraRig.centerOfCar = centerOfCar;
            cameraRig.playerCar = this.GetComponent<Car>();
        }
        
        this.gameObject.name = info.Sender.NickName;
        

        World.currentWorld.AddPlayerToList(this.gameObject, this.gameObject.name, info.photonView.ViewID, this.GetComponent<Car>());
    }


    // Update is called once per frame
    void FixedUpdate()
    {
      
        if (hasBomb)
        {
            timeHeld += Time.deltaTime;      // while this Car object has the bomb, count up timeHeld for use in Collision_Detect script that only transfers bomb if you've held it for a certain amount of time. 
            

            if (World.currentWorld.bombTimer <= 0.5 && !dead)
            {
                dead = true;

                if (photonView.IsMine)
                {

                    World.UnityPlayer bombPlayer = World.currentWorld.playerList.Find(t => t.playerGameObject == this.gameObject);

                    int thisPlayerID = bombPlayer.playerID;

                    World.currentWorld.playerList.Remove(bombPlayer);
                    
                    int randomInt = Random.Range(0, World.currentWorld.playerList.Count);                       // Choice for new target accounting for one less player alive when this car is removed   

                    World.UnityPlayer target = World.currentWorld.playerList[randomInt];

                    bool isGameOver = false;

                    if(World.currentWorld.playerList.Count <= 1)
                    { isGameOver = true; }

                    byte evCode = 2;                                                                             // Custom Event 2: Used as "BombHolderDies" event
                    object[] content = new object[] { (short)thisPlayerID, (short)target.playerID, isGameOver };               // Current bomb holder, random new target choice, and if the car is dead
                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                    SendOptions sendOptions = new SendOptions { Reliability = true };
                    PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);
                    
                }
                
            }
            
        }
        
    }

    public void SetBomb()
    {
        hasBomb = true;
        fire.Play();
    }

    public void RemoveBomb()
    {
        hasBomb = false;
        fire.Pause();
        fire.Clear();
    }


    // Instantiate new dead car across all clients when a player dies and make sure car control is turned off
    public void SetCarDead()
    {
        
        Car_Control carControl = this.gameObject.GetComponent<Car_Control>();
        carControl.MotorForce = 0;
        carControl.SteerForce = 0;
        carControl.enabled = false;
        

        World.currentWorld.playersAlive--;
        dead = true;

        if (photonView.IsMine)
        {
            cameraRig = GameObject.FindGameObjectWithTag("CameraRig").GetComponent<CameraController>();
            cameraRig.SetGameOverPosition();
            chargeMeterImg.SetActive(false);
        }
        

        if (PhotonNetwork.IsMasterClient)
        {

            object[] customInitData = new object[] { thisBody.velocity, thisBody.angularVelocity};
            
            if (this.gameObject.tag.Equals("ThinCar"))
            {
                PhotonNetwork.InstantiateSceneObject("Destroyed_ThinCar", this.transform.position, this.transform.rotation, 0, customInitData);
                
            }
            else
            {
                PhotonNetwork.InstantiateSceneObject("Destroyed_ThickCar", this.transform.position, this.transform.rotation, 0, customInitData);
                
            }


        }
        

        Destroy(this.gameObject);

        
        
    }

}

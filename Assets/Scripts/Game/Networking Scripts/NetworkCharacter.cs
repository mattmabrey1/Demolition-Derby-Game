using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCharacter : MonoBehaviourPun, IPunObservable
{
    private float distanceDifference;
    private float angleDifference;

    private Vector3 networkPosition;
    private Quaternion networkRotation;

    private Rigidbody rigidBody;

    [SerializeField]
    private WheelCollider[] localWheelColliders = new WheelCollider[4];

    private float lag;

    private float localDissolveStep = -1, networkDissolveStep = -1, localDissolveDelta, networkDissolveDelta;

    private int networkSteerAngle, networkMotorTorque, networkBrakeTorque, networkTorqueBack, networkBrakeTorqueBack, steerDifference, torqueDifference1, brakeDifference1, torqueDifference2, brakeDifference2;

    private bool isTeleporting = false, networkIsTeleporting = false, teleportParticles = false, networkTeleportParticles = false;

    private bool endDissolving = false, networkEndDissolving = false;

    private bool isRocketing = false, networkIsRocketing = false;

    private bool isJumping = false, networkIsJumping = false;

    

    private Car_Special_Ability specialAbilities;

    [SerializeField]
    private MeshRenderer[] carMeshes = new MeshRenderer[0];
    private MaterialPropertyBlock _propBlock;

    // Start is called before the first frame update
    void Awake()
    {
        networkPosition = new Vector3();
        networkRotation = new Quaternion();

        rigidBody = GetComponent<Rigidbody>();

        _propBlock = new MaterialPropertyBlock();

        specialAbilities = GetComponent<Car_Special_Ability>();

        int abilityChoice = specialAbilities.GetAbilityChoice();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        if(!photonView.IsMine)
        {
            
            this.rigidBody.position = Vector3.MoveTowards(this.rigidBody.position, this.networkPosition, this.distanceDifference * (1.0f / PhotonNetwork.SerializationRate));
            this.rigidBody.rotation = Quaternion.RotateTowards(this.rigidBody.rotation, this.networkRotation, this.angleDifference * (1.0f / PhotonNetwork.SerializationRate));


            localWheelColliders[0].steerAngle = Mathf.MoveTowards(localWheelColliders[0].steerAngle, networkSteerAngle, steerDifference * (1.0f / PhotonNetwork.SerializationRate));
            localWheelColliders[1].steerAngle = Mathf.MoveTowards(localWheelColliders[1].steerAngle, networkSteerAngle, steerDifference * (1.0f / PhotonNetwork.SerializationRate));

            localWheelColliders[0].motorTorque = localWheelColliders[1].motorTorque = networkMotorTorque;//Mathf.MoveTowards(localWheelColliders[0].motorTorque, networkTorqueFront, (1.0f / PhotonNetwork.SerializationRate));
            localWheelColliders[0].brakeTorque = localWheelColliders[1].brakeTorque = networkBrakeTorque;//Mathf.MoveTowards(localWheelColliders[0].brakeTorque, networkBrakeTorqueFront, (1.0f / PhotonNetwork.SerializationRate));
            localWheelColliders[2].motorTorque = localWheelColliders[3].motorTorque = networkMotorTorque;//Mathf.MoveTowards(localWheelColliders[2].motorTorque, networkTorqueBack, (1.0f / PhotonNetwork.SerializationRate));
            localWheelColliders[2].brakeTorque = localWheelColliders[3].brakeTorque = networkBrakeTorque;//Mathf.MoveTowards(localWheelColliders[2].brakeTorque, networkBrakeTorqueBack, (1.0f / PhotonNetwork.SerializationRate));
            

            // If the dissolve step recieved for this client is less than fully opaque, start dissolving
            if (networkDissolveStep > -1 || localDissolveStep > -1)
            {
                
                networkDissolveStep += networkDissolveDelta;
                networkDissolveStep = Mathf.Clamp(networkDissolveStep, -1, 1);
                
                float dissolveDifference = 3 * Mathf.Abs(networkDissolveStep - localDissolveStep);

                localDissolveStep = Mathf.MoveTowards(localDissolveStep, networkDissolveStep, dissolveDifference * (1.0f / PhotonNetwork.SerializationRate));

                localDissolveStep = Mathf.Clamp(localDissolveStep, -1, 1);
                
                //Changing the dissolve step for each material in carMeshes
                foreach (MeshRenderer m in carMeshes)
                {
                    m.GetPropertyBlock(_propBlock);
                    
                    // Assign our new value.
                    _propBlock.SetFloat("_TeleportEnable", 1);

                    // Assign our new value.
                    _propBlock.SetFloat("_DissolveStep", localDissolveStep);

                    // Apply the edited values to the renderer.
                    m.SetPropertyBlock(_propBlock);
                }
            }
            
            //Ending teleport dissolve effect by changing shader boolean _TeleportEnabled so that dithering effect is back to normal
            if (networkEndDissolving)
            {

                foreach (MeshRenderer m in carMeshes)
                {
                    m.GetPropertyBlock(_propBlock);

                    // Assign our new value.
                    _propBlock.SetFloat("_TeleportEnable", 0);

                    // Assign our new value.
                    _propBlock.SetFloat("_DissolveStep", -1);



                    // Apply the edited values to the renderer.
                    m.SetPropertyBlock(_propBlock);
                }

                networkEndDissolving = false;
                localDissolveStep = networkDissolveStep = -1;
            }


            if (networkIsRocketing)
            {
                isRocketing = true;
                specialAbilities.EnableRockets(true);
            }

            if (networkIsJumping)
            {
                networkIsJumping = false;
                specialAbilities.EnableJump();
            }

            if (networkTeleportParticles)
            {
                networkTeleportParticles = false;
                specialAbilities.EnableTeleportParticles();
            }

        }
    }

    public void SetTeleporting(bool enable)
    {
        isTeleporting = enable;
    }


    public void SetRocketThrusting(bool enable)
    {
        isRocketing = enable;
    }

    public void SetJumpingEffect()
    {
        isJumping = true;
    }

    public void SetTeleportParticles()
    {
        teleportParticles = true;
    }

    public void EndDissolving()
    {
        endDissolving = true;
        isTeleporting = false;

        localDissolveStep = -1;
    }

    public void SetDissolve(float step, float delta)
    {
        localDissolveStep = step;
        localDissolveDelta = delta;
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        if (stream.IsWriting)
        {
            // Send transform data
            stream.SendNext(this.rigidBody.position);
            stream.SendNext(this.rigidBody.rotation);
            
            stream.SendNext(isTeleporting);

            // Send all rigidbody car movement data
            stream.SendNext(this.rigidBody.velocity);
            stream.SendNext(this.rigidBody.angularVelocity);
            
            // Send car movement wheel collider data
            stream.SendNext((short)localWheelColliders[0].steerAngle);
            stream.SendNext((short)localWheelColliders[0].motorTorque);
            stream.SendNext((short)localWheelColliders[0].brakeTorque);

            // Send teleport dissolve data
            stream.SendNext(endDissolving);
            stream.SendNext(localDissolveStep);
            stream.SendNext(localDissolveDelta);

            stream.SendNext(isRocketing);
            stream.SendNext(isJumping);
            stream.SendNext(teleportParticles);

            if (isJumping)
            {
                isJumping = false;
            }

            if (teleportParticles)
            {
                teleportParticles = false;
            }

            if (endDissolving)
            {
                endDissolving = false;
            }
        }
        else
        {
            this.networkPosition = (Vector3)stream.ReceiveNext();
            this.networkRotation = (Quaternion)stream.ReceiveNext();

            this.networkIsTeleporting = (bool)stream.ReceiveNext();

            if (networkIsTeleporting)
            {
                if (Vector3.Distance(rigidBody.position, networkPosition) > 3)
                {
                    rigidBody.position = networkPosition;
                }
            }

            lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));

            // Syncronize Velocity
            this.rigidBody.velocity = (Vector3)stream.ReceiveNext();

            this.networkPosition += this.rigidBody.velocity * lag;

            this.distanceDifference = Vector3.Distance(this.rigidBody.position, this.networkPosition);

            // Syncronize Angular Velocity
            this.rigidBody.angularVelocity = (Vector3)stream.ReceiveNext();

            this.networkRotation = Quaternion.Euler(this.rigidBody.angularVelocity * lag) * this.networkRotation;

            this.angleDifference = Quaternion.Angle(this.rigidBody.rotation, this.networkRotation);
            

            // Syncronize Tire Motor and Brake Torque
            networkSteerAngle = (short)stream.ReceiveNext();

            networkMotorTorque = (short)stream.ReceiveNext();
            networkBrakeTorque = (short)stream.ReceiveNext();

            steerDifference = Mathf.Abs((int)localWheelColliders[0].steerAngle - Mathf.Abs(networkSteerAngle));

            networkEndDissolving = (bool)stream.ReceiveNext();

            networkDissolveStep = (float)stream.ReceiveNext();

            networkDissolveDelta = (float)stream.ReceiveNext();

            networkIsRocketing = (bool)stream.ReceiveNext();

            networkIsJumping = (bool)stream.ReceiveNext();

            networkTeleportParticles = (bool)stream.ReceiveNext();

            if (isRocketing && !networkIsRocketing)
            {
                specialAbilities.EnableRockets(false);
                isRocketing = false;
            }

        }
    }
}

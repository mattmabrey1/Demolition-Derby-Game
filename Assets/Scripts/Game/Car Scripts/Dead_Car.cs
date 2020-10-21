using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dead_Car : MonoBehaviour, IPunInstantiateMagicCallback
{
    [SerializeField]
    private ParticleSystem explosionParticles = null;
    [SerializeField]
    private ExplosionForce explosionForce = null;

    [SerializeField]
    private ParticleSystem smoke = null;

    [SerializeField]
    private Rigidbody thisRigidbody = null;
    
    private CameraController cameraRig;

    
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] instantiationData = info.photonView.InstantiationData;

        thisRigidbody.velocity = (Vector3)instantiationData[0];
        thisRigidbody.angularVelocity = (Vector3)instantiationData[1];

        cameraRig = GameObject.FindGameObjectWithTag("CameraRig").GetComponent<CameraController>();

        if (cameraRig.centerOfCar == null)
        {
            cameraRig.playerCar = null;
        }


        explosionParticles.Play();
        explosionForce.callExplosion();

        smoke.Play();
        
    }

    
    
}

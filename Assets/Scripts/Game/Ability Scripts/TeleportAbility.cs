using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TeleportAbility : MonoBehaviour
{

    // Teleport Variables

    [SerializeField]
    private LayerMask teleportLayerMask = 1;

    Vector3 teleportPosition = Vector3.zero;

    [SerializeField]
    private float teleportDistance = 35;
    [SerializeField]
    private Vector3 teleportSize = new Vector3(4,2,8);

    [SerializeField]
    private Transform carCenter = null;

    private float dissolveStep, dissolveGoal, dissolveDelta, lastStep;

    [SerializeField]
    private MeshRenderer[] carMeshes = new MeshRenderer[5];
    private MaterialPropertyBlock _propBlock;

    private NetworkCharacter networkCharacter = null;


    [SerializeField]
    private ParticleSystem teleportParticles = null;


    // Start is called before the first frame update
    void Start()
    {
        networkCharacter = GetComponent<NetworkCharacter>();
        _propBlock = new MaterialPropertyBlock();
    }


    public void CastTeleport()
    {

        StopAllCoroutines();
        StartCoroutine(TeleportFade());

    }

    // Begin teleport fade effect, then find a viable teleport spot, then teleport and reverse the teleport fade
    private IEnumerator TeleportFade()
    {
        
        dissolveStep = -1;

        dissolveGoal = 1;

        
        foreach (MeshRenderer m in carMeshes)
        {
            // Get the current value of the material properties in the renderer.
            m.GetPropertyBlock(_propBlock);

            // Assign our new value.
            _propBlock.SetFloat("_TeleportEnable", 1);

            // Apply the edited values to the renderer.
            m.SetPropertyBlock(_propBlock);
        }

        // Setting network to allow teleporting so objects don't smoothly move between two points
        networkCharacter.SetTeleporting(true);

        // Start teleport dissolve until invisible
        while (dissolveStep < 1)
        {
            lastStep = dissolveStep;
            dissolveStep = Mathf.MoveTowards(dissolveStep, dissolveGoal, 3f * Time.deltaTime);
            dissolveDelta = dissolveStep - lastStep;

            foreach (MeshRenderer m in carMeshes)
            {
                // Get the current value of the material properties in the renderer.
                m.GetPropertyBlock(_propBlock);

                // Assign our new value.
                _propBlock.SetFloat("_DissolveStep", dissolveStep);

                // Apply the edited values to the renderer.
                m.SetPropertyBlock(_propBlock);
            }


            networkCharacter.SetDissolve(dissolveStep, dissolveDelta);

            yield return new WaitForFixedUpdate();
        }

        // Get the furthest available teleport spot
        teleportPosition = FindTeleportSpot();

        Debug.DrawLine(carCenter.position, teleportPosition, Color.blue);
        
        // Teleport the player by giving them new position
        this.transform.position = teleportPosition;

        dissolveStep = 1;

        dissolveGoal = -1;

        // Shoot out teleport particles on every other players local clients
        networkCharacter.SetTeleportParticles();
        teleportParticles.Play();


        // Reverse teleport dissolve until visible
        while (dissolveStep > -1)
        {
            lastStep = dissolveStep;
            dissolveStep = Mathf.MoveTowards(dissolveStep, dissolveGoal, 3.5f * Time.deltaTime);
            dissolveDelta = dissolveStep - lastStep;
            

            foreach (MeshRenderer m in carMeshes)
            {
                // Get the current value of the material properties in the renderer.
                m.GetPropertyBlock(_propBlock);

                // Assign our new value.
                _propBlock.SetFloat("_DissolveStep", dissolveStep);

                // Apply the edited values to the renderer.
                m.SetPropertyBlock(_propBlock);
            }

            networkCharacter.SetDissolve(dissolveStep, dissolveDelta);

            yield return new WaitForFixedUpdate();

            
        }

        networkCharacter.SetDissolve(-1, 0);

        networkCharacter.SetTeleporting(false);

        networkCharacter.EndDissolving();

        // Turn off teleport enable so dithering can continue normally
        foreach (MeshRenderer m in carMeshes)
        {
            // Get the current value of the material properties in the renderer.
            m.GetPropertyBlock(_propBlock);

            // Assign our new value.
            _propBlock.SetFloat("_TeleportEnable", 0);

            // Apply the edited values to the renderer.
            m.SetPropertyBlock(_propBlock);
        }

        teleportParticles.Stop();
    }

    // Return the furthest available spot that can fit the players car if it teleported
    private Vector3 FindTeleportSpot()
    {

        Vector3 newPosition;
        
        Ray teleportRay = new Ray(carCenter.position, carCenter.forward);
        
        newPosition = teleportRay.GetPoint(teleportDistance);

        // Send out a box collider forward checking for where the car can teleport to safely
        if (Physics.CheckBox(newPosition, teleportSize / 2, transform.rotation, teleportLayerMask))
        {
            float tempDist = teleportDistance;

            newPosition = this.transform.position;

            // Continually check boxes along the teleport path to find the first available spot for player car to teleport to
            while (tempDist >= 0)
            {
                if (Physics.CheckBox(teleportRay.GetPoint(tempDist), teleportSize / 2, carCenter.rotation, teleportLayerMask))
                {
                    tempDist -= 2;
                }
                else
                {
                    newPosition = teleportRay.GetPoint(tempDist);
                    break;
                }
            }

        }
        
        // Return the found teleport position 
        return newPosition;
    }
    
    public void CastTeleportParticles()
    {
        teleportParticles.Play();
    }


    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(carCenter.position, teleportSize);
        

    }

}

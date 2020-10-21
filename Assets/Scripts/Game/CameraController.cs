using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    /*
     * currentTilt: Affects the forward/backward rotation of the camera (imagine a circle that the camera is attached to on the y axis above player)
     * currentAngle: MOVED TO "PlayerController.cs" script to simplify player and camera rotation. Camera is now child of player and 
     * cameraMoveSpeed: Affects the currentTilt and currentAngle camera rotation speed around player
     * cameraMaxTilt: Affects how high you can go rotating on the "y-axis tilt circle" (currently stops at right above player)
     * cameraMinTilt: Affects how low you can go rotating on the "y-axis tilt circle" (notice no limit for "x-axis circle" because you should be able to rotate 360degrees around player)
     * smoothTime: Affects how fast the SmoothDamp affects camera positions
     * currentVelocity: Used in smoothDamp 
     * */




    //Camera General Variables
    private bool controlsEnabled = true;
    [SerializeField]
    private float cameraMaxDistance = 15;
    [SerializeField]
    private float cameraHeight = 5;
    [SerializeField]
    private float cameraMoveSpeed = 125;


    //Camera Tilt Variables
    private float currentTilt = 0;
    [SerializeField]
    private float cameraMaxTilt = 60;
    [SerializeField]
    private float cameraMinTilt = -10;

    //Camera Angle Variables
    private float currentAngle = 0;
    

    //Camera Collision Variables
    private RaycastHit wallHit;
    private Ray playerRay;
    private Vector3 fromPosition, targetPosition;


    //Camera Movement Smoothing
    [SerializeField]
    private float smoothTime = 0.3f;
    [SerializeField]
    private float collisionSmoothTime = 0.05f;
    private Vector3 currentVelocity = Vector3.zero;

    //References
    [SerializeField]
    private Transform cameraTarget = null;
    [SerializeField]
    private Transform tilt = null;

    public GameObject centerOfCar = null;
    public Car playerCar = null;
    
    private Camera mainCamera;

    [SerializeField]
    private LayerMask mapLayerMask;
    
    [SerializeField]
    private GameObject text = null, gameOverPosition = null;
    

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.lockState = CursorLockMode.Locked;

        mainCamera = Camera.main;

        transform.position = centerOfCar.transform.position;
        transform.rotation = centerOfCar.transform.rotation;

        tilt.position = centerOfCar.transform.position;

        tilt.eulerAngles = new Vector3(currentTilt, transform.eulerAngles.y , transform.eulerAngles.z);
        

        cameraTarget.position = centerOfCar.transform.position;

        cameraTarget.position += (centerOfCar.transform.forward * -cameraMaxDistance) + Vector3.up * cameraHeight;
        

        mainCamera.transform.position = cameraTarget.position;

        //Only cast camera rays against layer 10 (the map layer)
        mapLayerMask = 1 << 10;

        fromPosition = mainCamera.transform.position;


    }

    

    void FixedUpdate()
    {

        if (controlsEnabled)
        {
            GetInputs();
        }
        else
        {  currentTilt = currentAngle = 0;  }

        CameraMovement();
    }


    private void CameraMovement()
    {
        // Only perform camera movement if car reference still exists
        if (centerOfCar != null)
        {
            // Make camera rig follow center of player car
            transform.position = centerOfCar.transform.position;

            // Rotating camera rig vertically 
            tilt.Rotate(currentTilt, 0, 0);

            // Rotating camera rig horizonatally  
            transform.Rotate(0, currentAngle, 0);

            // set the target position to the static cameraTarget position that is rotated around the car by a camera rig
            targetPosition = cameraTarget.position;

            // Check if the camera's view is obstructed by environment
            CameraCollision(centerOfCar.transform.position, targetPosition);
            

            // If there was a collision smooth the camera to the hit point faster than normal camera smoothing
            if (Physics.Linecast(fromPosition, targetPosition, mapLayerMask))
            {
                mainCamera.transform.position = Vector3.SmoothDamp(fromPosition, targetPosition, ref currentVelocity, collisionSmoothTime);
            }
            else
            {
                mainCamera.transform.position = Vector3.SmoothDamp(fromPosition, targetPosition, ref currentVelocity, smoothTime);
            }

            
            mainCamera.transform.LookAt(centerOfCar.transform);
       
            //save last viable position that we know was outside/above an object/wall
            fromPosition = mainCamera.transform.position;

            // Only display bomb timer text if the players car reference still exists
            if (playerCar != null)
            {
                if (playerCar.hasBomb)
                {   text.SetActive(true);    }
                else
                {   text.SetActive(false);    }

            }
            else
            {  text.SetActive(false);    }

        }
        else
        {
            text.SetActive(false);
        }
    }
    

    //Checks to see if the camera is about to enter an object/wall or lose line of sight of the player
    private void CameraCollision(Vector3 fromObject, Vector3 toTarget)
    {
        
        //declare a new raycast hit.
        wallHit = new RaycastHit();

        //declare new ray from player to the direction of camera (you get ray direction by taking (target vector - origin vector)
        playerRay = new Ray(fromObject, (toTarget - fromObject));

        //Spherecast from your player (fromObject) to your camera (toTarget) to find if the camera will collide with a surface.
        if (Physics.SphereCast(playerRay, 2f, out wallHit, cameraMaxDistance, mapLayerMask))
        {
            //Move the camera along the ray towards the player when there is a collision point along the ray
            float hitDistance = Mathf.Clamp(wallHit.distance, 0, cameraMaxDistance);
            
            targetPosition = playerRay.GetPoint(hitDistance);
            
        }
        
    }
    
    private void GetInputs()
    {
        currentTilt = (-1) * Input.GetAxis("Mouse Y") * cameraMoveSpeed * Time.deltaTime;

        currentAngle = Input.GetAxis("Mouse X") * cameraMoveSpeed * Time.deltaTime;


        //If tilt is at it's min or max values, then don't tilt in that direction anymore
        if ((tilt.localRotation.x >= cameraMaxTilt / 100) && (currentTilt > 0))
        {
            currentTilt = 0;
        }
        else if ((tilt.localRotation.x <= cameraMinTilt / 100) && (currentTilt < 0))
        {
            currentTilt = 0;
        }

    }

    public void SetInputsActive(bool enable)
    {
        controlsEnabled = enable;
    }


    public void SetGameOverPosition()
    {
        SetInputsActive(false);

        centerOfCar = null;
        playerCar = null;

        mainCamera.transform.position = cameraTarget.position;

        StartCoroutine(LerpToGameOverPosition());
    }

    // Move camera to game over position looking over the whole map
    IEnumerator LerpToGameOverPosition()
    {
        while (mainCamera.transform.position != gameOverPosition.transform.position && mainCamera.transform.rotation != gameOverPosition.transform.rotation)
        {
            mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, gameOverPosition.transform.rotation, 0.5f * Time.deltaTime);
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, gameOverPosition.transform.position, 0.5f * Time.deltaTime);

            yield return new WaitForFixedUpdate();
        }

        mainCamera.transform.rotation = gameOverPosition.transform.rotation;
    }
}

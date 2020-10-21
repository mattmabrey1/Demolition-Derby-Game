using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleCarDummy : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera = null;
    [SerializeField]
    private RectTransform carLocation = null;

    [SerializeField]
    private float rayDistance = 40f;

    private float dist = 1080;

    private Ray locRay = new Ray();
    

    // Update is called once per frame
    void LateUpdate()
    {
        locRay = mainCamera.ScreenPointToRay(carLocation.position);
        
        dist = rayDistance * (Screen.height / 1080f);
        
        dist = Mathf.Clamp(dist, 20, 40);
        

        this.transform.position = locRay.GetPoint(rayDistance);
    }
}

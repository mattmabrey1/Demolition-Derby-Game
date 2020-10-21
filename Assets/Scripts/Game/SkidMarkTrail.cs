using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkidMarkTrail : MonoBehaviour
{
    TrailRenderer TR;
    public WheelCollider WC;

    float rayDist = 0.2f;
    float slide = 0.25f;

    bool grounded;

    void Start()
    {
        TR = GetComponent<TrailRenderer>();
    }

    void Update()
    {
        grounded = false;

        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit rHit;

        if (Physics.Raycast(ray, out rHit, rayDist))
        {
            grounded = true;
        }

        WheelHit hit;
        WC.GetGroundHit(out hit);

       if (grounded && (hit.sidewaysSlip > slide))
        {
            TR.emitting = true;
        }
        else
        {
            TR.emitting = false;
        }
    }

   
}

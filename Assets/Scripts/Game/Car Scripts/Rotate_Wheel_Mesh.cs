using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate_Wheel_Mesh : MonoBehaviourPun
{
    public Transform flWheel, frWheel, blWheel, brWheel;
    public WheelCollider flWheelCollider, frWheelCollider, blWheelCollider, brWheelCollider;

    private Vector3 position;
    private Quaternion rotation;

    // Update is called once per frame
    void FixedUpdate()
    {
        RotateWheels();
    }

    private void RotateWheels()
    {
        /*
        flWheelCollider.GetWorldPose(out position, out rotation);

        flWheel.transform.position = position;
        flWheel.transform.rotation = rotation;

        frWheelCollider.GetWorldPose(out position, out rotation);

        frWheel.transform.position = position;
        frWheel.transform.rotation = rotation;

        blWheelCollider.GetWorldPose(out position, out rotation);

        blWheel.transform.position = position;
        blWheel.transform.rotation = rotation;

        brWheelCollider.GetWorldPose(out position, out rotation);

        brWheel.transform.position = position;
        brWheel.transform.rotation = rotation;
        */

        flWheel.localEulerAngles = new Vector3(flWheel.localEulerAngles.x, flWheelCollider.steerAngle - flWheel.localEulerAngles.z, flWheel.localEulerAngles.z);
        frWheel.localEulerAngles = new Vector3(frWheel.localEulerAngles.x, frWheelCollider.steerAngle - frWheel.localEulerAngles.z, frWheel.localEulerAngles.z);

        
        flWheel.Rotate(flWheelCollider.rpm / 60 * 360 * Time.deltaTime, 0, 0);
        frWheel.Rotate(frWheelCollider.rpm / 60 * 360 * Time.deltaTime, 0, 0);
        blWheel.Rotate(blWheelCollider.rpm / 60 * 360 * Time.deltaTime, 0, 0);
        brWheel.Rotate(brWheelCollider.rpm / 60 * 360 * Time.deltaTime, 0, 0);
        
    }
}

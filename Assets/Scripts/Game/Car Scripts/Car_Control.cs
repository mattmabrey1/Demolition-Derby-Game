using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_Control : MonoBehaviourPun
{

    public Control_Keybindings controls;

    private Vector2 inputs;

    private bool braking = false, specialAbility = false, controlsEnabled = false;

    public float MotorForce, SteerForce, brakeForce;
    public WheelCollider frontLeft, frontRight, backLeft, backRight;
    float timePressed = 0f;

    private Rigidbody carRigidbody;

    private Car_Special_Ability carSpecial;

    void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();

        carSpecial = GetComponent<Car_Special_Ability>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            if (controlsEnabled)
            {   GetInputs();    }
            else
            {   inputs = Vector2.zero;  }

            Drive();

        }

    }

    public void Drive()
    {
        
        if (carRigidbody.velocity.magnitude > 1)
        {
            SteerForce = ((-20f * Mathf.Log(carRigidbody.velocity.magnitude, 10)) + 65f);    // Temporary possible FIXME if-else statement that makes steering less powerful at higher speeds using logarithmic curve
        }
        else
        {   SteerForce = 65f;   }


        float forwardForce = inputs.y * MotorForce;
        float horizontalForce = inputs.x * SteerForce;

        frontLeft.motorTorque = forwardForce;
        frontRight.motorTorque = forwardForce;
        backLeft.motorTorque = forwardForce;
        backRight.motorTorque = forwardForce;
  

        frontLeft.steerAngle = Mathf.Lerp(frontLeft.steerAngle, horizontalForce, 5f * Time.deltaTime);
        frontRight.steerAngle = Mathf.Lerp(frontRight.steerAngle, horizontalForce, 5f * Time.deltaTime);


        if (braking)
        {
            frontLeft.brakeTorque = brakeForce;
            frontRight.brakeTorque = brakeForce;
            backLeft.brakeTorque = brakeForce;
            backRight.brakeTorque = brakeForce;


            //carRigidbody.drag = 0.5f;  // sets drag on car 1 to when braking so it slows down easier
        }
        else if (inputs.y == 0)
        {
            frontLeft.brakeTorque = brakeForce / 5;
            frontRight.brakeTorque = brakeForce / 5;
            backLeft.brakeTorque = brakeForce / 5;
            backRight.brakeTorque = brakeForce / 5;

            //carRigidbody.drag = 0.2f;
        }
        else
        {
            frontLeft.brakeTorque = 0;
            frontRight.brakeTorque = 0;
            backLeft.brakeTorque = 0;
            backRight.brakeTorque = 0;
            //carRigidbody.drag = 0f;

        }



        if (Input.GetKey(KeyCode.I) && carRigidbody.velocity.magnitude < 3)
        {
            FlipCar();
        }

        if (specialAbility)
        {
            carSpecial.CastAbility();
            specialAbility = false;
        }


    }


    public void FlipCar()
    {
        
            timePressed = timePressed + Time.deltaTime;

            if (timePressed > 3 )
            {
                Vector3 pos = new Vector3(1, 5, 0);
                Quaternion rot = new Quaternion();
                transform.position += pos;
                transform.rotation = rot;

                timePressed = 0;
            }

            


    }


    void GetInputs()
    {

        //Forwards movement 
        if (Input.GetKey(controls.forwards))
        {
            inputs.y = 1;
        }

        //Backwards movement
        if (Input.GetKey(controls.backwards))
        {
            if (Input.GetKey(controls.forwards))
            {   inputs.y = 0;   }
            else
            {   inputs.y = -1;  }
            
        }

        //FW nothing
        if (!Input.GetKey(controls.forwards) && !Input.GetKey(controls.backwards))
        {
            inputs.y = 0;
        }

        //Turn left movement
        if (Input.GetKey(controls.turnRight))
        {
            inputs.x = 1;

        }

        //Turn right movement
        if (Input.GetKey(controls.turnLeft))
        {
            if (Input.GetKey(controls.turnRight))
            {
                inputs.x = 0;
            }
            else
            {
                inputs.x = -1;
            }
        }

        //Strafe LR nothing
        if (!Input.GetKey(controls.turnLeft) && !Input.GetKey(controls.turnRight))
        {
            inputs.x = 0;
        }

        // Special Ability cast
        braking = Input.GetKey(controls.brake);

        // Special Ability cast
        specialAbility = Input.GetKey(controls.specialAbility);

    }


    public void SetInputsActive(bool enable)
    {
        controlsEnabled = enable;
    }


}

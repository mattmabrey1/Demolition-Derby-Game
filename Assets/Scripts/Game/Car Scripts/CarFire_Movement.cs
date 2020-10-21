using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarFire_Movement : MonoBehaviour
{

    [SerializeField]
    private ParticleSystem fireAlpha = null;
    [SerializeField]
    private ParticleSystem fireAdditive = null;


    private ParticleSystem.MainModule mainModule_Alpha;
    private ParticleSystem.MainModule mainModule_Additive;


    private float alphaMin = 1f, alphaMax = 2f, alphaMaxCurrent;

    private float additiveMin = 0.15f, additiveMax = 0.35f, additiveCurrent;

    
    private Rigidbody carRigidbody;

    private Car thisCar;

    void Start()
    {
        thisCar = this.GetComponent<Car>();
        carRigidbody = this.GetComponent<Rigidbody>();

        mainModule_Alpha = fireAlpha.main;
        mainModule_Additive = fireAdditive.main;


        alphaMaxCurrent = 2f;

        additiveCurrent = 0.35f;


        mainModule_Alpha.startLifetime = new ParticleSystem.MinMaxCurve(alphaMaxCurrent / 2, alphaMaxCurrent);


        mainModule_Additive.startLifetime = new ParticleSystem.MinMaxCurve(additiveCurrent);
    }

    // Update is called once per frame
    void Update()
    {
        if (thisCar.hasBomb)
        {
            
            if(carRigidbody.velocity.magnitude > 5)
            {
                // if rigidbody is going fast enough reduce flame distance to look more realisitc

                alphaMaxCurrent = Mathf.MoveTowards(alphaMaxCurrent, alphaMin, Time.deltaTime * 0.5f);

                additiveCurrent = Mathf.MoveTowards(additiveCurrent, additiveMin, Time.deltaTime * 0.5f);

                mainModule_Alpha.startLifetime = new ParticleSystem.MinMaxCurve(alphaMaxCurrent / 2, alphaMaxCurrent);


                mainModule_Additive.startLifetime = new ParticleSystem.MinMaxCurve(additiveCurrent);
            }
            else
            {
                // if rigidbody is not moving have flame go full distance

                alphaMaxCurrent = Mathf.MoveTowards(alphaMaxCurrent, alphaMax, Time.deltaTime * 0.5f);

                additiveCurrent = Mathf.MoveTowards(additiveCurrent, additiveMax, Time.deltaTime * 0.5f);

                mainModule_Alpha.startLifetime = new ParticleSystem.MinMaxCurve(alphaMaxCurrent / 2, alphaMaxCurrent);


                mainModule_Additive.startLifetime = new ParticleSystem.MinMaxCurve(additiveCurrent);

            }


        }
        

    }
}

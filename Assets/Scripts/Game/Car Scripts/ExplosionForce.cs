using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionForce : MonoBehaviour
{

    public float force = 100.0f;
    public float radius = 5.0f;
    public float upwardsModifier = 0.0f;
    public ForceMode forceMode;


    public ParticleSystem explosion;
    /*
    void Update()
    {
        if (Input.GetKey(KeyCode.B))
        {
            callExplosion();
            explosion.Play();
        }
    }
    */

    // Update is called once per frame
    public void callExplosion()
    {
        
        foreach (Collider col in Physics.OverlapSphere(transform.position, radius))
        {
            if (col.GetComponent<Rigidbody>())
            {
                col.GetComponent<Rigidbody>().AddExplosionForce(force, transform.position, radius, upwardsModifier, forceMode);
            }
            
           
        }
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterOfMass : MonoBehaviour
{
    [SerializeField]
    private Vector3 com = Vector3.zero;
    [SerializeField]
    private Rigidbody rb = null;
    

    void Start()
    {
        rb.centerOfMass = com;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarTransparency : MonoBehaviour
{

    [SerializeField]
    private MeshRenderer[] carPartRenderers = null;

    [SerializeField]
    private Renderer[] fireRenderers = new Renderer[4];
    private MaterialPropertyBlock _propBlock;


    private Color[] fireColors;

    private float ditherAmount = 2;
    
    private Car thisCar;

    private bool faded = false;

    void Start()
    {
        thisCar = this.GetComponent<Car>();

        fireColors = new Color[fireRenderers.Length];

        _propBlock = new MaterialPropertyBlock();

        for (int i = 0; i < fireRenderers.Length; i++)
        {
            fireColors[i] = fireRenderers[i].material.color;
        }
    }
    void Update()
    {
        // distance between car and camera
        float distance = Vector3.Distance(transform.position, Camera.main.transform.position);


        // when below certain distance start to dither
        if (distance <= 20)
        {
            ditherAmount = Mathf.Clamp((distance / 10) + 0.15f, 1.125f, 3f);

            foreach (MeshRenderer m in carPartRenderers)
            {
                m.material.SetFloat("_DitherAmount", ditherAmount);
            }


            if (thisCar.hasBomb)
            {
                float fadeAmount = Mathf.Clamp((distance / 10) - 0.4f, 0f, 1f);

                faded = true;

                for (int i = 0; i < fireRenderers.Length; i++)
                {
                    // Get the current value of the material properties in the renderer.
                    fireRenderers[i].GetPropertyBlock(_propBlock);

                    // Assign our new value.
                    _propBlock.SetColor("_BaseColor", new Color(fireColors[i].r, fireColors[i].g, fireColors[i].b, fadeAmount));

                    // Apply the edited values to the renderer.
                    fireRenderers[i].SetPropertyBlock(_propBlock);

                }
            }
            

        }
        else if(ditherAmount < 2 || faded)
        {
            foreach (MeshRenderer m in carPartRenderers)
            {
                m.material.SetFloat("_DitherAmount", 2f);
            }


            ditherAmount = 2;


            if (thisCar.hasBomb)
            {
                for (int i = 0; i < fireRenderers.Length; i++)
                {
                    // Get the current value of the material properties in the renderer.
                    fireRenderers[i].GetPropertyBlock(_propBlock);

                    // Assign our new value.
                    _propBlock.SetColor("_BaseColor", new Color(fireColors[i].r, fireColors[i].g, fireColors[i].b, 1));

                    // Apply the edited values to the renderer.
                    fireRenderers[i].SetPropertyBlock(_propBlock);

                }

                faded = false;
            }
            
            
        }
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAbility : MonoBehaviour
{
    [SerializeField]
    private float rocketJumpForce = 55555f;

    private Rigidbody carRigidbody;

    private Animator carAnimator;

    [SerializeField]
    private ParticleSystem[] jumpParticles = null;

    [SerializeField]
    private MeshRenderer[] wheelMeshes = new MeshRenderer[4];
    private MaterialPropertyBlock _propBlock;


    // Start is called before the first frame update
    void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
        carAnimator = GetComponent<Animator>();

        _propBlock = new MaterialPropertyBlock();
    }
    
    public void CastRocketJump()
    {
        // Add force to the player car sending them upwards like a rocket jump
        carRigidbody.AddForce(this.transform.up * rocketJumpForce, ForceMode.Impulse);

        // Adding the particle and color effects to the car jump
        RocketJumpEffect();
    }

    public void RocketJumpEffect()
    {

        carAnimator.SetTrigger("Jump");

        foreach(ParticleSystem p in jumpParticles)
        {
            p.Play();
        }

        foreach (MeshRenderer m in wheelMeshes)
        {
            // Get the current value of the material properties in the renderer.
            m.GetPropertyBlock(_propBlock);

            // Assign our new value.
            _propBlock.SetFloat("_JumpEnable", 1);

            // Apply the edited values to the renderer.
            m.SetPropertyBlock(_propBlock);
            
        }

    }

    public void EndRocketJumpEffect()
    {
        

        foreach (MeshRenderer m in wheelMeshes)
        {
            // Get the current value of the material properties in the renderer.
            m.GetPropertyBlock(_propBlock);

            // Assign our new value.
            _propBlock.SetFloat("_JumpEnable", 0);

            // Apply the edited values to the renderer.
            m.SetPropertyBlock(_propBlock);


        }
    }
}

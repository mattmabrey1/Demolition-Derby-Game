using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeLerp : MonoBehaviour
{
    /*
    private float startX;
    private float startZ;
    private float endX;
    private float endZ;
    private float timeStartedLerping;
    private bool started = false;
    [SerializeField] private float lerpTime = 0;


    // Start is called before the first frame update
    void Start()
    {
        startX = 0f;
        startZ = 0f;
        endX = Random.Range(-1.5f, 1.5f);
        endZ = Random.Range(-1.5f, 1.5f);
    }

    /*
    // Update is called once per frame
    void Update()
    {
        if(World.currentWorld.deadCars.Count > 0)
        {
            if (!started)
            {
                timeStartedLerping = Time.time;
                started = true;
            }
            

            foreach (ParticleSystem.NoiseModule noise in World.currentWorld.deadCars)
            {
                generateSmoke(noise);
            }
        }
        
    }
    
    // generate the smoke for the car after it has exploded and use Mathf.Lerp to slowly move the smoke simulating wind changes
    public void generateSmoke(ParticleSystem.NoiseModule noise)
    {
        
        if (!Mathf.Approximately(noise.strengthXMultiplier, endX))
        {
            noise.strengthXMultiplier = Lerp(startX, endX, timeStartedLerping, lerpTime);   // lerp the smoke noise along the X axis
        }

        if (!Mathf.Approximately(noise.strengthZMultiplier, endZ))
        {
            noise.strengthZMultiplier = Lerp(startZ, endZ, timeStartedLerping, lerpTime);   // lerp the smoke noise along the Z axis
        }

        if (Mathf.Approximately(noise.strengthXMultiplier, endX) && Mathf.Approximately(noise.strengthZMultiplier, endZ))     // when the smoke has reached its target endX and endZ position, give it new values to move to  
        {
            startX = endX;
            startZ = endZ;
            endX = Random.Range(-1.5f, 1.5f); ;
            endZ = Random.Range(-1.5f, 1.5f); ;
            timeStartedLerping = Time.time;

        }
        
    }

    public float Lerp(float start, float end, float timeStartedLerping, float lerpTime)
    {
        float timeSinceStarted = Time.time - timeStartedLerping;

        float percentageComplete = timeSinceStarted / lerpTime;

        float result = Mathf.Lerp(start, end, percentageComplete);

        return result;
    }
    */
}

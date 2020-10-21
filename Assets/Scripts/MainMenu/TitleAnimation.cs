using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleAnimation : MonoBehaviour
{
    [SerializeField]
    Image title1 = null;
    [SerializeField]
    Image title2 = null;
    
    [SerializeField]
    private float glowAmount = 1;

    
    // Update is called once per frame
    void Update()
    {
        
        glowAmount = Mathf.PingPong(Time.time, 1f);
        
        
        title1.color = new Color(1, 1, glowAmount, 1);
        title2.color  = new Color(1, 1, glowAmount, 1);
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bomb_Timer : MonoBehaviour
{
    

    public Text timerText;
  

    // Update is called once per frame
    void Update()
    {

        if(gameObject.activeSelf)
        {
            timerText.text = World.currentWorld.bombTimer.ToString("0");
            GetComponent<Text>().color = Color.Lerp(Color.yellow, Color.red, Mathf.PingPong(Time.time, 1));
            
        }
        
        
    }
}

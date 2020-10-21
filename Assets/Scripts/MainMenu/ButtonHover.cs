using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHover : MonoBehaviour
{
    [SerializeField]
    private Image buttonBackground = null;

    // Color of unhighlighted button vs highlighted
    [SerializeField]
    private Color backgroundNormal = new Color(), backgroundActive = new Color();

    [SerializeField]
    private TMP_Text buttonText = null;
    
    // Color of unhighlighted text vs highlighted
    [SerializeField]
    private Color textNormal = new Color(), textActive = new Color();
    

    void OnEnable()
    {
        SetNormal();
    }

    // Set button's color to normal unhighlighted color
    public void SetNormal()
    {
        buttonBackground.color = backgroundNormal;

        // if the button has text, set to unhighlighted color
        if (buttonText != null)
        { buttonText.color = textNormal; }
        
    }

    // Set button's color to highlighted color
    public void SetActive()
    {
        buttonBackground.color = backgroundActive;

        // if the button has text, set to highlighted color
        if (buttonText != null)
        { buttonText.color = textActive; }
    }

}

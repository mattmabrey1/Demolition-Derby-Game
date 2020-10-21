using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class Settings : MonoBehaviour
{
    
    [SerializeField]
    private Image[] windowSizeChecks = new Image[3];

    [SerializeField]
    private Image[] graphicsQualityChecks = new Image[3];


    [SerializeField]
    private TMP_Dropdown resolutionDropdown = null;

    private List<string> resolutionOptions = new List<string>();

    [SerializeField]
    private VisualEffect[] dustStormVFX = new VisualEffect[2];

    [SerializeField]
    private ParticleSystem[] fireBrazierVFX = new ParticleSystem[2];

    void Start()
    {

        resolutionDropdown.ClearOptions();

        Resolution[] resolutions = Screen.resolutions;
        
        foreach (Resolution r in resolutions)
        {
            if (Screen.width <= r.width && !resolutionOptions.Contains("" + Screen.width + "x" + Screen.height))
            {
                resolutionOptions.Add("" + Screen.width + "x" + Screen.height);
            }

            if (!resolutionOptions.Contains("" + r.width + "x" + r.height))
            {
                resolutionOptions.Add("" + r.width + "x" + r.height);
            }
        }

        if (!resolutionOptions.Contains("" + Screen.width + "x" + Screen.height))
        {
            resolutionOptions.Add("" + Screen.width + "x" + Screen.height);
        }
        

        resolutionDropdown.AddOptions(resolutionOptions);
        
        int i = 0;

        resolutionDropdown.value = i;

        foreach (string s in resolutionOptions)
        {
            if (s.Equals("" + Screen.width + "x" + Screen.height))
            {
                resolutionDropdown.value = i;
            }

            i++;
        }
    }


    // Start is called before the first frame update
    void OnEnable()
    {
        if(Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen)
        {
            windowSizeChecks[0].enabled = true;
            windowSizeChecks[1].enabled = windowSizeChecks[2].enabled = false;
        }
        else if (Screen.fullScreenMode == FullScreenMode.Windowed)
        {
            windowSizeChecks[1].enabled = true;
            windowSizeChecks[0].enabled = windowSizeChecks[2].enabled = false;
        }
        else
        {
            windowSizeChecks[2].enabled = true;
            windowSizeChecks[0].enabled = windowSizeChecks[1].enabled = false;
        }
        

        GraphicsQualityChange(QualitySettings.GetQualityLevel());


    }

    public void ScreenSizeChange(int choice)
    {
        // 0 full screen, 1 windowed, 2 borderless

        if (choice == 0)
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;

            windowSizeChecks[0].enabled = true;
            windowSizeChecks[1].enabled = windowSizeChecks[2].enabled = false;

        }
        else if (choice == 1)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;

            windowSizeChecks[1].enabled = true;
            windowSizeChecks[0].enabled = windowSizeChecks[2].enabled = false;
        }
        else if (choice == 2)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;

            windowSizeChecks[2].enabled = true;
            windowSizeChecks[0].enabled = windowSizeChecks[1].enabled = false;
            
        }
    }

    public void GraphicsQualityChange(int choice)
    {
        // 0 low, 1 medium, 2 high

        if (choice == 0)
        {
            QualitySettings.SetQualityLevel(0, true); 

            graphicsQualityChecks[0].enabled = true;
            graphicsQualityChecks[1].enabled = graphicsQualityChecks[2].enabled = false;

            foreach(VisualEffect vfx in dustStormVFX)
            {
                vfx.Stop();
                vfx.gameObject.SetActive(false);
            }

            foreach(ParticleSystem vfx in fireBrazierVFX)
            {
                vfx.Stop();
                vfx.gameObject.SetActive(false);
            }

        }
        else if (choice == 1)
        {
            QualitySettings.SetQualityLevel(1, true);

            graphicsQualityChecks[1].enabled = true;
            graphicsQualityChecks[0].enabled = graphicsQualityChecks[2].enabled = false;

            dustStormVFX[0].gameObject.SetActive(true);
            dustStormVFX[0].Play();
            dustStormVFX[1].Stop();
            dustStormVFX[1].gameObject.SetActive(false);

            foreach (ParticleSystem vfx in fireBrazierVFX)
            {
                vfx.gameObject.SetActive(true);
                vfx.Play();
            }

        }
        else if (choice == 2)
        {
            QualitySettings.SetQualityLevel(2, true);

            graphicsQualityChecks[2].enabled = true;
            graphicsQualityChecks[0].enabled = graphicsQualityChecks[1].enabled = false;

            foreach (VisualEffect vfx in dustStormVFX)
            {
                vfx.gameObject.SetActive(true);
                vfx.Play();
            }

            foreach (ParticleSystem vfx in fireBrazierVFX)
            {
                vfx.gameObject.SetActive(true);
                vfx.Play();
            }
        }
    }


    public void VolumeControl()
    {

    }

    public void HandleInputData(int val)
    {
        string newResolution = resolutionDropdown.options[val].text;
        
        string[] dimensions = newResolution.Split('x');

        int width = int.Parse(dimensions[0]);
        int height = int.Parse(dimensions[1]);
        
        Screen.SetResolution(width, height, Screen.fullScreenMode);
    }
    
}

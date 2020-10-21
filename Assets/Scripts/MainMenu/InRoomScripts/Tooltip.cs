using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    // External References
    [SerializeField]
    private RectTransform canvasTransform = null;
    
    [SerializeField]
    private TMP_Text tooltipDescription = null;
    [SerializeField]
    private GameObject tooltipImage = null;

    private bool tooltipActive = false;
    private RectTransform tooltipTransform;
    


    // Start is called before the first frame update
    void Start()
    {
        tooltipTransform = this.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void LateUpdate()
    {

        if (tooltipActive)
        {
            Vector2 localpoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTransform, Input.mousePosition, null, out localpoint);


            tooltipTransform.localPosition = localpoint;
        }


    }

    public void ShowTooltip (TMP_Text hoveredItem)
    {
        tooltipImage.SetActive(true);
        tooltipActive = true;
        
        if (hoveredItem.text.Equals("Afterburners"))
        {
            tooltipDescription.text = "Greatly accelerate your car.";
        }
        else if (hoveredItem.text.Equals("Explosive Suspension"))
        {
            tooltipDescription.text = "Instantly blast your car upwards.";
        }
        else if (hoveredItem.text.Equals("Wormhole Generator"))
        {
            tooltipDescription.text = "After a short delay, teleport forward.";
        }
        else if (hoveredItem.text.Equals("Heavy"))
        {
            tooltipDescription.text = "Generate charge from getting hit.";
        }
        else if (hoveredItem.text.Equals("Standard"))
        {
            tooltipDescription.text = "Slowly charge your ability over time.";
        }
        else if (hoveredItem.text.Equals("Special"))
        {
            tooltipDescription.text = "Hold 2 ability charges at once.";
        }
            
    }

    public void HideTooltip()
    {
        tooltipImage.SetActive(false);
        tooltipActive = false;
    }
}
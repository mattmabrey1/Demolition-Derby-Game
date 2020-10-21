using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityChargeMeter : MonoBehaviour
{
    private int currentCharge = 0, maxCharge = 100, chargeGoal = 0;

    private Car_Special_Ability specialAbility;

    private int chargesReady = 0;

    [SerializeField]
    private Image meterImage = null, backgroundImage = null;

    [SerializeField]
    private TMP_Text chargeText = null, abilityChargesText = null;

    [SerializeField]
    private GameObject chargingMeter = null, abilityCharged = null, rocketSprite = null, jumpSprite = null, teleportSprite = null;

    private bool chargingAllowed = true;

    public void InitizalizeMeter(Car_Special_Ability special)
    {
        specialAbility = special;
        
        int carChoice = (int)PhotonNetwork.LocalPlayer.CustomProperties["carChoice"];
        
        // Different cars provide different benefits to charge meter so, Car1 gets charge from being hit, Car2 gets charge over time, and Car3 gets 2 max charges
        if (carChoice > 3)
        {
            maxCharge = 200;
        }
        else if(carChoice == 2 || carChoice == 3)
        {
            StartCoroutine(ChargeOverTime());
        }

        // If abilities aren't enabled for this lobby turn this gameobject off, otherwise activate chosen ability image
        if (!specialAbility.IsAbilityEnabled())
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            int abilityChoice = specialAbility.GetAbilityChoice();

            if(abilityChoice == 0)
            {
                rocketSprite.SetActive(true);
            }
            else if (abilityChoice == 1)
            {
                jumpSprite.SetActive(true);
            }
            else if (abilityChoice == 2)
            {
                teleportSprite.SetActive(true);
            }
        }
    }
    
    public void AddCharge(float velocity)
    {

        if (chargingAllowed)
        {
            chargeGoal += (int)velocity;

            chargeGoal = Mathf.Clamp(chargeGoal, 0, maxCharge);


            StopCoroutine(SmoothCharge());
            StartCoroutine(SmoothCharge());
            StartCoroutine(ChargeWaitTime());
        }
        
        
    }

    // Reset all charge meter values after casting your ability
    public void ClearCharge()
    {
        currentCharge = Mathf.Clamp(currentCharge, 0, maxCharge);
        chargeGoal = Mathf.Clamp(chargeGoal, 0, maxCharge);
        chargesReady = Mathf.Clamp(chargesReady, 0, 2);

        chargesReady--;
        currentCharge -= 100;
        chargeGoal -= 100;

        currentCharge = Mathf.Clamp(currentCharge, 0, maxCharge);
        chargeGoal = Mathf.Clamp(chargeGoal, 0, maxCharge);
        chargesReady = Mathf.Clamp(chargesReady, 0, 2);

        chargeText.text = "" + currentCharge + "%";
        
        meterImage.fillAmount = (float)currentCharge / 100;
        backgroundImage.fillAmount = 1 - ((float)currentCharge / 100);

        if (currentCharge < 100)
        {
            chargingMeter.SetActive(true);
            abilityCharged.SetActive(false);

            abilityChargesText.enabled = false;
        }
        else
        {
            abilityChargesText.text = "" + 1;
        }
        
    }


    private IEnumerator SmoothCharge()
    {
        while (currentCharge != chargeGoal)
        {
            currentCharge = (int)Mathf.MoveTowards(currentCharge, chargeGoal, 150 * Time.deltaTime);

            meterImage.fillAmount = (float)currentCharge / 100f;
            backgroundImage.fillAmount = 1f - ((float)currentCharge / 100f);

            chargeText.text = "" + currentCharge + "%";

            if(currentCharge >= 100 && chargesReady == 0)
            {
                specialAbility.AddAbilityCharge();
                chargesReady++;

                chargingMeter.SetActive(false);
                abilityCharged.SetActive(true);
                
                if (maxCharge == 200)
                {
                    abilityChargesText.text = "" + 1;
                    abilityChargesText.enabled = true;
                }

                break;
            }
            else if (currentCharge >= 200 && chargesReady == 1)
            {
                abilityChargesText.text = "" + 2;
                specialAbility.AddAbilityCharge();
                chargesReady++;
                
                break;
            }

            yield return new WaitForFixedUpdate();
        }
        
        
    }

    IEnumerator ChargeWaitTime()
    {
        chargingAllowed = false;

        yield return new WaitForSeconds(1f);

        chargingAllowed = true;
    }

    IEnumerator ChargeOverTime()
    {
        yield return new WaitForSeconds(5);

        while(true)
        {

            chargeGoal += 1;
            StopCoroutine(SmoothCharge());
            StartCoroutine(SmoothCharge());

            yield return new WaitForSeconds(0.5f);
        }
        
    }
}

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_Special_Ability : MonoBehaviour
{
    private Rigidbody carRigidbody;

    private int abilityCharges = 0;

    private Abilities abilityChoice = Abilities.rocketBoost;

    // Rocket Jump/Thrust Variables

    [SerializeField]
    private ParticleSystem[] rocketThrusters = new ParticleSystem[2];

    // Rocket Duration
    [SerializeField]
    private float thrustTime = 0.5f;

    // How much force is applied every update
    [SerializeField]
    private float thrustForce = 2;

    private bool applyForce = false;

    //Jump ability script
    private JumpAbility jumpAbility;
    //Teleport script
    private TeleportAbility teleportAbility;


    private AbilityChargeMeter chargeMeter;

    private bool abilitiesEnabled = true;


    // Network Character to ensure abilities happen on all clients
    private NetworkCharacter networkCharacter;
    

    private enum Abilities
    {
        none = -1,
        rocketBoost = 0,
        rocketJump = 1,
        teleport = 2
    };

    // Start is called before the first frame update
    void Start()
    {
        
        if (this.gameObject.GetComponent<PhotonView>().IsMine)
        {
            chargeMeter = GameObject.FindGameObjectWithTag("ChargeMeter").GetComponent<AbilityChargeMeter>();
            chargeMeter.InitizalizeMeter(this);
        }

        jumpAbility = GetComponent<JumpAbility>();
        teleportAbility = GetComponent<TeleportAbility>();

        carRigidbody = this.GetComponent<Rigidbody>();

        networkCharacter = GetComponent<NetworkCharacter>();

    }

    public void AddAbilityCharge()
    {
        abilityCharges++;
    }

    public void SetAbility(int choice)
    {
        if (choice == -1)
        {
            abilityChoice = Abilities.none;
            abilitiesEnabled = false;
        }
        else if (choice == 0)
        {
            abilityChoice = Abilities.rocketBoost;
        }
        else if (choice == 1)
        {
            abilityChoice = Abilities.rocketJump;
        }
        else if (choice == 2)
        {
            abilityChoice = Abilities.teleport;
        }
        
    }


    public void CastAbility()
    {
        if (abilityCharges > 0 && abilitiesEnabled)
        {
            switch (abilityChoice)
            {
                case Abilities.rocketBoost:
                    CastRocketBoost();
                    break;
                case Abilities.rocketJump:
                    networkCharacter.SetJumpingEffect();
                    jumpAbility.CastRocketJump();
                    break;
                case Abilities.teleport:
                    teleportAbility.CastTeleport();
                    break;
                default:
                    Debug.Log("Ability cast, but no ability choice specified");
                    break;
            }

            abilityCharges--;
            abilityCharges = Mathf.Clamp(abilityCharges, 0, 2);

            chargeMeter.ClearCharge();

            StartCoroutine(AbilityWaitTime());
        }

    }

    private void CastRocketBoost()
    {
        networkCharacter.SetRocketThrusting(true);

        rocketThrusters[0].Play();
        rocketThrusters[1].Play();


        StartCoroutine(RocketThrustTime(thrustTime));
        StartCoroutine(RocketBoost());
    }

    public void EnableRockets(bool enable)
    {
        if (enable)
        {
            rocketThrusters[0].Play();
            rocketThrusters[1].Play();
        }
        else
        {
            rocketThrusters[0].Stop();
            rocketThrusters[1].Stop();
        }
    }

    public void EnableJump()
    {
        jumpAbility.RocketJumpEffect();
    }

    public void EnableTeleportParticles()
    {
        teleportAbility.CastTeleportParticles();
    }

    public bool IsAbilityEnabled()
    {
        return abilitiesEnabled;
    }

    public int GetAbilityChoice()
    {
        return (int)abilityChoice;
    }
    

    IEnumerator RocketBoost()
    {
        while (applyForce)
        {
            // Increasing cars velocity every update to simulate rocket thrust
            carRigidbody.velocity += this.transform.forward * thrustForce;

            yield return new WaitForFixedUpdate();
        }

    }

    IEnumerator RocketThrustTime(float time)
    {

        applyForce = true;

        yield return new WaitForSeconds(time);

        networkCharacter.SetRocketThrusting(false);

        applyForce = false;

        rocketThrusters[0].Stop();
        rocketThrusters[1].Stop();
    }
    
    IEnumerator AbilityWaitTime()
    {
        abilitiesEnabled = false;

        yield return new WaitForSeconds(1.5f);

        abilitiesEnabled = true;
    }

}

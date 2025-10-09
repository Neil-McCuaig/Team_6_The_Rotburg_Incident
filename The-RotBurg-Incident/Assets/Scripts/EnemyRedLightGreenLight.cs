using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using UnityEngine;


public class EnemyRedLightGreenLight : MonoBehaviour
{
    //Note for future: using UnityEngine.Rendering.Universal; HAS TO BE AT THE TOP! That's how it accesses the URP light script. Otherwise,
    //It things your trying to get a normal light. It has to be Light2D and not Light.

    public float maxPhaseTimer = 0f;
    public float currentPhaseTimer = 0f;
    public DomainZoneLogic DomainZone;
    private PlayerController playerController;

    //Colors
    public GameObject AttachedLight;
    public Light2D lt;
    Color Sleeping = Color.black;
    Color Go = Color.green;
    Color Hide = Color.yellow;
    Color Attack = Color.red;

    public bool Active = false;

    // Start is called before the first frame update
    void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();
        DomainZone = FindAnyObjectByType<DomainZoneLogic>();
        lt = AttachedLight.GetComponent<Light2D>();
        //lt = FindAnyObjectByType<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        if (DomainZone.playerInDomain == true && Active == false)
        {
            SleepPhase();
            Active = true;
            Debug.Log("Is it looping in Update");
        }
        if (DomainZone.playerInDomain == false && Active == true)
        {
            Active = false;
        }
    }

    private void SleepPhase()
    {
        maxPhaseTimer = 5f;
        currentPhaseTimer = maxPhaseTimer;
        Debug.Log("Is it looping in SleepPhase");

        //Maybe change some of these to a "While" or otherwise add some sort of a cutoff for if the player just leaves in the middle of
        //a phase?
        if (DomainZone.playerInDomain == true)
        {
            //MaxPhaseTimer should be randomized here, and every time it gets called for a new phase, at random, in a small range.
            //Leave the player guessing a bit. Just doing it as 5 so I can process with other code for the moment.
            currentPhaseTimer -= Time.deltaTime;
            Debug.Log("Is it looping in the If");

            lt.color = Sleeping;

            if (currentPhaseTimer < 0f && playerController.inLocker == false)
            {
                PhaseGreen();
            }
        }
    }

    private void PhaseGreen() 
    {
        maxPhaseTimer = 5f;
        currentPhaseTimer = maxPhaseTimer;
        currentPhaseTimer -= Time.deltaTime;
        lt.color = Go;

        if (currentPhaseTimer < 0f && playerController.inLocker == false)
        {
            PhaseYellow();
        }
    }

    private void PhaseYellow()
    {
        maxPhaseTimer = 5f;
        currentPhaseTimer = maxPhaseTimer;
        currentPhaseTimer -= Time.deltaTime;
        lt.color = Hide;

        if (currentPhaseTimer < 0f && playerController.inLocker == false)
        {
            PhaseRed();
        }
        //Originally put this here. Maybe it would work better is phase red?
        //else if (currentPhaseTimer < 0f && playerController.inLocker == true)
        //{
            //SleepPhase();
        //}
    }

    private void PhaseRed()
    {
        maxPhaseTimer = 5f;
        currentPhaseTimer = maxPhaseTimer;
        currentPhaseTimer -= Time.deltaTime;
        lt.color = Attack;

        if (currentPhaseTimer < 0f && playerController.inLocker == true)
        {
            SleepPhase();
        }
        else if (currentPhaseTimer < 0f && playerController.inLocker == false)
        {
            AttackPhase();
        }
    }

    private void AttackPhase()
    {

    }
}

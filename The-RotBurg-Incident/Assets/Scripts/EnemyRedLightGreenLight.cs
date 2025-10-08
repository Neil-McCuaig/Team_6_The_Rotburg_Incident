using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRedLightGreenLight : MonoBehaviour
{
    public float maxPhaseTimer = 0f;
    public float currentPhaseTimer = 0f;
    public DomainZoneLogic DomainZone;
    private PlayerController playerController;
    public Light lightColor;

    // Start is called before the first frame update
    void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();
        DomainZone = FindAnyObjectByType<DomainZoneLogic>();
        lightColor = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SleepPhase()
    {

        //Maybe change some of these to a "While" or otherwise add some sort of a cutoff for if the player just leaves in the middle of
        //a phase?
        if (DomainZone.playerInDomain == true)
        {
            //MaxPhaseTimer should be randomized here, and every time it gets called for a new phase, at random, in a small range.
            //Leave the player guessing a bit. Just doing it as 5 so I can process with other code for the moment.
            maxPhaseTimer = 5f;
            currentPhaseTimer = maxPhaseTimer;
            currentPhaseTimer -= Time.deltaTime;

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

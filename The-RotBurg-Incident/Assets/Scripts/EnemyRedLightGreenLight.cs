using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using UnityEngine;


public class EnemyRedLightGreenLight : MonoBehaviour
{
    //Note for future: using UnityEngine.Rendering.Universal; HAS TO BE AT THE TOP! That's how it accesses the URP light script. Otherwise,
    //It things your trying to get a normal light. It has to be Light2D and not Light.

    public float maxPhaseTimer;
    public float currentPhaseTimer;
    public DomainZoneLogic DomainZone;
    private PlayerController playerController;

    //Colors
    public GameObject AttachedLight;
    public Light2D lt;
    Color Sleeping = Color.black;
    Color Go = Color.green;
    Color Hide = Color.yellow;
    Color Attack = Color.red;

    public int currentPhaseCount;

    public bool Active = false;

    // Start is called before the first frame update
    void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();
        DomainZone = FindAnyObjectByType<DomainZoneLogic>();
        lt = AttachedLight.GetComponent<Light2D>();
        //lt = FindAnyObjectByType<Light>();

        maxPhaseTimer = 5f;
        currentPhaseTimer = maxPhaseTimer;

        currentPhaseCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (DomainZone.playerInDomain == true && Active == false && currentPhaseCount == 0)
        {
            SleepPhase();
            Active = true;
            //Debug.Log("Is it looping in Update");
        }
        if (DomainZone.playerInDomain == true && Active == true && currentPhaseCount == 1 && currentPhaseTimer <= 0f) 
        {
            PhaseGreen();
        }
        if (DomainZone.playerInDomain == true && Active == true && currentPhaseCount == 2 && currentPhaseTimer <= 0f)
        {
            PhaseYellow();
        }
        if (DomainZone.playerInDomain == true && Active == true && currentPhaseCount == 3 && currentPhaseTimer <= 0f)
        {
            PhaseRed();
        }
        //Do we want it to count down again here, or just have a check of "Oh. Player is in locker. Back to bed."
        if (DomainZone.playerInDomain == true && Active == true && currentPhaseCount == 4 && currentPhaseTimer <= 0f && playerController.inLocker == true)
        {
            SleepPhase();
        }
        if (DomainZone.playerInDomain == true && Active == true && currentPhaseCount == 4 && currentPhaseTimer <= 0f && playerController.inLocker == false)
        {
            AttackPhase();
        }
        if (DomainZone.playerInDomain == false && Active == true)
        {
            Active = false;
            currentPhaseCount = 0;
            maxPhaseTimer = 5f;
            currentPhaseTimer = maxPhaseTimer;
        }

        //This is the working version. Nothing else works. Probably just replace it with an IEnumerator like Parker suggested, namely:
        //StartCoroutine(StartTimer()); IEnumerator StartTimer() { startTimer = true; yield return new WaitForSeconds(3f); startTimer = false;}
        if (DomainZone.playerInDomain == true && currentPhaseTimer > 0f)
        {
           currentPhaseTimer -= Time.deltaTime;
        }
    }

    private void SleepPhase()
    {
        //MaxPhaseTimer should be randomized, and every time it gets called for a new phase, at random, in a small range.
        //Leave the player guessing a bit. Just doing it as 5 so I can process with other code for the moment.
        maxPhaseTimer = 5f;
        currentPhaseTimer = maxPhaseTimer;
        lt.color = Sleeping;
        currentPhaseCount = 1;

        //Need to figure out a cutoff for if the player just leaves in the middle of a phase?
        //if (DomainZone.playerInDomain == true)
        //{
            //Debug.Log("Is it looping in the If");



            //These need to be ported up into Update, they don't get checked every frame and that is the problem.
            //if (currentPhaseTimer <= 0f && playerController.inLocker == false)
            //{
                //PhaseGreen();
            //}
        //}
    }

    private void PhaseGreen() 
    {
        maxPhaseTimer = 5f;
        currentPhaseTimer = maxPhaseTimer;
        lt.color = Go;
        currentPhaseCount = 2;

        //if (currentPhaseTimer < 0f && playerController.inLocker == false)
        //{
            //PhaseYellow();
        //}
    }

    private void PhaseYellow()
    {
        maxPhaseTimer = 5f;
        currentPhaseTimer = maxPhaseTimer;
        lt.color = Hide;
        currentPhaseCount = 3;

        //if (currentPhaseTimer < 0f && playerController.inLocker == false)
        //{
        //PhaseRed();
        //}


        //Originally put this here. Maybe it would work better in phase red? It's for if the player does hide in the locker. Do we want it
        //To reach Phase Red and you have to keep hiding through it, or do we want it to just check "it's Phase Yellow, their Hidden, back
        //to sleep"
        //else if (currentPhaseTimer < 0f && playerController.inLocker == true)
        //{
        //SleepPhase();
        //}
    }

    private void PhaseRed()
    {
        maxPhaseTimer = 5f;
        currentPhaseTimer = maxPhaseTimer;
        lt.color = Attack;
        currentPhaseCount = 4;

        //if (currentPhaseTimer < 0f && playerController.inLocker == true)
        //{
        //SleepPhase();
        //}
        //else if (currentPhaseTimer < 0f && playerController.inLocker == false)
        //{
        //AttackPhase();
        //}
    }

    private void AttackPhase()
    {
        //Insert Parker's attack code here. Also will need some way to reset the whole thing.
        Debug.Log("Attacking now!");
    }
}

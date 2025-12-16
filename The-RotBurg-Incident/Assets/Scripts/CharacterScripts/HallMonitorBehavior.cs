using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using UnityEngine;

public class HallMonitorBehavior : MonoBehaviour, EnemyStunable
{
    [Header("State Management")]
    Color Sleeping = Color.black;
    Color Go = Color.green;
    Color Hide = Color.yellow;
    Color Attack = Color.red;
    public float greenLightDuration;
    public float yellowLightDuration;
    bool greenLightBegun;
    bool yellowLightBegun;
    bool playerCaught;

    [Header("Orbit Settings")]
    public float orbitRadius;
    public float orbitSpeed;
    private float orbitTimer;
    private bool isOrbiting = true;

    [Header("Timing")]
    public float minOrbitTime;
    public float maxOrbitTime;
    public float preDashDelay;

    [Header("Dash Settings")]
    public float dashSpeed;
    public float damageAmount;
    private bool isPreparingToDash = false;
    private bool isDashing = false;

    [Header("Stun Settings")]
    public float stunDuration;
    private bool isStunned = false;

    private Transform player;
    Animator anim;
    public DomainZoneLogic DomainZone;
    public GameObject AttachedLight;
    public Light2D lt;
    PlayerController playerController;
    private bool isLockedToPlayer = true;
    private float currentAngle;

    private enum State { GreenLight, YellowLight, RedLight }
    private State currentState = State.GreenLight;

    public Transform returnPoint;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
        anim = GetComponent<Animator>();
        playerController = FindAnyObjectByType<PlayerController>();

        orbitTimer = Random.Range(minOrbitTime, maxOrbitTime);

        Vector3 offset = transform.position - player.position;
        currentAngle = Mathf.Atan2(offset.y, offset.x);

        DomainZone = FindAnyObjectByType<DomainZoneLogic>();
        lt = AttachedLight.GetComponent<Light2D>();
    }

    private void Update()
    {
        if (DomainZone.playerInDomain == false)
        {
            //Causes the Hall Monitor to return to it's returnPoint. This happens instantly. I want it to fly there. Also, needs to have
            //An offset where it will stop trying to get to the return point so it does not lag the game.
            Vector3 direction = returnPoint.position - transform.position;
            transform.position += direction * dashSpeed * Time.deltaTime;
            currentState = State.GreenLight;
            lt.color = Sleeping;
            return;
        }
        switch (currentState)
        {
            case State.GreenLight:
            {
                foreach (var locker in LockerInteraction.allLockers)
                {
                    locker.UnSealLockers();
                }
                anim.SetBool("IsClosed", true);

                lt.color = Go;
                if (!greenLightBegun)
                {
                    greenLightBegun = true;
                    StartCoroutine(GreenLightDuration());
                }
                Vector3 direction = player.position - transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);
                Vector3 offset = new Vector3(0f, orbitRadius, 0f);
                transform.position = player.transform.position + offset;

                break;
            }
            case State.YellowLight:
            {
                anim.SetBool("IsClosed", true);
                lt.color = Hide;
                if (!yellowLightBegun)
                {
                    yellowLightBegun = true;
                    StartCoroutine(YellowLightDuration());
                }
                Vector3 direction = player.position - transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);
                Vector3 offset = new Vector3(0f, orbitRadius, 0f);
                transform.position = player.transform.position + offset;

                break;
            }
            case State.RedLight:
            {
                foreach (var locker in LockerInteraction.allLockers)
                {
                    locker.SealAllLockers();
                }
                lt.color = Attack;

                if (isOrbiting)
                {
                    anim.SetBool("IsClosed", true);
                    OrbitAroundPlayer();
                    orbitTimer -= Time.deltaTime;
                    if(playerController.inLocker == false)
                    {
                        playerCaught = true;
                    }
                    if (orbitTimer <= 0f)
                    {
                        if (playerCaught)
                        {
                            StartCoroutine(PrepareToDash());
                            playerCaught = false;
                        }
                        else
                        {
                            playerCaught = false;
                            ResetOrbit();
                            currentState = State.GreenLight;
                        }
                    }
                }
                else if (isPreparingToDash)
                {
                    anim.SetBool("IsClosed", false);
                    Vector3 offset = new Vector3(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle), 0f) * orbitRadius;
                    transform.position = player.position + offset;
                }
                else if (isDashing)
                {
                    DashTowardPlayerWhileLocked();
                }
                               
                Vector3 direction = player.position - transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);

                break;
            }
        }
    }

    void OrbitAroundPlayer()
    {
        if (!isLockedToPlayer)
        {
            return;
        }
        currentAngle += orbitSpeed * Mathf.Deg2Rad * Time.deltaTime;
        Vector3 offset = new Vector3(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle), 0f) * orbitRadius;
        transform.position = player.position + offset;
    }

    IEnumerator PrepareToDash()
    {
        if (isPreparingToDash)
        {
            yield break;
        }
        isPreparingToDash = true;
        isOrbiting = false;

        yield return new WaitForSeconds(preDashDelay);

        if (!isStunned)
        {
            isDashing = true;
        }
        isPreparingToDash = false;
    }

    void DashTowardPlayerWhileLocked()
    {
        if (!isLockedToPlayer)
        {
            return;
        }

        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * dashSpeed * Time.deltaTime;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance < 0.5f)
        {
            FindAnyObjectByType<PlayerHealth>().TakeDamage(damageAmount);
            isLockedToPlayer = false;
            isDashing = false;
            ResetOrbit();
            currentState = State.GreenLight;
        }
    }

    public void Stun()
    {
        if (isStunned || isDashing || isOrbiting)
        {
            return;
        }
        anim.SetBool("IsStunned", true);
        StopAllCoroutines();
        isOrbiting = false;
        isPreparingToDash = false;
        isDashing = false;
        isStunned = true;
        Invoke(nameof(RecoverFromStun), stunDuration);
    }

    void RecoverFromStun()
    {
        isStunned = false;
        anim.SetBool("IsStunned", false);
        ResetOrbit();
        currentState = State.GreenLight;
    }

    void ResetOrbit()
    {
        orbitTimer = Random.Range(minOrbitTime, maxOrbitTime);
        isOrbiting = true;
        isDashing = false;
        isPreparingToDash = false;
        isLockedToPlayer = true;
    }

    IEnumerator GreenLightDuration()
    {
        yield return new WaitForSeconds(greenLightDuration);
        currentState = State.YellowLight;
        greenLightBegun = false;
    }
    IEnumerator YellowLightDuration()
    {
        yield return new WaitForSeconds(yellowLightDuration);
        currentState = State.RedLight;
        yellowLightBegun = false;
        if (playerController.inLocker)
        {
            playerCaught = false;
        }
        else
        {
            playerCaught = true;
        }
    }

}

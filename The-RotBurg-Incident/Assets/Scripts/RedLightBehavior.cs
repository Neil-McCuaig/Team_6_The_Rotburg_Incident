using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedLightBehavior : MonoBehaviour, EnemyStunable
{
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
    private bool isPreparingToDash = false;
    private bool isDashing = false;

    [Header("Stun Settings")]
    public float stunDuration;
    private bool isStunned = false;

    private Transform player;
    Animator anim;
    private bool isLockedToPlayer = true;
    private float currentAngle;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
        anim = GetComponent<Animator>();

        orbitTimer = Random.Range(minOrbitTime, maxOrbitTime);

        Vector3 offset = transform.position - player.position;
        currentAngle = Mathf.Atan2(offset.y, offset.x);
    }

    private void Update()
    {
        if (isStunned)
        {
            return;
        }
        if (isOrbiting)
        {
            anim.SetBool("IsClosed", true);
            OrbitAroundPlayer();
            orbitTimer -= Time.deltaTime;

            if (orbitTimer <= 0f)
            {
                StartCoroutine(PrepareToDash());
            }
        }
        else if (isPreparingToDash)
        {
            anim.SetBool("IsClosed", false);
            LockToPlayerOrbitSpot();
        }
        else if (isDashing)
        {
            DashTowardPlayerWhileLocked();
        }

        Vector3 direction = player.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);
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
            StartDash();
        }

        isPreparingToDash = false;
    }

    void StartDash()
    {
        isDashing = true;
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
            UnlockFromPlayer();
            ResetOrbit();
        }
    }

    void LockToPlayerOrbitSpot()
    {
        Vector3 offset = new Vector3(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle), 0f) * orbitRadius;
        transform.position = player.position + offset;
    }

    void UnlockFromPlayer()
    {
        isLockedToPlayer = false; 
        isDashing = false;
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
    }

    void ResetOrbit()
    {
        orbitTimer = Random.Range(minOrbitTime, maxOrbitTime);
        isOrbiting = true;
        isDashing = false;
        isPreparingToDash = false;
        isLockedToPlayer = true;
    }
}

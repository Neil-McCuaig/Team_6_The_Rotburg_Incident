using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;

public class FlyingEnemyController : MonoBehaviour, EnemyStunable, EnemyKnockbackable
{
    [Header("Enemy Movement Settings")]
    public float moveSpeed = 3f;
    public float idleFloatSpeed = 0.5f;
    public float idleFloatHeight = 0.5f;
    private Vector2 idleStartPos;
    private float floatTimer = 0f;
    private Vector2 currentDirection;

    [Header("Dash Settings")]
    public float dashSpeed = 3f;
    public float dashDetectionDistance;
    public float windupTime;
    public float maxDashTime;
    public int dashDir;
    public bool isDashing;
    public bool isCharging;
    public bool detected;

    [Header("Player Detection")]
    [Range(0, 360)]
    public float viewDistance = 10f;
    public float viewAngle = 45f;
    public LayerMask playerMask;

    [Header("Stun Settings")]
    public bool isStunned;
    public float stunTimer;
    private float stunCountdown;
    public bool isDead = false;

    [Header("WallBounce Settings")]
    public float bounceForce = 2f;
    private bool wallBounce;
    public LayerMask groundLayer;
    private float detectionDistance = 3f;

    [Header("Knockback Settings")]
    public float knockbackTime = 0.15f;
    public float hitRecoverTime = 0.5f;
    private bool isKnockedBack = false;

    private enum State { HoveringIdle, ChasePlayer, StunState, EnemyDeath }
    private State currentState = State.HoveringIdle;
    private State previousState;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Transform player;
    private EnemyHealth health;
    private Animator anim;
    private Collider2D enemyCollider;
    private Collider2D playerCollider;

    private PlayerController playerController;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        health = GetComponent<EnemyHealth>();
        enemyCollider = GetComponent<Collider2D>();
        playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();

        idleStartPos = transform.position;
        stunCountdown = stunTimer;

        playerController = FindAnyObjectByType<PlayerController>();
    }

    void Update()
    {
        // Detect state change
        if (previousState != currentState)
        {
            OnStateEnter(currentState);
            previousState = currentState;
        }

        switch (currentState)
        {
            case State.HoveringIdle:
            {
                Hover();
                if(CanSeePlayer())
                {
                    currentState = State.ChasePlayer;
                }

                break;
            }

            case State.ChasePlayer:
            {
                if (!CanSeePlayer())
                {
                    currentState = State.HoveringIdle;
                }
                if (isStunned)
                {
                    isCharging = false;
                    isDashing = false;
                    rb.velocity = Vector2.zero;
                    StopAllCoroutines();
                    currentState = State.StunState;
                    return;
                }
                if (!isDashing || !isCharging)
                {
                    Vector2 targetDirection = ((Vector2)(player.position - transform.position)).normalized;
                    currentDirection = targetDirection;
                }

                RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, dashDetectionDistance, playerMask);
                RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, dashDetectionDistance, playerMask);
                if (hitLeft.collider != null && hitLeft.collider.CompareTag("Player") && !isDashing)
                {
                    detected = true;
                    dashDir = -1;
                }
                else if (hitRight.collider != null && hitRight.collider.CompareTag("Player") && !isDashing)
                {
                    detected = true;
                    dashDir = 1;
                }
                else
                {
                    detected = false;
                }
                if (detected && !isDashing && !isCharging)
                {
                    StartCoroutine(DashRoutine());
                }

                break;
            }

            case State.StunState:
            {
                anim.SetBool("isStunned", true);
                if (!isKnockedBack)
                {
                    rb.velocity = Vector2.zero;
                }
                else
                {
                    stunCountdown = stunTimer;
                    isStunned = false;
                    anim.SetBool("isStunned", false);
                }
                stunCountdown -= Time.deltaTime;
                if (stunCountdown <= 0f)
                {
                    stunCountdown = stunTimer;
                    isStunned = false;
                    anim.SetBool("isStunned", false);
                    currentState = State.ChasePlayer;
                }
                break;
            }
            case State.EnemyDeath:
            {
                isDead = true;
                Physics2D.IgnoreCollision(enemyCollider, playerCollider, true);
                rb.gravityScale = 5.0f;

                break;
            }
        }
        if (health.currentHealth > 0)
        {
            Vector3 scale = transform.localScale;
            if (currentDirection.x > 0 )
            {
                scale.x = Mathf.Abs(scale.x);
            }
            else if (currentDirection.x < 0)
            {
                scale.x = -Mathf.Abs(scale.x);
            }
            if (isDashing == false)
            {
                transform.localScale = scale;
            }
            DetectWallsAndGround();
        }
        else
        {
            currentState = State.EnemyDeath;
        }
    }
    void OnStateEnter(State newState)
    {
        if (newState == State.HoveringIdle)
        {
            idleStartPos = transform.position; 
            floatTimer = 0f;                   
        }
    }
    private IEnumerator DashRoutine()
    {
        isCharging = true;
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(windupTime);

        isDashing = true;
        isCharging = false;

        float dashTimer = 0f;

        while (dashTimer < maxDashTime)
        {
            dashTimer += Time.deltaTime;

            rb.velocity = new Vector2(dashDir * dashSpeed, 0);

            RaycastHit2D wallHit = Physics2D.Raycast(transform.position, new Vector2(dashDir, 0), 3.5f, groundLayer);
            if (wallHit.collider != null)
            {
                isDashing = false;
                isCharging = false;
                rb.velocity = Vector2.zero;
                StopAllCoroutines();
                BounceFromWall(hit: wallHit);
                currentState = State.HoveringIdle;
                yield break;
            }
            if (isStunned)
            {
                rb.velocity = Vector2.zero;
                isDashing = false;
                isCharging = false;
                currentState = State.StunState;
                yield break;
            }
            if (isDead)
            {
                rb.velocity = Vector2.zero;
                isDashing = false;
                isCharging = false;
                currentState = State.EnemyDeath;
                yield break;
            }
            yield return null;
        }

        rb.velocity = Vector2.zero;
        isDashing = false;

        currentState = State.HoveringIdle;
    }

    private void FixedUpdate()
    {
        if (!wallBounce && !isStunned && !isDead && !isKnockedBack && !isDashing && !isCharging)
        {
            rb.velocity = currentDirection * moveSpeed;
        }
    }

    void Hover()
    {
        floatTimer += Time.deltaTime;
        float newY = idleStartPos.y + Mathf.Sin(floatTimer * idleFloatSpeed) * idleFloatHeight;
        float newX = idleStartPos.x + Mathf.Sin(floatTimer * idleFloatSpeed) * 0.2f;
        Vector2 targetPos = new Vector2(newX, newY);
        currentDirection = (targetPos - (Vector2)transform.position).normalized;
    }

    bool CanSeePlayer()
    {
        Vector2 directionToPlayer = (player.position - transform.position).normalized;

        if (Vector2.Distance(transform.position, player.position) > viewDistance)
        {
            return false;
        }
        if (currentDirection.x > 0)
        {
            float angleToPlayer = Vector2.Angle(transform.right, directionToPlayer);
            if (angleToPlayer > viewAngle / 2)
            {
                return false;
            }
        }
        else if (currentDirection.x < 0)
        {
            float angleToPlayer = Vector2.Angle(-transform.right, directionToPlayer);
            if (angleToPlayer > viewAngle / 2)
            {
                return false;
            }
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, viewDistance, playerMask);
        if (hit.collider != null && hit.collider.transform == player)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void DetectWallsAndGround()
    {
        Vector2[] directions = { Vector2.right, Vector2.left, Vector2.up, Vector2.down };

        foreach (Vector2 dir in directions)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, detectionDistance, groundLayer);
            if (hit.collider != null)
            {
                wallBounce = true;

                Vector2 bounceDirection = Vector2.Reflect(currentDirection, hit.normal).normalized;
                Vector2 toPlayer = ((Vector2)(player.position - transform.position)).normalized;
                float blendTowardPlayer = 0.3f; 
                Vector2 offsetDirection = Vector2.Lerp(bounceDirection, toPlayer, blendTowardPlayer).normalized;
                currentDirection = offsetDirection;
                rb.velocity = currentDirection * bounceForce;
                StartCoroutine(BounceCooldown());

                break;
            }
        }
    }
    void BounceFromPlayer()
    {
        wallBounce = true;
        Vector2 bounceDirection = Vector2.Reflect(currentDirection, player.position).normalized;
        Vector2 toPlayer = ((Vector2)(player.position - transform.position)).normalized;
        float blendTowardPlayer = 0.3f;
        Vector2 offsetDirection = Vector2.Lerp(bounceDirection, toPlayer, blendTowardPlayer).normalized;
        currentDirection = offsetDirection;
        rb.velocity = currentDirection * bounceForce;
        StartCoroutine(BounceCooldown());
    }

    void BounceFromWall(RaycastHit2D hit)
    {
        Vector2 reflect = Vector2.Reflect(new Vector2(dashDir, 0), hit.normal).normalized;
        rb.velocity = reflect * bounceForce;
        wallBounce = true;
        StartCoroutine(BounceCooldown());
    }

    IEnumerator BounceCooldown()
    {
        yield return new WaitForSeconds(1f);
        wallBounce = false;
    }

    public void Stun()
    {
        isStunned = true;
        currentState = State.StunState;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (isDashing)
            {
                isDashing = false;
                isCharging = false;
                StopAllCoroutines();
                rb.velocity = Vector2.zero;
                BounceFromPlayer();
                FindAnyObjectByType<PlayerHealth>().TakeDamage(10f);
                currentState = State.HoveringIdle;
                return;
            }
            if (currentState != State.StunState)
            {
                FindAnyObjectByType<PlayerHealth>().TakeDamage(10f);
                BounceFromPlayer();
            }
        }
    }

    public void ApplyKnockback(Transform player, float knockbackAmount)
    {
        if (isDead) return;

        StartCoroutine(HitRecoverTimer());
        Vector2 direction = (transform.position - player.position).normalized;
        if(isStunned)
        {
            rb.velocity = direction * (knockbackAmount + 50f);
        }
        else
        {
            rb.velocity = direction * knockbackAmount;
        }
        isKnockedBack = true;
    }
    IEnumerator HitRecoverTimer()
    {
        yield return new WaitForSeconds(knockbackTime);
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(hitRecoverTime);
        isKnockedBack = false;
        currentState = State.ChasePlayer;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class FlyingEnemyController : MonoBehaviour, EnemyStunable
{
    public float moveSpeed = 3f;
    public float bounceForce = 2f;
    private bool wallBounce;
    public LayerMask groundLayer;
    private float detectionDistance = 3f;

    private Vector2 currentDirection;

    public float idleFloatSpeed = 0.5f;
    public float idleFloatHeight = 0.5f;
    private Vector2 idleStartPos;
    private float floatTimer = 0f;

    [Range(0, 360)]
    public float viewDistance = 10f;
    public float viewAngle = 45f;
    public LayerMask playerMask;

    public bool isStunned;
    public float stunTimer;
    private float stunCountdown;
    public bool isDead = false;

    private enum State { HoveringIdle, ChasePlayer, StunState, EnemyDeath }
    private State currentState = State.HoveringIdle;

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
                Vector2 targetDirection = ((Vector2)(player.position - transform.position)).normalized;
                currentDirection = targetDirection;
                if (!CanSeePlayer())
                {
                    currentState = State.HoveringIdle;
                }
                break;
            }

            case State.StunState:
            {
                anim.SetBool("isStunned", true);
                rb.velocity = Vector2.zero;
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
            if (currentDirection.x > 0)
            {
                spriteRenderer.flipX = false;
            }
            else if (currentDirection.x < 0)
            {
                spriteRenderer.flipX = true;
            }

            DetectWallsAndGround();
        }
        else
        {
            currentState = State.EnemyDeath;
        }
    }

    private void FixedUpdate()
    {
        if (!wallBounce && !isStunned && !isDead)
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
            FindAnyObjectByType<PlayerHealth>().TakeDamage(10f);

            wallBounce = true;
            Vector2 bounceDirection = Vector2.Reflect(currentDirection, player.position).normalized;
            Vector2 toPlayer = ((Vector2)(player.position - transform.position)).normalized;
            float blendTowardPlayer = 0.3f;
            Vector2 offsetDirection = Vector2.Lerp(bounceDirection, toPlayer, blendTowardPlayer).normalized;
            currentDirection = offsetDirection;
            rb.velocity = currentDirection * bounceForce;
            StartCoroutine(BounceCooldown());
        }
    }
}

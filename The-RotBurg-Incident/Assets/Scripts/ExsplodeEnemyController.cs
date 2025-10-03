using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExsplodeEnemyController : MonoBehaviour, EnemyStunable
{
    public float moveSpeed = 3f;
    public LayerMask groundLayer;
    public float explosionRadius = 3f;
    public bool isDead = false;

    private Vector2 currentDirection;

    [Range(0, 360)]
    public float viewDistance = 10f;
    public float viewAngle = 45f;
    public LayerMask playerMask;

    public bool isStunned;
    public float stunTimer;
    private float stunCountdown;

    public float patrolDuration = 2f;      
    private float patrolTimer;
    private bool patrolMovingRight = true; 

    private enum State { DetectingIdle, ChasePlayer, StunState, EnemyIgnite }
    private State currentState = State.DetectingIdle;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Transform player;
    private EnemyHealth health;
    private Animator anim;
    private Collider2D enemyCollider;
    private Collider2D playerCollider;
    public GameObject exsplosionObject;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        health = GetComponent<EnemyHealth>();
        enemyCollider = GetComponent<Collider2D>();
        playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();

        stunCountdown = stunTimer;
    }


    void Update()
    {
        switch (currentState)
        {
            case State.DetectingIdle:
                {
                    PatrolIdle();
                    if (CanSeePlayer())
                    {
                        currentState = State.ChasePlayer;
                    }
                    break;
                }

            case State.ChasePlayer:
                {
                    Vector2 targetDirection = ((Vector2)(player.position - transform.position)).normalized;
                    currentDirection.x = targetDirection.x;

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

            case State.EnemyIgnite:
                {
                    moveSpeed = moveSpeed = 35f;
                    Vector2 targetDirection = ((Vector2)(player.position - transform.position)).normalized;
                    currentDirection.x = targetDirection.x;
                    Physics2D.IgnoreCollision(enemyCollider, playerCollider, true);
                    break;
                }
        }
        Debug.Log(currentState);
    }

    private void FixedUpdate()
    {
        if (health.currentHealth > 0)
        {
            if (!isStunned && !isDead)
            {
                rb.velocity = currentDirection * moveSpeed;

                if (currentDirection.x > 0)
                {
                    spriteRenderer.flipX = false;
                }
                else if (currentDirection.x < 0)
                {
                    spriteRenderer.flipX = true;
                }
            }
        }
        else
        {
            currentState = State.EnemyIgnite;
        }
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

    void PatrolIdle()
    {
        patrolTimer -= Time.deltaTime;

        if (patrolTimer <= 0f)
        {
            patrolMovingRight = !patrolMovingRight; 
            patrolTimer = patrolDuration;
        }

        currentDirection = patrolMovingRight ? Vector2.right : Vector2.left;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            currentState = State.EnemyIgnite;
            anim.SetBool("IsDead", true);
        }
    }

    public void Stun()
    {
        if (currentState != State.EnemyIgnite)
        {
            isStunned = true;
            currentState = State.StunState;
        }
    }

    public void ExsplodeEnemy()
    {
        Instantiate(exsplosionObject, transform.position + new Vector3(0f, 3f, 0f), Quaternion.identity);

        Collider2D playerHit = Physics2D.OverlapCircle(transform.position, explosionRadius, playerMask);
        if (playerHit.CompareTag("Player"))
        {
            FindAnyObjectByType<PlayerHealth>().TakeDamage(20f);
        }

        isDead = true;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
    }
}

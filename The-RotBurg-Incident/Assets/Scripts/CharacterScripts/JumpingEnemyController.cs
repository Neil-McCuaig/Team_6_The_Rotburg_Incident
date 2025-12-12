using System.Collections;
using UnityEngine;

public class JumpingEnemyController : MonoBehaviour, EnemyStunable, EnemyKnockbackable
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float jumpForceX = 5f;
    public float jumpForceY = 8f;
    public float maxJumpY;
    public float jumpDistance = 3f;
    public float jumpCooldown = 1.5f;
    private bool isJumping = false;
    private float jumpTimer = 0f;

    [Header("Patrol Settings")]
    public float patrolDuration = 2f;
    private float patrolTimer;
    private bool patrolMovingRight = true;
    private Vector2 currentDirection;
    [Range(0, 360)]
    public float viewDistance = 10f;
    public float viewAngle = 45f;
    public LayerMask playerMask;

    [Header("Damage/Recoil Settings")]
    public GameObject bloodSplatter;
    public float attackDamage;
    public bool isDead;
    private bool isBounce = false;
    public float bounceForce = 5f;
    public float bounceCooldownTime = 0.5f;
    public float knockbackTime = 0.15f;
    public float hitRecoverTime = 0.5f;

    [Header("Stunned Settings")]
    public bool isStunned = false;
    public float stunTimer;
    private float stunCountdown;

    [Header("References")]
    public Transform player;
    private Rigidbody2D rb;
    private Animator enemyAnim;
    private Collider2D enemyCollider;
    private Collider2D playerCollider;
    EnemyHealth health;

    enum State { Idle, Attack, Stunned, Knockback, Death }
    State currentState = State.Idle;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemyCollider = GetComponent<Collider2D>();
        playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        enemyAnim = GetComponent<Animator>();
        health = GetComponent<EnemyHealth>();
        jumpTimer = jumpCooldown;
        stunCountdown = stunTimer;
    }

    private void Update()
    {
        if (player == null)
        { 
            return;
        }
        else if (health.currentHealth <= 0)
        {
            currentState = State.Death;
        }

            switch (currentState)
            {
                case State.Idle:
                    {
                        patrolTimer -= Time.deltaTime;

                        if (patrolTimer <= 0f)
                        {
                            patrolMovingRight = !patrolMovingRight;
                            patrolTimer = patrolDuration;

                            Vector3 scale = transform.localScale;
                            scale.x = patrolMovingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
                            transform.localScale = scale;
                        }

                        currentDirection.x = patrolMovingRight ? 1f : -1f;

                        if (CanSeePlayer())
                        {
                            currentState = State.Attack;
                        }
                        break;
                    }
                case State.Attack:
                    {
                        enemyAnim.SetBool("isStunned", false);
                        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
                        float direction = Mathf.Sign(player.position.x - transform.position.x);

                        if (distanceToPlayer > jumpDistance && !isJumping)
                        {
                            rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);
                        }
                        else if (distanceToPlayer < jumpDistance && !isJumping)
                        {
                            rb.velocity = new Vector2(direction * (moveSpeed / 5), rb.velocity.y);
                            HandleJumpCycle();
                        }
                        FlipTowardsPlayer();
                        if (!CanSeePlayer())
                        {
                            currentState = State.Idle;
                        }

                        break;
                    }
                case State.Stunned:
                    {
                        rb.velocity = Vector2.down * moveSpeed;
                        enemyAnim.SetBool("isStunned", true);
                        stunCountdown -= Time.deltaTime;
                        if (stunCountdown <= 0f)
                        {
                            isStunned = false;
                            stunCountdown = stunTimer;
                            rb.mass = 40;
                            currentState = State.Attack;
                        }
                        break;
                    }
                case State.Knockback:
                    {
                        break;
                    }
                case State.Death:
                    {
                        isDead = true;
                        Physics2D.IgnoreCollision(enemyCollider, playerCollider, true);
                        rb.velocity = Vector2.down * moveSpeed;
                        break;
                    }
            }
    }

    private void FixedUpdate()
    {
        if (currentState != State.Attack && currentState != State.Knockback && isBounce == false && !isStunned && !isDead)
        {
            rb.velocity = new Vector2((currentDirection.x * moveSpeed), rb.velocity.y);
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
    private void HandleJumpCycle()
    {
        jumpTimer -= Time.deltaTime;
        if (jumpTimer <= 0 && !isJumping)
        {
            JumpTowardsPlayer();
        }
    }
    private void JumpTowardsPlayer()
    {
        isJumping = true;
        jumpTimer = jumpCooldown;

        Vector2 jumpDirection = (player.position - transform.position).normalized;

        float jumpVelX = jumpDirection.x * jumpForceX;
        float jumpVelY = jumpDirection.y * jumpForceY;
        jumpVelY = Mathf.Clamp(jumpVelY, 0f, maxJumpY);
        rb.velocity = new Vector2(jumpVelX, jumpVelY);
    }
    private void FlipTowardsPlayer()
    {
        Vector3 scale = transform.localScale;
        if (player.position.x > transform.position.x)
        {
            scale.x = Mathf.Abs(scale.x);
        }
        else
        {
            scale.x = -Mathf.Abs(scale.x);
        }
        transform.localScale = scale;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (currentState != State.Stunned)
            {
                FindAnyObjectByType<PlayerHealth>().TakeDamage(attackDamage);
                isBounce = true;
                Vector2 awayFromPlayer = (transform.position - collision.transform.position).normalized;
                rb.velocity = awayFromPlayer * bounceForce;
                StartCoroutine(BounceCooldown());
            }
        }
        if (collision.contacts[0].normal.y > 0.5f && collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isJumping = false;
        }
    }
    private IEnumerator BounceCooldown()
    {
        yield return new WaitForSeconds(bounceCooldownTime);
        isBounce = false;
    }
    public void Stun()
    {
        isStunned = true;
        rb.mass = 100;
        currentState = State.Stunned;
    }
    public void ApplyKnockback(Transform player, float knockbackAmount)
    {
        Instantiate(bloodSplatter, transform.position, Quaternion.identity);

        currentState = State.Knockback;
        StartCoroutine(HitRecoverTimer());

        Vector2 direction = (gameObject.transform.position - player.position).normalized;
        if(isJumping)
        {
            rb.velocity = direction * 5f;
        }
        else
        {
            rb.velocity = direction * knockbackAmount;
        }
    }
    IEnumerator HitRecoverTimer()
    {
        yield return new WaitForSeconds(knockbackTime);
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(hitRecoverTime);
        currentState = State.Attack;
    }
}

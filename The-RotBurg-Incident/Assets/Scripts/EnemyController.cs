using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, EnemyStunable
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 velocity;
    private SpriteRenderer spriteRenderer;
    private Animator enemyAnim;
    private EnemyHealth health;
    private Collider2D collision;

    public bool isStunned;
    public float stunTimer;
    private float stunCountdown;

    public float patrolDuration = 2f;
    private float patrolTimer;
    private bool patrolMovingRight = true;

    private Transform wallCheck;
    public LayerMask groundLayer;

    private PlayerController playerController;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyAnim = GetComponent<Animator>();
        health = GetComponent<EnemyHealth>();
        collision = GetComponent<Collider2D>();
        wallCheck = transform.Find("WallCheck");

        isStunned = false;
        stunCountdown = stunTimer;

        playerController = FindAnyObjectByType<PlayerController>();
    }

    private void Update()
    {
        if (!isStunned)
        {
            enemyAnim.SetBool("isStunned", false);
            patrolTimer -= Time.deltaTime;

            if (patrolTimer <= 0f)
            {
                patrolMovingRight = !patrolMovingRight;
                patrolTimer = patrolDuration;
            }

            velocity.x = patrolMovingRight ? 1f : -1f;
        }
        else
        {
            enemyAnim.SetBool("isStunned", true);

            stunCountdown -= Time.deltaTime;
            if (stunCountdown <= 0f)
            {
                isStunned = false;
                stunCountdown = stunTimer;
                rb.constraints = RigidbodyConstraints2D.None;
            }
            return;
        }

        if (health.currentHealth > 0)
        {
            rb.velocity = velocity * moveSpeed;

            if (velocity.x > 0)
            {
                spriteRenderer.flipX = false;
            }
            else if (velocity.x < 0)
            {
                spriteRenderer.flipX = true;
            }
        }
        else
        {
            collision.enabled = false;
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    public void Stun()
    {
        isStunned = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && playerController.isHiding == false)
        {
            FindAnyObjectByType<PlayerHealth>().TakeDamage(10f);
        }
    }
}

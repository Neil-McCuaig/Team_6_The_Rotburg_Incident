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

    bool facingRight = true;
    bool hitWall = false;
    public bool isStunned;
    public float stunTimer;
    private float stunCountdown;

    private Transform wallCheck;
    public LayerMask groundLayer;

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
    }

    private void Update()
    {
        hitWall = Physics2D.OverlapCircle(wallCheck.position, 0.1f, groundLayer);

        if (!isStunned)
        {
            enemyAnim.SetBool("isStunned", false);
            if (facingRight)
            {
                velocity.x = moveSpeed;
            }
            else
            {
                velocity.x = -moveSpeed;
            }

            if (hitWall && facingRight)
            {
                Vector3 localScale = transform.localScale;
                localScale.x *= -1;
                transform.localScale = localScale;

                facingRight = false;
            }
            else if (hitWall && !facingRight)
            {
                Vector3 localScale = transform.localScale;
                localScale.x *= -1;
                transform.localScale = localScale;

                facingRight = true;
            }
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
            rb.velocity = velocity;
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
        if (other.gameObject.CompareTag("Player"))
        {
            FindAnyObjectByType<PlayerHealth>().TakeDamage(10f);
        }
    }
}

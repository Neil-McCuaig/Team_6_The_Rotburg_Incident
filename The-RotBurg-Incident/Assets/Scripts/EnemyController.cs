using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 velocity;
    private SpriteRenderer spriteRenderer;

    bool facingRight = true;
    bool hitWall = false;
    public bool isStunned = false;

    public Transform wallCheck;
    public LayerMask groundLayer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        hitWall = Physics2D.OverlapCircle(wallCheck.position, 0.1f, groundLayer);
        if (!isStunned)
        {
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
            velocity.x = 0f;
        }

            rb.velocity = velocity;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemyController : MonoBehaviour
{
    public Transform player;
    public float hoverSpeed = 2f;
    public float hoverDistance = 3f;
    public float moveAboveSpeed = 5f;
    public float slamSpeed = 15f;
    public float returnSpeed = 7f;
    public float slamCooldown = 2f;
    public float groundCheckDistance = 0.5f;
    public LayerMask groundLayer;

    private enum State { Hovering, MovingAbovePlayer, Slamming, Returning }
    private State currentState = State.Hovering;

    private Vector2 startPosition;
    private Rigidbody2D rb;
    private float slamTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        slamTimer = slamCooldown;
        startPosition = transform.position;
    }

    void Update()
    {
        if (player == null) return;

        switch (currentState)
        {
            case State.Hovering:
                Hover();
                slamTimer -= Time.deltaTime;
                if (slamTimer <= 0f)
                {
                    currentState = State.MovingAbovePlayer;
                }
                break;

            case State.MovingAbovePlayer:
                MoveAbovePlayer();
                break;

            case State.Slamming:
                SlamDown();
                break;

            case State.Returning:
                ReturnToAir();
                break;
        }
    }

    void Hover()
    {
        Vector2 targetPos = (Vector2)player.position + new Vector2(hoverDistance, Mathf.Sin(10));
        transform.position = Vector2.Lerp(transform.position, targetPos, hoverSpeed);
    }

    void MoveAbovePlayer()
    {
        Vector2 targetPos = new Vector2(player.position.x, player.position.y + 10f);
        transform.position = Vector2.MoveTowards(transform.position, targetPos, moveAboveSpeed);

        float distanceAbove = transform.position.y - player.position.y;
        if (distanceAbove <= 6f && distanceAbove >= 4.5f)
        {
            currentState = State.Slamming;
        }
    }

    void SlamDown()
    {
        rb.velocity = new Vector2(0, -slamSpeed);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        if (hit.collider != null)
        {
            rb.velocity = Vector2.zero;
            currentState = State.Returning;
        }
    }

    void ReturnToAir()
    {
        Vector2 targetPos = (Vector2)player.position + new Vector2(hoverDistance, 3f);
        transform.position = Vector2.MoveTowards(transform.position, targetPos, returnSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPos) < 0.5f)
        {
            slamTimer = slamCooldown;
            currentState = State.Hovering;
        }
    }
}

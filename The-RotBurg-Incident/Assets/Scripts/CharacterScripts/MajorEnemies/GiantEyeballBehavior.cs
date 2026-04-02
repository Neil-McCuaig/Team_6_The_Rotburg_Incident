using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantEyeballBehavior : MonoBehaviour, MonnsterActivation, EnemyStunable
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public bool facingRight = true;
    public bool isActive;
    private Vector2 sleepingTransform;

    [Header("Stunned Settings")]
    public bool isStunned = false;
    public float stunTimer;
    private float stunCountdown; 

    private enum State { FollowState, Stunned }
    private State currentState = State.FollowState;

    Transform player;
    PlayerController playerController;
    Animator anim;


    void Awake()
    {
        Vector3 currentPos = transform.position;
        sleepingTransform = new Vector2(currentPos.x, currentPos.y);
    }
    private void Start()
    {
        anim = GetComponent<Animator>();
        playerController = FindAnyObjectByType<PlayerController>();
        player = playerController.transform;
        stunCountdown = stunTimer;
    }

    void Update()
    {
        if (!isActive)
        {
            transform.position = sleepingTransform;
            return;
        }

        if (playerController.inLocker)
        {
            currentState = State.Stunned;
        }

        switch (currentState)
        {
            case State.FollowState:
                {
                    if (playerController.inLocker)
                    {
                        currentState = State.Stunned;
                    }
                    anim.SetBool("isStunned", false);
                    MoveTowardTarget();
                    break;
                }
            case State.Stunned:
                {
                    anim.SetBool("isStunned", true);
                    stunCountdown -= Time.deltaTime;
                    if (stunCountdown <= 0f)
                    {
                        isStunned = false;
                        stunCountdown = stunTimer;
                        currentState = State.FollowState;
                    }
                    break;
                }
        }

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance < 0.5f)
        {
            FindAnyObjectByType<PlayerHealth>().TakeDamage(100f, this.gameObject);
            transform.position = sleepingTransform;
        }
    }
    void MoveTowardTarget()
    {
        if (player == null)
        {
            return;
        }

        Vector2 newPosition = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        transform.position = newPosition;

        Vector2 directionToPlayer = player.position - transform.position;
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        angle += 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        float direction = player.position.x - transform.position.x;
        if (direction > 0 && !facingRight)
        {
            Flip();
        }
        else if (direction < 0 && facingRight)
        {
            Flip();
        }
    }
    void Flip()
    {
        facingRight = !facingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    public void SetActiveState(bool value)
    {
        isActive = value;
    }
    public void Stun()
    {
        isStunned = true;
        currentState = State.Stunned;
    }
}

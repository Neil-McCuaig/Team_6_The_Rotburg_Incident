using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeepingAngelBehavior : MonoBehaviour, MonnsterActivation
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public bool facingRight = true;
    public bool isActive;
    private Vector2 sleepingTransform;

    [Header("Light Detection")]
    public bool isInLight;

    private enum State { FollowState, HideState }
    private State currentState = State.FollowState;

    public Transform player;
    public DomainZoneLogic DomainZone;
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
    }

    void Update()
    {
        if (!isActive)
        {
            transform.position = sleepingTransform;
            return;
        }

        if (isInLight)
        {
            currentState = State.HideState;
        }
        else if (!playerController.inLocker)
        {
            currentState = State.FollowState;
        }

        switch (currentState)
        {
            case State.FollowState:
                {
                    if(playerController.inLocker)
                    {
                        currentState = State.HideState;
                    }
                    MoveTowardTarget();
                    break;
                }
            case State.HideState:
                {
                    HandleHideState();
                    break;
                }
        }

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance < 0.5f)
        {
            FindAnyObjectByType<PlayerHealth>().TakeDamage(100f);
            transform.position = sleepingTransform;
        }
    }
    void HandleHideState()
    {
        anim.SetBool("IsHiding", true);
    }

    void MoveTowardTarget()
    {
        if (player == null)
        {
            return;
        }
        anim.SetBool("IsHiding", false);

        Vector2 newPosition = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        transform.position = newPosition;

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
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("FlashLight"))
        {
            isInLight = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("FlashLight"))
        {
            isInLight = false;
        }
    }

    public void SetActiveState(bool value)
    {
        isActive = value;
    }
}

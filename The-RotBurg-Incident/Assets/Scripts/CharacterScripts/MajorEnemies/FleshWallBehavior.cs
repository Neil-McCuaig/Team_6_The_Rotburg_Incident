using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleshWallBehavior : MonoBehaviour, MonnsterActivation
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public bool facingRight = true;
    public bool isActive;
    private Vector2 sleepingTransform;

    [Header("StartPoint")]
    public Transform chaseSequenceStartPoint;

    private enum State { FollowState, FreezeState, ChaseBeginState }
    private State currentState = State.FreezeState;

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
    }

    void Update()
    {
        if (!isActive)
        {
            transform.position = sleepingTransform;
            currentState = State.FreezeState;
            return;
        }

        switch (currentState)
        {
            case State.FollowState:
                {
                    MoveTowardTarget();
                    break;
                }
            case State.ChaseBeginState:
                {
                    transform.position = chaseSequenceStartPoint.position;
                    currentState = State.FollowState;
                    break;
                }
            case State.FreezeState:
                {
                    //transform.position = chaseSequenceStartPoint.position;
                    BeginChaseSequence();
                    break;
                }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();

            if (health != null)
            {
                health.TakeDamage(100f, this.gameObject);
            }

            transform.position = sleepingTransform;
        }
    }
    void MoveTowardTarget()
    {
        if (player == null)
        {
            return;
        }

        Vector2 targetPosition = new Vector2(player.position.x + 10f, player.position.y + 15f);
        Vector2 newPosition = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        transform.position = newPosition;
    }
    public void SetActiveState(bool value)
    {
        isActive = value;
    }
    public void BeginChaseSequence()
    {
        currentState = State.ChaseBeginState;
    }
}

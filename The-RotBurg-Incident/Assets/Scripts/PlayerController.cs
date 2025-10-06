using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public Animator anim;
    private Collider2D collision;
    GameManager manager;
    FadeToBlack fader;
    PlayerHealth health;
    EnemySpawnerManager enemySpawnerManager;
    private Vector2 respawnPoint;
    private bool isDead;
    public float deathFadeDelay = 1f;

    [Header("Attack Settings")]
    public float attackCooldown = 1f;  
    private float lastAttackTime = 0f;
    public float damageAmount;
    public GameObject attackPointA;
    public GameObject attackPointB;
    public GameObject attackPosition;
    public float attackRadius;
    public LayerMask enemies;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public Vector2 moveInput;
    private Vector2 velocity;
    public float jumpForce = 10f;
    public float gravityScale = 3f;
    public float terminalVelocity = -15f;
    public bool isSitting = false;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 1f;
    public LayerMask groundLayer;
    private bool isGrounded;

    [Header("Arm Aiming")]
    public Transform arm;
    public Transform aimLeft;
    public Transform aimRight;
    private Vector2 aimInput;
    Vector2 lastAimDirection = Vector2.right;
    float lastAngle = 0f;
    public bool flipArmLeft = true;
    public SpriteRenderer armRender;

    [Header("Flash References")]
    public float drainAmount;
    public GameObject stunEffect;
    public Transform cameraFlash;
    public Transform flashLeft;
    public Transform flashRight;
    public bool canFlash = true;
    public bool batteryDead;
    public SpriteRenderer effectRender;

    [Header("Input Actions")]
    public InputActionAsset inputActions;
    public InputAction moveAction;
    public InputAction jumpAction;
    public InputAction attackAction;
    private InputAction aimAction;
    private InputAction flashAction;

    [Header("Power-Ups")]
    public bool hasDoubleJump = false;
    private int numOfJumps = 2;



    private void Awake()
    {
        var playerActions = inputActions.FindActionMap("BaseGameplay");
        moveAction = playerActions.FindAction("MoveX");
        jumpAction = playerActions.FindAction("Jump");
        attackAction = playerActions.FindAction("Attack");
        aimAction = playerActions.FindAction("AimDirection");
        flashAction = playerActions.FindAction("ActionFlash");

        health = FindAnyObjectByType<PlayerHealth>();
        manager = FindAnyObjectByType<GameManager>();
        fader = FindAnyObjectByType<FadeToBlack>();
        enemySpawnerManager = FindAnyObjectByType<EnemySpawnerManager>();
        collision = GetComponent<Collider2D>();


        fader.FadeIn();
    }

    void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        attackAction.Enable();
        aimAction.Enable();
        flashAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        attackAction.Disable();
        aimAction.Disable();
        flashAction.Disable();
    }

    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();

        if (!isSitting && !isDead) 
        { 
            CheckInput();
            AimingDirection();
        }
    }

    void FixedUpdate()
    {
        if (!isDead)
        {
            HandleMovement();
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    void CheckInput()
    {
        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (!hasDoubleJump)
        {
            // Jump
            if (jumpAction.WasPressedThisFrame() && isGrounded)
            {
                velocity.y = jumpForce;
            }
        }
        else
        {
            // Jump
            if (jumpAction.WasPressedThisFrame() && isGrounded)
            {
                velocity.y = jumpForce;
                numOfJumps--;
            }
            if (jumpAction.WasPressedThisFrame() && !isGrounded && numOfJumps != 0)
            {
                velocity.y = jumpForce / 1.4f;
                numOfJumps = 0;
            }
            else if (isGrounded)
            {
                numOfJumps = 2;
            }
        }

        if (moveInput.x > 0)
        {
            spriteRenderer.flipX = false;
            arm.position = aimLeft.position;
            attackPosition.transform.position = attackPointA.transform.position;
        }
        else if (moveInput.x < 0)
        {
            spriteRenderer.flipX = true;
            arm.position = aimRight.position;
            attackPosition.transform.position = attackPointB.transform.position;
        }

        if (attackAction.WasPressedThisFrame())
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                anim.SetBool("IsAttacking", true);
                lastAttackTime = Time.time;
            }
        }
    }
    public void PlayerAttack()
    {
        Collider2D[] enemy = Physics2D.OverlapCircleAll(attackPosition.transform.position, attackRadius, enemies);

        foreach (Collider2D enemyGameObject in enemy)
        {
            enemyGameObject.GetComponent<EnemyHealth>().health -= damageAmount;
        }
    }
    public void EndAttack()
    {
        anim.SetBool("IsAttacking", false);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackPosition.transform.position, attackRadius);
    }

    void HandleMovement()
    {
        velocity.x = moveInput.x * moveSpeed;

        if (!isGrounded || velocity.y > 0)
        {
            velocity.y += Physics2D.gravity.y * gravityScale * Time.fixedDeltaTime;
            velocity.y = Mathf.Max(velocity.y, terminalVelocity);
        }
        else
        {
            velocity.y = 0f;
        }

        rb.velocity = velocity;
    }

    void AimingDirection()
    {
        aimInput = aimAction.ReadValue<Vector2>();

        if (aimInput.sqrMagnitude > 0.01f)
        {
            lastAimDirection = aimInput.normalized;
            lastAngle = Mathf.Atan2(lastAimDirection.y, lastAimDirection.x) * Mathf.Rad2Deg;
        }

        arm.rotation = Quaternion.Euler(0f, 0f, lastAngle);
        
        if (flipArmLeft)
        {
            if (lastAngle > 130 || lastAngle < -60)
            {
                armRender.flipY = true;
                cameraFlash.position = flashLeft.position;
            }
            else
            {
                armRender.flipY = false;
                cameraFlash.position = flashRight.position;
            }
        }

        if (flashAction.WasPressedThisFrame() && canFlash && manager.batteryPercentage > 0)
        {
            ActivateFlash();
        }
    }

    void ActivateFlash()
    {
        manager.ReduceBattery(drainAmount);
        canFlash = false;
        stunEffect.SetActive(true);
        Color originalColor = effectRender.color;
        effectRender.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
        StartCoroutine(DecayFlash());
    }

    IEnumerator DecayFlash()
    {
        float elapsed = 0f;
        Color originalColor = effectRender.color;

        while (elapsed < 1f)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / 1f);
            effectRender.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        effectRender.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        stunEffect.SetActive(false);
        canFlash = true;
    }

    public void SetRespawnPoint(Vector2 point)
    {
        respawnPoint = point;
    }

    public void Respawn()
    {
        transform.position = respawnPoint;
        health.ResetHealthFull();
    }

    public void Die()
    {
        isDead = true;
        collision.enabled = false;

        anim.SetBool("IsDead", true);

        StartCoroutine(HandleDeathFadeOut());
    }

    private IEnumerator HandleDeathFadeOut()
    {
        yield return new WaitForSeconds(deathFadeDelay);

        fader.FadeOut();
        StartCoroutine(HandleDeathFadeIn());
    }
    private IEnumerator HandleDeathFadeIn()
    {
        yield return new WaitForSeconds(deathFadeDelay);

        isDead = false;
        collision.enabled = true;
        enemySpawnerManager.SpawnEnemies();
        Respawn();
        anim.SetBool("IsDead", false);
        fader.FadeIn();
    }
}

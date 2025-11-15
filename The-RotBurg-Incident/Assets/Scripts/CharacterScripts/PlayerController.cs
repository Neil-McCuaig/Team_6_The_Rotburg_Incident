using System.Collections;
using UnityEngine.Rendering.Universal;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public Animator anim;
    private Collider2D collision;
    GameManager manager;
    PauseMenuManager pauseManager;
    FadeToBlack fader;
    PlayerHealth health;
    EnemySpawnerManager enemySpawnerManager;
    [SerializeField] private Camera mainCamera;

    [Header("Player Checks")]
    private Vector2 respawnPoint;
    public bool isDead;
    public float deathFadeDelay = 1f;
    public bool isSitting = false;
    public bool inLocker = false;

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

    [Header("Jumping")]
    public float jumpForce = 10f;
    public float gravityScale = 3f;
    public float terminalVelocity = -15f;
    private bool cutJump = false;
    public float jumpCutMultiplier = 0.5f;
    public float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 1f;
    public LayerMask groundLayer;
    private bool isGrounded;

    [Header("Ceiling Check")]
    public Transform ceilingCheck;
    public float ceilingCheckRadius = 0.1f;
    private bool isTouchingCeiling;

    [Header("Arm Aiming")]
    public Transform arm;
    public Transform aimLeft;
    public Transform aimRight;
    private Vector2 aimInput;
    private Vector3 lastMousePosition;
    private float idleTimer;
    Vector2 lastAimDirection = Vector2.right;
    float lastAngle = 0f;
    public bool flipArmLeft = true;
    public SpriteRenderer armRender;

    [Header("Stun-Ability Settings")]
    public float drainAmount;
    public GameObject stunEffect;
    public Transform cameraFlash;
    public Transform flashLeft;
    public Transform flashRight;
    public Transform lightLeft;
    public Transform lightRight;
    public Light2D flashLight;
    public bool canFlash = true;
    public bool batteryDead;
    public SpriteRenderer effectRender;

    [Header("Input Actions")]
    public InputActionAsset inputActions;
    public InputAction moveAction;
    public InputAction jumpAction;
    public InputAction attackAction;
    public InputAction interactAction;
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
        interactAction = playerActions.FindAction("Interact");
        aimAction = playerActions.FindAction("AimDirection");
        flashAction = playerActions.FindAction("ActionFlash");

        health = FindAnyObjectByType<PlayerHealth>();
        manager = FindAnyObjectByType<GameManager>();
        pauseManager = FindAnyObjectByType<PauseMenuManager>();
        fader = FindAnyObjectByType<FadeToBlack>();
        enemySpawnerManager = FindAnyObjectByType<EnemySpawnerManager>();
        collision = GetComponent<Collider2D>();

        lastMousePosition = Input.mousePosition;
    }

    void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        attackAction.Enable();
        interactAction.Enable();
        aimAction.Enable();
        flashAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        attackAction.Disable();
        interactAction.Disable();
        aimAction.Disable();
        flashAction.Disable();
    }

    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();

        if (!isSitting && !isDead && !inLocker && !pauseManager.isPaused) 
        { 
            CheckInput();
            AimingDirection();
        }
    }

    void FixedUpdate()
    {
        if (!isDead && !inLocker)
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
        isTouchingCeiling = Physics2D.OverlapCircle(ceilingCheck.position, ceilingCheckRadius, groundLayer);
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (!hasDoubleJump)
        {
            if (jumpAction.WasPressedThisFrame() && (isGrounded || coyoteTimeCounter > 0f))
            {
                SoundManager.instance.PlaySound(SoundManager.instance.playerJump);
                velocity.y = jumpForce;
                coyoteTimeCounter = 0f;
            }
        }
        else
        {
            if (jumpAction.WasPressedThisFrame() && (isGrounded || coyoteTimeCounter > 0f))
            {
                SoundManager.instance.PlaySound(SoundManager.instance.playerJump);
                velocity.y = jumpForce;
                numOfJumps--;
                coyoteTimeCounter = 0f;
            }
            else if (jumpAction.WasPressedThisFrame() && numOfJumps > 0 && !isGrounded)
            {
                SoundManager.instance.PlaySound(SoundManager.instance.playerJump);
                velocity.y = jumpForce / 1.2f;
                numOfJumps = 0;
            }

            if (isGrounded)
            {
                numOfJumps = 2;
            }
        }

        if (jumpAction.WasReleasedThisFrame() && rb.velocity.y > 0f)
        {
            cutJump = true;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCutMultiplier);
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
                SoundManager.instance.PlaySound(SoundManager.instance.playerAttack);
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
            if (isTouchingCeiling && velocity.y > 0f)
            {
                velocity.y = -2f; 
            }
            else
            {
                velocity.y += Physics2D.gravity.y * gravityScale * Time.fixedDeltaTime;

                if (cutJump && velocity.y > 0)
                {
                    velocity.y *= jumpCutMultiplier;
                    cutJump = false;
                }

                velocity.y = Mathf.Max(velocity.y, terminalVelocity);
            }
        }
        else
        {
            velocity.y = 0f;
        }
        rb.velocity = velocity;
    }

    void AimingDirection()
    {
        Vector2 stickInput = aimAction.ReadValue<Vector2>();
        Vector3 mouseScreenPos = Input.mousePosition;

        if (Input.mousePosition != lastMousePosition)
        {
            idleTimer = 0f;
            lastMousePosition = Input.mousePosition;
        }
        else
        {
            idleTimer += Time.deltaTime;
        }

        if (idleTimer >= 1f)
        {
            if (stickInput.sqrMagnitude > 0.01f)
            {
                aimInput = stickInput.normalized;
                lastAimDirection = aimInput.normalized;
                lastAngle = Mathf.Atan2(lastAimDirection.y, lastAimDirection.x) * Mathf.Rad2Deg;
                arm.rotation = Quaternion.Euler(0f, 0f, lastAngle);
            }
        }
        else
        {
            Vector3 mouseWorldPos3D = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            mouseWorldPos3D.z = 0f;
            Vector2 direction = (mouseWorldPos3D - transform.position).normalized;
            lastAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        }

        arm.rotation = Quaternion.Euler(0f, 0f, lastAngle);

        if (flipArmLeft)
        {
            if (lastAngle > 90 || lastAngle < -90)
            {
                armRender.flipY = true;
                cameraFlash.position = flashLeft.position;
                flashLight.transform.position = lightLeft.position;
            }
            else
            {
                armRender.flipY = false;
                cameraFlash.position = flashRight.position;
                flashLight.transform.position = lightRight.position;
            }
        }

        if (flashAction.WasPressedThisFrame() && canFlash && manager.batteryPercentage > 0)
        {
            ActivateFlash();
        }
    }

    void ActivateFlash()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.playerFlash);
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
        armRender.enabled = true;
        health.ResetHealthFull();
    }

    public void Die()
    {
        isDead = true;
        collision.enabled = false;
        armRender.enabled = false;
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

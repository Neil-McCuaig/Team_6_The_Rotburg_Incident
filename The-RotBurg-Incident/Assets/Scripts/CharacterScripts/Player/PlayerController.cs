using System.Collections;
using System.Runtime.CompilerServices;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using static PlayerController;

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
    SafeStations safeStations;
    ViewerStats viewerStats;

    [Header("Camera Settings")]
    CameraFollowDirection cameraFollow;
    private float fallSpeedYDampingChangeThreshold;

    [Header("Player Checks")]
    private Vector2 respawnPoint;
    public bool isDead;
    public float deathFadeDelay = 1f;
    public bool inLocker = false;
    public bool canMove = true;
    public bool hasFlashlight = true;
    public bool canControl = true;

    [Header("Attack Settings")]
    public float attackCooldown;
    private float nextAttackTime = 0f;
    public float damageAmount;
    public float attackRadius;
    public LayerMask enemies;
    public GameObject attackEffectPrefab;
    public Transform attackSpawnPosition;

    [Header("Combo Settings")]
    public float comboResetTime = 0.6f;
    public float comboLockedTime;
    public float comboAttackDelay = 0.25f;
    private int currentComboCount = 0;
    private float lastComboInputTime;
    private bool attackDownNext = true;
    private bool isCurrentAttackUp;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public Vector2 moveInput;
    public Vector2 velocity;

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
    public LayerMask platformLayer;
    private bool isGrounded;
    private bool onPlatform;
    [HideInInspector]
    public Vector3 lastGroundedPosition;

    [Header("Ceiling Check")]
    public Transform ceilingCheck;
    public float ceilingCheckRadius = 0.1f;
    private bool isTouchingCeiling;

    [Header("Arm Aiming")]
    public Light2D personalLight;
    public Transform arm;
    public Transform aimLeft;
    public Transform aimRight;
    private Vector2 aimInput;
    private Vector3 lastMousePosition;
    public float armFlipAnglePos = 90f;
    public float armFlipAngleNeg = -90f;
    private float idleTimer;
    Vector2 lastAimDirection = Vector2.right;
    float lastAngle = 0f;
    public bool flipArmLeft = true;
    public SpriteRenderer armRender;

    [Header("Stun-Ability Settings")]
    public float flashIntensity = 1f;
    private float oriFlashIntensity;
    private float oriPictureIntensity;
    public float decayTime = 1f;
    public Transform lightLeft;
    public Transform lightRight;
    public Light2D flashLight;
    public Light2D pictureLight;
    public Light2D lighterLight;
    public bool canFlash = true;
    public bool batteryDead;

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
    public bool hasMetalPipe = false;
    public bool hasPhone = true;
    private int numOfJumps = 2;
    public int numOfLives = 3;

    public enum DeathType
    {
        Normal,
        Pouncer,
        Flyer,
        Popper,
        WeepingAngel,
        HallMonitor
    }
    private DeathType currentDeathType = DeathType.Normal;

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
        safeStations = FindAnyObjectByType<SafeStations>();
        viewerStats = FindAnyObjectByType<ViewerStats>();

        SetRespawnPoint();

        cameraFollow = FindAnyObjectByType<CameraFollowDirection>();
        fallSpeedYDampingChangeThreshold = -15f;

        lastMousePosition = Input.mousePosition;

        if (pictureLight != null)
        {
            oriPictureIntensity = pictureLight.intensity;
            pictureLight.intensity = 0f;
            pictureLight.gameObject.SetActive(false);
        }
        if (flashLight != null)
        {
            oriFlashIntensity = flashLight.intensity;
        }
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

        if (canMove && !isDead && !inLocker && !pauseManager.isPaused) 
        {
            CheckInput();
            AimingDirection();
        }
        if (rb.velocity.y < fallSpeedYDampingChangeThreshold && !CameraManager.instance.IsLerpingYDamping && !CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpYDamping(true);
        }
        if (rb.velocity.y >= 0f && !CameraManager.instance.IsLerpingYDamping && CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpedFromPlayerFalling = false;
            CameraManager.instance.LerpYDamping(false);
        }
       if (canControl == false)
        {
            canMove = false;
            anim.SetInteger("WalkX", 0);

        }
        if (canControl == true)
        {
            canMove = true;
        }

        if (personalLight != null)
        {
            float personalLightRadius = viewerStats.personalLightRadius;
            personalLight.pointLightOuterRadius = personalLightRadius;
        }
    }

    void FixedUpdate()
    {
        if (canMove && !isDead && !inLocker)
        {
            HandleMovement();
            anim.SetFloat("VelocityY", Mathf.Abs(rb.velocity.y));
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
        onPlatform = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, platformLayer);

        if (isGrounded || onPlatform)
        {
            coyoteTimeCounter = coyoteTime;
            if (isGrounded)
            {
                lastGroundedPosition = transform.position;
            }
            anim.SetBool("IsJumping", false);
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
            anim.SetBool("IsJumping", true);
        }

        if (!hasDoubleJump)
        {
            if (jumpAction.WasPressedThisFrame() && ((isGrounded || onPlatform) || coyoteTimeCounter > 0f))
            {
                SoundManager.instance.PlaySound(SoundManager.instance.playerJump);
                velocity.y = jumpForce;
                coyoteTimeCounter = 0f;
            }
        }
        else
        {
            if (jumpAction.WasPressedThisFrame() && ((isGrounded || onPlatform) || coyoteTimeCounter > 0f))
            {
                SoundManager.instance.PlaySound(SoundManager.instance.playerJump);
                velocity.y = jumpForce;
                numOfJumps--;
                coyoteTimeCounter = 0f;
            }
            else if (jumpAction.WasPressedThisFrame() && numOfJumps > 0 && (!isGrounded || !onPlatform))
            {
                SoundManager.instance.PlaySound(SoundManager.instance.playerJump); 
                velocity.y = jumpForce / 1.2f;
                numOfJumps = 0;
            }

            if (isGrounded || onPlatform)
            {
                numOfJumps = 2;
            }
        }

        if (jumpAction.WasReleasedThisFrame() && rb.velocity.y > 0f && (!isGrounded || !onPlatform))
        {
            cutJump = true;
        }

        if (moveInput.x > 0)
        {
            anim.SetInteger("WalkX", 1);
            spriteRenderer.flipX = false;
            arm.position = aimLeft.position;

            cameraFollow.CallTurn(false);
        }
        else if (moveInput.x < 0)
        {
            anim.SetInteger("WalkX", -1);
            spriteRenderer.flipX = true;
            arm.position = aimRight.position;

            cameraFollow.CallTurn(true);
        }
        else if (moveInput.x == 0)
        {
            anim.SetInteger("WalkX", 0);
        }

        if (attackAction.WasPressedThisFrame() && hasMetalPipe)
        {
            if (Time.time < comboLockedTime)
            {
                Debug.Log("Waiting for combo cooldown...");
                return;
            }

            // Prevent attacks from coming out too quickly
            if (Time.time < nextAttackTime)
            {
                return;
            }

            if (currentComboCount > 0 && Time.time - lastComboInputTime > comboResetTime)
            {
                currentComboCount = 0;
                attackDownNext = true;
            }

            currentComboCount++;
            lastComboInputTime = Time.time;

            // Set the delay before next attack
            nextAttackTime = Time.time + comboAttackDelay;

            if (attackDownNext)
            {
                anim.SetTrigger("AttackDown");
                isCurrentAttackUp = false;
            }
            else
            {
                anim.SetTrigger("AttackUp");
                isCurrentAttackUp = true;
            }

            attackDownNext = !attackDownNext;

            if (currentComboCount >= viewerStats.maxAttackAmount)
            {
                comboLockedTime = Time.time + attackCooldown;
                currentComboCount = 0;
            }

            SoundManager.instance.PlaySound(SoundManager.instance.playerAttack);
        }
    }
    public void PlayerAttack()
    {
        if (attackEffectPrefab != null)
        {
            GameObject attackEffect = Instantiate(attackEffectPrefab, attackSpawnPosition.position, arm.rotation, attackSpawnPosition);
            SpriteRenderer sr = attackEffect.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                float z = arm.eulerAngles.z;
                if (z > 180f)
                {
                    z -= 360f;
                }

                bool isBetweenNeg90And90 = z > -90f && z < 90f;

                sr.flipX = false;

                sr.flipY = isBetweenNeg90And90 ? isCurrentAttackUp : !isCurrentAttackUp;
            }
        }

        Collider2D[] enemy = Physics2D.OverlapCircleAll(attackSpawnPosition.position, attackRadius, enemies);

        foreach (Collider2D enemyGameObject in enemy)
        {
            enemyGameObject.GetComponent<EnemyHealth>().health -= damageAmount;

            viewerStats.AddViewers(10);

            EnemyKnockbackable applyKnockback = enemyGameObject.GetComponent<EnemyKnockbackable>();

            if (applyKnockback != null)
            {
                float knockBackAmount = viewerStats.knockbackAmount;
                applyKnockback.ApplyKnockback(this.transform, knockBackAmount);
            }
        }
    }
    public void EndAttack()
    {
        anim.SetBool("IsAttacking", false);

        if (currentComboCount >= viewerStats.maxAttackAmount || Time.time - lastComboInputTime > comboResetTime)
        {
            currentComboCount = 0;
            attackDownNext = true;
        }
    }
    public void EndCharging()
    {
        safeStations.StopCharging();
        EnableArmRender();
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackSpawnPosition.transform.position, attackRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
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
                if (cutJump)
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
            if (lastAngle > armFlipAnglePos || lastAngle < armFlipAngleNeg)
            {
                armRender.flipY = true;
                pictureLight.transform.position = lightLeft.position;
                flashLight.transform.position = lightLeft.position;
            }
            else
            {
                armRender.flipY = false;
                pictureLight.transform.position = lightRight.position;
                flashLight.transform.position = lightRight.position;
            }
        }

        if (flashAction.WasPressedThisFrame() && canFlash && manager.batteryPercentage > 0 && hasPhone == true)
        {
            ActivateFlash();
        }
    }

    void ActivateFlash()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.playerFlash);
        float flashDrainRate = viewerStats.flashDrainRate;
        manager.ReduceBattery(flashDrainRate);
        canFlash = false;

        StartCoroutine(DecayFlash());
    }

    IEnumerator DecayFlash()
    {
        float elapsed = 0f;
        if (pictureLight != null)
        {
            pictureLight.gameObject.SetActive(true);
            pictureLight.intensity = flashIntensity;
        }
        while (elapsed < decayTime)
        {
            float t = elapsed / decayTime;

            if (pictureLight != null)
            {
                pictureLight.intensity = Mathf.Lerp(flashIntensity, 0f, t);
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (pictureLight != null)
        {
            pictureLight.intensity = 0f;
            pictureLight.gameObject.SetActive(false);
        }
        canFlash = true;
    }

    public void SetRespawnPoint()
    {
        Vector3 currentPos = transform.position;
        respawnPoint = new Vector2(currentPos.x, currentPos.y);
    }

    public void Respawn()
    {
        transform.position = respawnPoint;
        health.ResetHealthFull();
        if (currentDeathType == DeathType.WeepingAngel)
        {
            health.ResetWeepingSpriteRenderer();
        }
    }

    public void Die(DeathType deathType = DeathType.Normal)
    {
        isDead = true;
        collision.enabled = false;
        DisableArmRender();

        anim.SetBool("IsDead", true);

        currentDeathType = deathType;
        anim.SetTrigger(GetDeathTrigger(currentDeathType));

        StartCoroutine(HandleDeathFadeOut());
    }

    private string GetDeathTrigger(DeathType type)
    {
        switch (type)
        {
            case DeathType.Pouncer:
                return "PouncerDeath";

            case DeathType.Flyer:
                return "FlyerDeath";

            case DeathType.Popper:
                return "PopperDeath";

            case DeathType.WeepingAngel:
                return "WeepingDeath";

            case DeathType.HallMonitor:
                return "HallMonitorDeath";

            default:
                return "NormalDeath"; 
        }
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

        safeStations.TriggerCharge();
        isDead = false;
        collision.enabled = true;
        enemySpawnerManager.SpawnEnemies();
        Respawn();
        anim.SetBool("IsDead", false);
        fader.FadeIn();
    }
    public void DisableArmRender()
    {
        armRender.enabled = false;
        flashLight.intensity = 0f;
    }
    public void EnableArmRender()
    {
        armRender.enabled = true;
        flashLight.intensity = oriFlashIntensity;
    }

}

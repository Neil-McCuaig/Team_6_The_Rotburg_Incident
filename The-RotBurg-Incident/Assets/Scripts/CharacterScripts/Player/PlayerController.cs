using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

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

    [Header("Attack Settings")]
    public float attackCooldown = 1f;  
    private float lastAttackTime = 0f;
    public float knockBackAmount;
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
    public LayerMask platformLayer;
    private bool isGrounded;
    private bool onPlatform;
    [HideInInspector]
    //This tells the player where to teleport to when they land on a spike
    public Vector3 lastGroundedPosition;

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
    public float armFlipAnglePos = 90f;
    public float armFlipAngleNeg = -90f;
    private float idleTimer;
    Vector2 lastAimDirection = Vector2.right;
    float lastAngle = 0f;
    public bool flipArmLeft = true;
    public SpriteRenderer armRender;

    [Header("Stun-Ability Settings")]
    public float drainAmount;
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
    public bool phoneDropped = false;
    private int numOfJumps = 2;
    public int numOfLives = 3;
    public GameObject phonePrefab;
    //public GameObject phoneDropPoint;
    GameObject renameSpawnedPhone;

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

        SetRespawnPoint();

        cameraFollow = FindAnyObjectByType<CameraFollowDirection>();
        fallSpeedYDampingChangeThreshold = -15f;

        enemySpawnerManager.SpawnEnemies();
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
            attackPosition.transform.position = attackPointA.transform.position;

            cameraFollow.CallTurn(false);
        }
        else if (moveInput.x < 0)
        {
            anim.SetInteger("WalkX", -1);
            spriteRenderer.flipX = true;
            arm.position = aimRight.position;
            attackPosition.transform.position = attackPointB.transform.position;

            cameraFollow.CallTurn(true);
        }
        else if (moveInput.x == 0)
        {
            anim.SetInteger("WalkX", 0);
        }

        if (attackAction.WasPressedThisFrame() && hasMetalPipe)
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
            EnemyKnockbackable applyKnockback = enemyGameObject.GetComponent<EnemyKnockbackable>();
            if (applyKnockback != null)
            {
                applyKnockback.ApplyKnockback(this.transform, knockBackAmount);
            }
        }
    }
    public void EndAttack()
    {
        anim.SetBool("IsAttacking", false);
    }
    public void EndCharging()
    {
        safeStations.StopCharging();
        EnableArmRender();
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackPosition.transform.position, attackRadius);

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
        manager.ReduceBattery(drainAmount);
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
    }

    public void Die()
    {
        isDead = true;
        collision.enabled = false;
        DisableArmRender();
        anim.SetBool("IsDead", true);

        if (phoneDropped == false) 
        {
            renameSpawnedPhone = Instantiate(phonePrefab, lastGroundedPosition, Quaternion.identity);
            renameSpawnedPhone.name = phonePrefab.name;
            phoneDropped = true;
        }

        numOfLives = numOfLives - 1;

        StartCoroutine(HandleDeathFadeOut());

        if (numOfLives == 0)
        {
            SceneManager.LoadScene(1);
        }

        LighterMode();
    }

    public void LighterMode() 
    {
        //flashLight.intensity = 0;
        flashLight.gameObject.SetActive(false);
        lighterLight.intensity = 1;
        hasPhone = false;
    }

    public void pickUpPhone()
    {
        //flashLight.intensity = 1;
        flashLight.gameObject.SetActive(true);
        lighterLight.intensity = 0;
        numOfLives = 3;
        hasPhone = true;
        phoneDropped = false;
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

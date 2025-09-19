using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;

    [Header("Movement")]
    public float moveSpeed = 5f;
    private Vector2 moveInput;
    private Vector2 velocity;
    public float jumpForce = 10f;
    public float gravityScale = 3f;
    public float terminalVelocity = -15f;

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
    public GameObject stunEffect;
    public Transform cameraFlash;
    public Transform flashLeft;
    public Transform flashRight;
    private bool canFlash = true;
    public SpriteRenderer effectRender;

    [Header("Input Actions")]
    public InputActionAsset inputActions;
    private InputAction moveAction;
    private InputAction jumpAction;
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
        aimAction = playerActions.FindAction("AimDirection");
        flashAction = playerActions.FindAction("ActionFlash");
    }

    void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        aimAction.Enable();
        flashAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        aimAction.Disable();
        flashAction.Disable();
    }

    void Update()
    {
        CheckInput();
        AimingDirection();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void CheckInput()
    {
        moveInput = moveAction.ReadValue<Vector2>();

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
                Debug.Log(numOfJumps);
            }
            if (jumpAction.WasPressedThisFrame() && !isGrounded && numOfJumps != 0)
            {
                velocity.y = jumpForce / 1.4f;
                numOfJumps = 0;
                Debug.Log(numOfJumps);
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
        }
        else if (moveInput.x < 0)
        {
            spriteRenderer.flipX = true;
            arm.position = aimRight.position;
        }
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

        if (flashAction.WasPressedThisFrame() && canFlash)
        {
            ActivateFlash();
        }
    }

    void ActivateFlash()
    {
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
}

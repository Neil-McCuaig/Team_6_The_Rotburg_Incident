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
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;
    private bool isGrounded;

    [Header("Arm Aiming")]
    public Transform arm;
    public Transform aimLeft;
    public Transform aimRight;

    [Header("Input Actions")]
    public InputAction moveAction;
    public InputAction jumpAction;

    void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
    }

    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();

        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Jump
        if (jumpAction.WasPressedThisFrame() && isGrounded)
        {
            velocity.y = jumpForce;
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

    void FixedUpdate()
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
}

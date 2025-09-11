using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;

    [Header("Custom Gravity")]
    public float gravityScale = 4f;       // Custom gravity strength
    public float terminalVelocity = -20f; // Max fall speed

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private bool isGrounded;
    private float moveInput;
    private Vector2 velocity;

    public Transform arm;
    public Transform aimLeft;
    public Transform aimRight;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // Turn off built-in gravity
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = jumpForce;
        }

        if (moveInput > 0)
        {
            spriteRenderer.flipX = false;
            arm.position = aimLeft.position;
        }
        else if (moveInput < 0)
        {
            spriteRenderer.flipX = true;
            arm.position = aimRight.position;
        }
    }

    void FixedUpdate()
    {
        // Apply horizontal movement
        velocity.x = moveInput * moveSpeed;

        // Apply custom gravity
        if (!isGrounded || velocity.y > 0)
        {
            velocity.y += Physics2D.gravity.y * gravityScale * Time.fixedDeltaTime;
            velocity.y = Mathf.Max(velocity.y, terminalVelocity); // Clamp to terminal velocity
        }

        // Apply velocity
        rb.velocity = velocity;
    }
}

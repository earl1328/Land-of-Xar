using UnityEngine;
using System.Collections;

public class PlayerMovement2D : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Jump")]
    public float jumpForce = 7f;
    public int maxJumps = 2;

    [Header("Dash")]
    public float dashVelocity = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    private Rigidbody2D rb;
    private Animator anim;

    private float moveInput;
    private int jumpCount = 0;

    private bool isDashing = false;
    private bool canDash = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        HandleInput();
        HandleJump();
        HandleDash();
        HandleAnimation();
        HandleFlip();
    }

    void FixedUpdate()
    {
        if (!isDashing)
        {
            HandleMovement();
        }
    }

    // -------------------------
    // Input
    // -------------------------
    void HandleInput()
    {
        moveInput = Input.GetAxis("Horizontal");
    }

    // -------------------------
    // Movement
    // -------------------------
    void HandleMovement()
    {
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    // -------------------------
    // Jump
    // -------------------------
    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount++;

            if (jumpCount == 1)
                anim.SetTrigger("Jump");
            else
                anim.SetTrigger("DoubleJump");
        }
    }

    // -------------------------
    // Dash
    // -------------------------
    void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        float dashDirection = transform.localScale.x;

        rb.velocity = new Vector2(dashDirection * dashVelocity, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }

    // -------------------------
    // Animation
    // -------------------------
    void HandleAnimation()
    {
        bool isWalking = moveInput != 0;
        anim.SetBool("isWalking", isWalking);

        bool isFalling = rb.velocity.y < -0.1f;
        anim.SetBool("isFalling", isFalling);

        if (Mathf.Abs(rb.velocity.y) < 0.05f)
        {
            jumpCount = 0;
        }
    }

    // -------------------------
    // Flip
    // -------------------------
    void HandleFlip()
    {
        if (moveInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }
}

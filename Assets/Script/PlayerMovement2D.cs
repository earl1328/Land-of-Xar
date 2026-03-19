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

    private Rigidbody2D rb;
    private Animator anim;

    private float moveInput;
    private int jumpCount = 0;

    private bool isDashing = false;
    private bool canDash = true;

    // ✅ Flip control
    private bool facingRight = true;

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
    void HandleInput()
    {
        moveInput = Input.GetAxis("Horizontal");
    }

    void HandleMovement()
    {
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

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
    void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isDashing)
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
    }

    // -------------------------
    void HandleAnimation()
    {
        anim.SetBool("isWalking", Mathf.Abs(moveInput) > 0.01f);
        anim.SetBool("isFalling", rb.velocity.y < -0.1f);
    }

    // -------------------------
    // ✅ FIXED FLIP SYSTEM
    // -------------------------
    void HandleFlip()
    {
        if (moveInput > 0 && !facingRight)
        {
            Flip();
        }
        else if (moveInput < 0 && facingRight)
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

    // -------------------------
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts[0].normal.y > 0.5f)
        {
            jumpCount = 0;
            canDash = true;
        }
    }
}

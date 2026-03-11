using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Jump")]
    public float jumpForce = 7f;
    public int maxJumps = 2;

    private Rigidbody2D rb;
    private Animator anim;

    private float moveInput;
    private int jumpCount = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        HandleInput();
        HandleJump();
        HandleAnimation();
        HandleFlip();
    }

    void FixedUpdate()
    {
        HandleMovement();
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
    // Jump / Double Jump
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
    // Animation
    // -------------------------
    void HandleAnimation()
    {
        bool isWalking = moveInput != 0;
        anim.SetBool("isWalking", isWalking);

        bool isFalling = rb.velocity.y < -0.1f;
        anim.SetBool("isFalling", isFalling);

        // safer jump reset (instead of velocity == 0)
        if (Mathf.Abs(rb.velocity.y) < 0.05f)
        {
            jumpCount = 0;
        }
    }

    // -------------------------
    // Flip Player
    // -------------------------
    void HandleFlip()
    {
        if (moveInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }
}

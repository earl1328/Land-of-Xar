using UnityEngine;
using VirController;

[RequireComponent(typeof(Animator))]
public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;
    private PlayerController controller;
    private Rigidbody2D rb;

    private bool isGrounded;
    private int jumpCount;

    void Awake()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        controller.GroundedChanged += OnGroundedChanged;
        controller.Jumped += OnJumped;
    }

    void OnDisable()
    {
        controller.GroundedChanged -= OnGroundedChanged;
        controller.Jumped -= OnJumped;
    }

    void Update()
    {
        HandleRun();
        HandleFall();
        Flip();
    }

    void HandleRun()
    {
        float moveX = controller.FrameInput.x;
        bool isRunning = Mathf.Abs(moveX) > 0.1f;

        anim.SetBool("isRunning", isRunning);
    }

    void HandleFall()
    {
        bool isFalling = rb.velocity.y < -0.1f && !isGrounded;
        anim.SetBool("isFalling", isFalling);
    }

    void Flip()
    {
        float moveX = controller.FrameInput.x;

        if (moveX != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(moveX), 1, 1);
        }
    }

    void OnGroundedChanged(bool grounded, float velocity)
    {
        isGrounded = grounded;

        if (grounded)
        {
            jumpCount = 0;
        }
    }

    void OnJumped()
    {
        jumpCount++;

       
        anim.ResetTrigger("Jump");
       

        if (jumpCount == 1)
        {
            anim.SetTrigger("Jump");
        }
        else if (jumpCount == 2)
        {
            anim.SetTrigger("DoubleJump");
        }
    }

}

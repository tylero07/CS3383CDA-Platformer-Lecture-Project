using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float dashSpeed = 18f;
    [SerializeField] private float dashDuration = 0.18f;
    [SerializeField] private float dashCooldown = 0.35f;
    [SerializeField] private bool isGrounded;
    [SerializeField] private float minGroundNormalY = 0.7f;

    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer playerSprite;

    private Rigidbody2D rb;
    private float moveInput;
    private float facingDirection = 1f;
    private float dashTimer;
    private float dashCooldownTimer;
    private bool isDashing;
    private string currentAnimation;
    private readonly ContactPoint2D[] contacts = new ContactPoint2D[16];
    private int jumpCount = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (playerSprite == null)
            playerSprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            rb.linearVelocity = new Vector2(
            facingDirection * dashSpeed,
            rb.linearVelocity.y
        );
        return;
        }
        rb.linearVelocity = new Vector2(
            moveInput * moveSpeed,
            rb.linearVelocity.y
        );
    }

    private void Update()
    {
        CheckGrounded();

        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
            }
        }

        // Assumes the original sprite faces right.
        if (playerSprite != null && Mathf.Abs(moveInput) > 0.01f)
            playerSprite.flipX = moveInput < 0f;

        if (!isGrounded)
        {
            PlayAnimation(rb.linearVelocity.y > 0.01f ? "Jump" : "Fall");
        }
        else
        {
            PlayAnimation(Mathf.Abs(moveInput) > 0.01f ? "Run" : "Idle");
        }
    }

    private void CheckGrounded()
    {
        isGrounded = false;

        // Ignore lingering ground contacts immediately after jumping.
        if (rb.linearVelocity.y > 0.1f)
            return;

        int contactCount = rb.GetContacts(contacts);

        for (int i = 0; i < contactCount; i++)
        {
            if (contacts[i].normal.y >= minGroundNormalY)
            {
                isGrounded = true;
                jumpCount = 0;
                return;
            }
        }
    }

    private void PlayAnimation(string animationName)
    {
        if (animator == null || animator.runtimeAnimatorController == null)
            return;

        // Only restart the animation when the state changes.
        if (currentAnimation == animationName)
            return;

        animator.Play("Base Layer." + animationName, 0, 0f);
        currentAnimation = animationName;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>().x;
    }

    public void OnDash(InputValue value)
    {
        if (!value.isPressed || dashCooldownTimer > 0f || isDashing)
        {
            return;
        }

        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;

        if (Mathf.Abs(moveInput) > 0.01f)
        {
            facingDirection = moveInput < 0f ? -1f : 1f;
        }
        rb.linearVelocity = new Vector2(facingDirection * dashSpeed, rb.linearVelocity.y);
    }

    public void OnJump(InputValue value)
    {
        CheckGrounded();

        if (value.isPressed && isGrounded)
        {
            Jump(rb.linearVelocity.x);
        }
        else if (value.isPressed && jumpCount < 2)
        {
            Jump(rb.linearVelocity.x * 0.5f, true);
        }
    }

    private void Jump(float height, bool grounded = false)
    {
        rb.linearVelocity = new Vector2(height, jumpForce);
        isGrounded = grounded;
        jumpCount++;
    }
}
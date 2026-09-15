using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private bool isGrounded;
    [SerializeField] private float minGroundNormalY = 0.7f;

    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer playerSprite;

    private Rigidbody2D rb;
    private float moveInput;
    private string currentAnimation;
    private readonly ContactPoint2D[] contacts = new ContactPoint2D[16];

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
        rb.linearVelocity = new Vector2(
            moveInput * moveSpeed,
            rb.linearVelocity.y
        );
    }

    private void Update()
    {
        CheckGrounded();

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

    public void OnJump(InputValue value)
    {
        CheckGrounded();

        if (value.isPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );

            isGrounded = false;
        }
    }
}
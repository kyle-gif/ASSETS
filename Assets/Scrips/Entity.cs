using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    protected bool IsGrounded;
    protected bool IsFalling = true;
    protected Rigidbody2D Rb;
    protected SpriteRenderer SpriteRenderer;
    public const float FallingThreshold = -0.5f;
    protected bool IsFlipped;  // Moved from Boss class

    protected virtual void Start()
    {
        Rb = GetComponent<Rigidbody2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Move
    protected void Move(float moveInput, float moveSpeed)
    {
        Rb.velocity = new Vector2(moveInput * moveSpeed, Rb.velocity.y);
    }

    // Flip
    public virtual void FlipDir(float moveInput)
    {
        if ((moveInput > 0 && IsFlipped) || (moveInput < 0 && !IsFlipped))
        {
            Vector3 flipped = transform.localScale;
            flipped.x *= -1f;
            transform.localScale = flipped;
            IsFlipped = !IsFlipped;
        }
    }

    // Jump
    protected void Jump(float jumpForce)
    {
        Rb.velocity = new Vector2(Rb.velocity.x, jumpForce);
    }

    // Collision detection
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            IsGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            IsGrounded = false;
        }
    }
    
    // Check if falling
    protected void SetFalling()
    {
        IsFalling = Rb.velocity.y < FallingThreshold;
    }
}
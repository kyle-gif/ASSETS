using System.Collections;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    protected bool IsGrounded;
    protected bool IsFalling = true;
    protected Rigidbody2D Rb;
    protected SpriteRenderer SpriteRenderer;
    public const float FallingThreshold = -0.5f;
    protected bool isFlipped;  // Moved from Boss class
    
    // Move
    protected void Move(float moveInput, float moveSpeed)
    {
        Rb.velocity = new Vector2(moveInput * moveSpeed, Rb.velocity.y);
    }

    // Flip
    public virtual void FlipDir(float moveInput)
    {
        if ((moveInput > 0 && isFlipped) || (moveInput < 0 && !isFlipped))
        {
            Vector3 flipped = transform.localScale;
            flipped.x *= -1f; // Flipping along the x-axis for 2D game
            transform.localScale = flipped;
            isFlipped = !isFlipped;
        }
    }

    // Jump
    protected void Jump(float jumpForce)
    {
        Rb.velocity = new Vector2(Rb.velocity.x, jumpForce);
    }

    // Is touching ground
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            IsGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            IsGrounded = false;
        }
    }
    
    // Is falling
    protected void SetFalling()
    {
        if (Rb.velocity.y < FallingThreshold)
        {
            IsFalling = true;
        }
        else
        {
            IsFalling = false;
        }
    }
    
}
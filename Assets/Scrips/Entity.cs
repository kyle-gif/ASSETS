using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    protected bool IsGrounded;
    protected bool IsFalling = false;
    protected Rigidbody2D Rb;
    protected SpriteRenderer SpriteRenderer;
    public const float FallingThreshold = -0.5f;
    private bool isFlipped;
    
    // Move
    protected void Move(float moveInput, float moveSpeed)
    {
        Rb.velocity = new Vector2(moveInput * moveSpeed, Rb.velocity.y);
    }

    // Flip
    public virtual void FlipDir(float moveInput)
    {
        Vector3 flipped = transform.localScale;
        flipped.z *= -1f;
        
        if (moveInput > 0 && isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = false;
        }
        else if (moveInput < 0 && !isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = true;
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
    
    //Is falling
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

    public virtual float Hit(float hp, float damage)
    {
        return hp -= damage;
    }

    void Start()
    {
    }
}
    
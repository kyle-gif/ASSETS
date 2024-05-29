using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    protected bool IsGrounded;
    protected bool IsFalling = false;
    protected Rigidbody2D Rb;
    protected SpriteRenderer SpriteRenderer;
    public const float FallingThreshold = -0.5f;
    // Move
    protected void Move(float moveInput, float moveSpeed)
    {
        Rb.velocity = new Vector2(moveInput * moveSpeed, Rb.velocity.y);
    }

    // Flip
    protected void FlipDir(float moveInput)
    {
        if (moveInput > 0)
        {
            SpriteRenderer.flipX = false;
        }
        else if (moveInput < 0)
        {
            SpriteRenderer.flipX = true;
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

    void Start()
    {
    }
}
    
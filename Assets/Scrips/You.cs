using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class You : Entity
{
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float jumpForce = 5.0f;
    private Animator animator;

    void PlayerPrimaryAttack()
    {
        
    }
    
    void Start()
    {
        Rb = GetComponent<Rigidbody2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        SetFalling();
        //Move
        float moveInput = Input.GetAxis("Horizontal");
        
        base.Move(moveInput, moveSpeed);
        animator.SetFloat("Velocity", Mathf.Abs(Rb.velocity.x));
        /*Debug.Log(Rb.velocity.x);*/
        FlipDir(moveInput);
        
        //Jump
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded)
        {
            animator.SetBool("IsJumping", true);
            Debug.Log("True");
            base.Jump(jumpForce);
        }
        else
        {
            animator.SetBool("IsJumping", false);
        }
        
        //Attack
        if (Input.GetKeyDown(KeyCode.K))
        {
            PlayerPrimaryAttack();
        }

        if (IsFalling)
        {
            animator.SetBool("IsFalling", true);
        }
        else
        {
            animator.SetBool("IsFalling", false);
        }
        
        Debug.Log("Hello World!");
    }
}
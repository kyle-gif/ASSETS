using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class You : Entity
{
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float jumpForce = 5.0f;
    private int AttackStack = 0;
    private float TimeToAttack = 0;
    private const int MaxAttackCount = 2;
    private bool IsAttacking;
    private Animator animator;
    private float Hp;

    void PlayerPrimaryAttack()
    {
        animator.SetTrigger("Attack");
    }
    void FreezeCharacter()
    {
        Rb.velocity = Vector2.zero;
        Rb.gravityScale = 0;
        Rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
    }

    void UnfreezeCharacter()
    {
        Rb.gravityScale = 1;
        Rb.constraints = RigidbodyConstraints2D.FreezeRotation; // or RigidbodyConstraints2D.None if you don't want to constrain rotation
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
        if(Mathf.Abs(Rb.velocity.x) > Mathf.Epsilon) animator.SetInteger("AnimState", 1);
        else animator.SetInteger("AnimState", 0);
        FlipDir(moveInput);
        
        //Jump
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded)
        {
            animator.SetBool("IsJumping", true);
            base.Jump(jumpForce);
        }
        else animator.SetBool("IsJumping", false);
        
        //Attack
        if (Input.GetKeyDown(KeyCode.K))
        {
            IsAttacking = true;
            PlayerPrimaryAttack();
            Debug.Log(AttackStack);
            animator.SetInteger("AttackStack", AttackStack);
            AttackStack++;
        }
        if (TimeToAttack > 1.5f)
        {
            TimeToAttack = 0.0f;
            AttackStack = 0;
        }
        if (AttackStack >= MaxAttackCount) AttackStack = 0;
        if (IsAttacking) FreezeCharacter();
        else UnfreezeCharacter();
        
        TimeToAttack += Time.deltaTime;
        animator.SetFloat("Till", TimeToAttack);
        
        if (IsGrounded) animator.SetBool("IsGrounded", false);
        else animator.SetBool("IsGrounded", true);
        
        if (IsFalling) animator.SetBool("IsFalling", true);
        else animator.SetBool("IsFalling", false);
    }
}
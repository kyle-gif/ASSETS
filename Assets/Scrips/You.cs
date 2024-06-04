using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class You : Entity
{
    public Transform pos;
    public Vector2 boxSize;
    private Animator m_animator;
    
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float jumpForce = 6.0f;
    [SerializeField] private float hp;
    [SerializeField] private float damage;
    
    private const int MaxAttackCount = 2;
    private const float MaxHp = 100.0f;
    private const float AtkCoolTime = 0.3f;
    
    private int m_attackStack = 0;
    private float m_timeToAttack = 0;
    private float m_curAtkTime;
    private bool m_isAttacking;
    private bool m_isJumping;
    

    void PlayerPrimaryAttack()
    {
        m_animator.SetTrigger("Attack");
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

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(pos.position, boxSize);
    }

    void Start()
    {
        Rb = GetComponent<Rigidbody2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        m_animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        Rb.velocity =Vector3.zero;
        SetFalling();
        //Move
        float moveInput = Input.GetAxis("Horizontal");
        base.Move(moveInput, moveSpeed);
        if(Mathf.Abs(Rb.velocity.x) > Mathf.Epsilon) 
            m_animator.SetInteger("AnimState", 1);
        else 
            m_animator.SetInteger("AnimState", 0);
        FlipDir(Rb.velocity.x);
        
        //Jump
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded)
        {
            m_animator.SetBool("IsJumping", true);
            m_isJumping = true;
            base.Jump(jumpForce);
        }
        else
        {
            m_animator.SetBool("IsJumping", false);
            m_isJumping = false;
        }

        //Deciding State
        if (IsGrounded) 
            m_animator.SetBool("IsGrounded", true);
        else 
            m_animator.SetBool("IsGrounded", false);
        
        if (IsFalling) 
            m_animator.SetBool("IsFalling", true);
        else 
            m_animator.SetBool("IsFalling", false);
        
        //Attack
        if (m_attackStack > MaxAttackCount - 1)
        {
            m_attackStack = 0;
            m_timeToAttack = 0.0f;
        }
        if (m_curAtkTime <= 0)
        {
            if (Input.GetKeyDown(KeyCode.K) && IsGrounded) 
            {
                /*Debug.Log(AttackStack);*/
                m_isAttacking = true;
                PlayerPrimaryAttack();
                m_animator.SetTrigger("Attack");
                m_animator.SetInteger("AttackStack", m_attackStack);
                Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(pos.position, boxSize, 0);
                foreach (Collider2D collider in collider2Ds)
                {
                    if (collider.tag == "Enemy")
                    {
                        
                    }
                }
                m_attackStack++;
                m_isAttacking = false;
                m_curAtkTime = AtkCoolTime;
            }
        }
        else
        {
            m_curAtkTime -= Time.deltaTime;
        }
        if (m_timeToAttack > 1.0f)
        {
            m_timeToAttack = 0.0f;
            m_attackStack = 0;
        }

        if (IsFalling || m_isJumping)
        {
            m_attackStack = 0;
            m_timeToAttack = 0.0f;
        }

        if (m_isAttacking) 
            FreezeCharacter();
        else 
            UnfreezeCharacter();
        
        m_timeToAttack += Time.deltaTime;
        m_animator.SetFloat("Till", m_timeToAttack);
    }
}
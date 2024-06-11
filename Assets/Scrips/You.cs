using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class You : Entity
{
    public Transform pos;
    public Vector2 boxSize;
    private Animator m_animator;
    
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private const float MaxHp = 100.0f;
    [SerializeField] private float jumpForce = 6.0f;
    [SerializeField] private float damage = MaxDamage;
    [SerializeField] private const float Dodgetime = 0.5f;
    
    private const int MaxAttackCount = 2;
    private const float AtkCoolTime = 0.3f;
    private const float MaxDamage = 20.0f;
    
    private int m_attackStack = 0;
    private float m_timeToAttack = 0;
    private float m_curAtkTime;
    private float m_enemyHP;
    private bool m_isAttacking;
    private bool m_isJumping;
    private bool m_isDodging;
    
    public float Hp = MaxHp;
    public HealthUI_TSET healthBar;

    private IEnumerator DeathDelay()
    {
        //yield return new WaitUntil(() => !m_animator.GetCurrentAnimatorStateInfo(0).IsName("Main_Death"));
        yield return new WaitForSeconds(0.0f);
        SceneManager.LoadScene("GamE_Oveer");
    }
    
    
    
    private void Death()
    {
        m_animator.SetTrigger("Death");
        StartCoroutine(DeathDelay());
    }
    
    private IEnumerator Delay(float delaytime)
    {
        // Wait for the attack animation to complete
        //yield return new WaitUntil(() => !m_animator.GetCurrentAnimatorStateInfo(0).IsName("Main_Attack1") && !m_animator.GetCurrentAnimatorStateInfo(0).IsName("Main_Attack2"));

        yield return new WaitForSeconds(delaytime);
    }
    
    void PlayerPrimaryAttack()
    {
        m_isAttacking = true;
        m_animator.SetTrigger("Attack");
        StartCoroutine(Delay(0.5f));
        m_isAttacking = false;
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(pos.position, boxSize, 0);
        foreach (Collider2D collider in collider2Ds)
        {
            if (collider.tag == "Enemy")
            {
                collider.GetComponent<Boss>().Hit(damage);
            }
        }
        m_animator.SetInteger("AttackStack", m_attackStack);
        m_attackStack++;
        m_curAtkTime = AtkCoolTime;
    }
    
    public void Hit(float enemyDamage)
    {
        if(!m_isDodging)
        {
            Hp -= enemyDamage;
        }
        healthBar.SetHealth(Hp);
    }

    private void Dodge()
    {
        m_isDodging = true;
        m_animator.SetTrigger("dodge");
        StartCoroutine(Delay(0.5f));
        m_isDodging = false;
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
        Rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    
    // Draw Attack Range
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(pos.position, boxSize);
    }

    void Start()
    {
        healthBar.SetMaxHealth(MaxHp);
        Rb = GetComponent<Rigidbody2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        m_animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            Dodge();
        }
        if (Hp <= 0)
        {
            Death();
        }
        SetFalling();
        // Move
        float moveInput = Input.GetAxis("Horizontal");
        base.Move(moveInput, moveSpeed);
        if (Mathf.Abs(Rb.velocity.x) > Mathf.Epsilon) 
            m_animator.SetInteger("AnimState", 1);
        else 
            m_animator.SetInteger("AnimState", 0);
        FlipDir(Rb.velocity.x);
        
        // Jump
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

        // Deciding State
        if (IsGrounded) 
            m_animator.SetBool("IsGrounded", true);
        else 
            m_animator.SetBool("IsGrounded", false);
        
        if (IsFalling) 
            m_animator.SetBool("IsFalling", true);
        else 
            m_animator.SetBool("IsFalling", false);
        
        // Attack
        if (m_attackStack > MaxAttackCount - 1)
        {
            m_attackStack = 0;
            m_timeToAttack = 0.0f;
        }
        if (m_curAtkTime <= 0)
        {
            if (Input.GetKeyDown(KeyCode.K) && IsGrounded && !m_isAttacking) 
            {
                PlayerPrimaryAttack();
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

        m_timeToAttack += Time.deltaTime;
        m_animator.SetFloat("Till", m_timeToAttack);
    }
}

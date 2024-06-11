using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class You : Entity
{
    public Transform pos;
    public Vector2 boxSize;
    public HealthUI_TSET healthBar;

    private Animator animator;

    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private const float MaxHp = 100.0f;
    [SerializeField] private float jumpForce = 6.0f;
    [SerializeField] private const float DodgeTime = 0.2f;
    
    private const int MaxAttackCount = 2;
    private const float AtkCoolTime = 0.3f;
    private const float MaxDamage = 20.0f;
    private const float AttackDelay = 0.25f; // Delay before attack hit detection

    private int attackStack = 0;
    private float curAtkTime;
    private float timeToAttack;
    private bool isAttacking;
    private bool isDodging;

    public float Hp = MaxHp;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        healthBar.SetMaxHealth(MaxHp);
    }

    private void Update()
    {
        HandleInput();
        UpdateAnimatorStates();
    }

    private void HandleInput()
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

        float moveInput = Input.GetAxis("Horizontal");
        Move(moveInput, moveSpeed);

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded)
        {
            Jump(jumpForce);
        }

        if (curAtkTime <= 0 && Input.GetKeyDown(KeyCode.K) && IsGrounded && !isAttacking)
        {
            StartCoroutine(PlayerPrimaryAttack());
        }
        
        curAtkTime -= Time.deltaTime;
        timeToAttack += Time.deltaTime;
    }

    private void UpdateAnimatorStates()
    {
        animator.SetInteger("AnimState", Mathf.Abs(Rb.velocity.x) > Mathf.Epsilon ? 1 : 0);
        animator.SetBool("IsJumping", !IsGrounded);
        animator.SetBool("IsGrounded", IsGrounded);
        animator.SetBool("IsFalling", IsFalling);
        
        FlipDir(Rb.velocity.x);

        if (IsFalling || !IsGrounded)
        {
            attackStack = 0;
            timeToAttack = 0.0f;
        }
        
        if (timeToAttack > 1.0f)
        {
            attackStack = 0;
            timeToAttack = 0.0f;
        }

        animator.SetFloat("Till", timeToAttack);
    }

    private IEnumerator PlayerPrimaryAttack()
    {
        if (attackStack >= MaxAttackCount)
        {
            attackStack %= 2;
            timeToAttack = 0.0f;
        }
        isAttacking = true;
        animator.SetTrigger("Attack");
        animator.SetInteger("AttackStack", attackStack);
        Debug.Log(attackStack);
        
        yield return new WaitForSeconds(AttackDelay);

        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(pos.position, boxSize, 0);
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                enemy.GetComponent<Boss>().Hit(MaxDamage);
            }
        }
        curAtkTime = AtkCoolTime;
        attackStack++;

        yield return new WaitForSeconds(0.4f - AttackDelay); // Remaining animation time
        isAttacking = false;
    }

    public void Hit(float enemyDamage)
    {
        if (!isDodging)
        {
            Hp -= enemyDamage;
            healthBar.SetHealth(Hp);
            animator.SetTrigger("Hit");
        }
    }

    private void Dodge()
    {
        if (isDodging) return; // Prevent dodging if already dodging
        isDodging = true;
        animator.SetTrigger("Dodge");
        StartCoroutine(ResetDodgeState(DodgeTime));
    }

    private IEnumerator ResetDodgeState(float delay)
    {
        yield return new WaitForSeconds(delay);
        isDodging = false;
    }

    private void Death()
    {
        animator.SetTrigger("Death");
        StartCoroutine(DeathDelay());
    }

    private IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(3.0f); // Adjust delay as needed
        SceneManager.LoadScene("GamE_Oveer");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(pos.position, boxSize);
    }
}

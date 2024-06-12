using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Boss : Entity
{
    public const float MaxHp = 100.0f;
    public const float MaxDamage = 20.0f;
    public const float AttackRange = 2.0f;
    public const float AttackWait = 0.52f;
    public const float MaxSpeed = 3.0f;

    public float Hp = MaxHp;
    public Transform player;
    public Transform pos;
    public Vector2 boxSize;
    public HealthUI_TSET healthBar;

    private Animator animator;
    private bool isAttacking;

    private void ChasePlayer()
    {
        if (isAttacking) return;

        float direction = player.position.x - Rb.position.x;
        FlipDir(direction * -1);
        animator.SetInteger("AnimState", Mathf.Abs(Rb.velocity.x) > 0 ? 1 : 0);
        Debug.Log(Mathf.Abs(player.position.x - Rb.position.x));
        if (Mathf.Abs(direction) > AttackRange)
        {
            Move(direction > 0 ? 1 : -1, MaxSpeed);
        }
        else
        {
            Move(0, 0);
        }
    }

    private void AttackPlayer()
    {
        if (!isAttacking && Vector2.Distance(player.position, Rb.position) <= AttackRange)
        {
            isAttacking = true;
            animator.SetTrigger("Attack1");
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        Rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(AttackWait);

        Collider2D[] hitPlayers = Physics2D.OverlapBoxAll(pos.position, boxSize, 0);
        foreach (Collider2D player in hitPlayers)
        {
            if (player.CompareTag("Player"))
            {
                player.GetComponent<You>().Hit(MaxDamage);
            }
        }

        isAttacking = false;
    }

    public void Hit(float damage)
    {
        Hp -= damage;
        healthBar.SetHealth(Hp);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(pos.position, boxSize);
    }
    private void Death()
    {
        animator.SetTrigger("Death");
        StartCoroutine(DeathDelay());
    }
    private IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(0.0f); // Adjust delay as needed
        SceneManager.LoadScene("Game_win");
    }
    
    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        healthBar.SetMaxHealth(MaxHp);
    }

    private void Update()
    {
        ChasePlayer();
        AttackPlayer();
        if (Hp <= 0)
        {
            Death();
        }
    }
    
}

using System.Collections;
using UnityEngine;

public class Boss : Entity
{
    public const float maxHP = 100.0f;
    public float Hp = maxHP;

    [SerializeField] private const float maxDamage = 5.0f;
    public float damage = maxDamage;
    
    public Transform player;
    private Animator animator;
    public float speed = 3.0f;
    public const float maxSpeed = 3.0f;
    public const float attackRange = 2.0f;
    public const float AttackWait = 0.6f;
    public Transform pos;
    public Vector2 boxSize;
    private float m_attackStack = 0;
    public HealthUI_TSET healthBar;
    

    public bool isAttacking = false;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(pos.position, boxSize);
    }

    public override void FlipDir(float xVel)
    {
        if ((xVel > 0 && !isFlipped) || (xVel < 0 && isFlipped))
        {
            Vector3 flipped = transform.localScale;
            flipped.x *= -1f; // Flipping along the x-axis for 2D game
            transform.localScale = flipped;
            isFlipped = !isFlipped;
        }
    }
    
    public void Hit(float damage)
    {
        Hp -= damage;
        healthBar.SetHealth(Hp);
    }
    
    private IEnumerator AttackDelay()
    {
        // Stop movement
        Rb.velocity = Vector2.zero;

        // Wait for the attack duration
        yield return new WaitForSeconds(AttackWait);

        // Wait for the attack animation to complete
        yield return new WaitUntil(() => !animator.GetCurrentAnimatorStateInfo(0).IsName("HeroKnight_Attack1"));

        isAttacking = false;
    }

    private void LetsGo()
    {
        if (!isAttacking)
        {
            float direction = player.position.x - Rb.position.x;
            FlipDir(direction);
            AmIMoving();
            if (Mathf.Abs(direction) > attackRange)
            {
                Move(Mathf.Sign(direction), speed);
            }
            else
            {
                Move(0, 0);
            }
        }
    }

    private void Attack()
    {
        if (!isAttacking && Vector2.Distance(player.position, Rb.position) <= attackRange)
        {
            isAttacking = true;
            animator.SetTrigger("Attack1");
            StartCoroutine(AttackDelay());
            Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(pos.position, boxSize, 0);
            foreach (Collider2D collider in collider2Ds)
            {
                if(collider.tag == "Player")
                {
                    collider.GetComponent<You>().Hit(damage);
                }
            }
            m_attackStack++;
        }
    }

    void Start()
    {
        healthBar.SetMaxHealth(maxHP);
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void AmIMoving()
    {
        if (Mathf.Abs(Rb.velocity.x) > 0)
        {
            animator.SetInteger("AnimState", 1);
        }
        else
        {
            animator.SetInteger("AnimState", 0);
        }
    }

    void Update()
    {
        LetsGo();
        Attack();
    }
}

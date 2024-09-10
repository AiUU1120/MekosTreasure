using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_SlimeKing : MonoBehaviour
{
    public GameObject attackTarget;
    Rigidbody2D rb;
    Animator animator;
    Collider2D col;
    [SerializeField] float verticalSpeed;//垂直初速度
    [SerializeField] LayerMask ground;
    float horizontalSpeed;//水平初速度

    float t1;//子弹上升时间
    float t2;//子弹下落时间
    float t;
    float g = 9.81f;//重力加速度
    float G;//实际受到重力效果
    float s;//水平飞行距离
    float h1;//上升高度
    float h2;//下降高度

    int dierection;

    bool isOnGround;
    bool isDrop;
    [SerializeField] Vector3 colliderOffset;
    [SerializeField] float groundLength;
    [SerializeField] int minDamage;
    [SerializeField] int maxDamage;

    [Header("===== SFX =====")]
    [SerializeField]
    private AudioData slimeBallSfx;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }
    void Start()
    {
        if (attackTarget != null)
        {
            G = g * rb.gravityScale;
            h1 = Mathf.Pow(verticalSpeed, 2) / (2f * G);
            t1 = verticalSpeed / G;
            h2 = h1 + (transform.position.y - attackTarget.transform.position.y);
            if(h2 <= 0.1f)
            {
                h2 = 0.1f;
            }
            t2 = Mathf.Sqrt((2 * h2) / G);
            t = t1 + t2;
            s = attackTarget.transform.position.x - transform.position.x;
            dierection = s > 0 ? 1 : -1;
            horizontalSpeed = s / t;

            rb.AddForce(new Vector2(horizontalSpeed, verticalSpeed), ForceMode2D.Impulse);
        }
        if(attackTarget == null)
        {
            rb.AddForce(new Vector2(6f, verticalSpeed), ForceMode2D.Impulse);
        }
        AudioManager.Instance.PlaySFX(slimeBallSfx);
    }

    private void Update()
    {
        isOnGround = Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, ground) || Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, ground);
        if (isOnGround && !isDrop)
        {
            animator.SetTrigger("Drop");
            isDrop = true;
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + colliderOffset, transform.position + colliderOffset + Vector3.down * groundLength);//触地检测
        Gizmos.DrawLine(transform.position - colliderOffset, transform.position - colliderOffset + Vector3.down * groundLength);
    }

    public void OnDestroySlimeBall()
    {
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttacked"))
        {
            Debug.Log("SlimeBall Attack!");
            var targetStats = other.GetComponentInParent<CharacterStats>();
            targetStats.TakeDamage(minDamage, maxDamage, targetStats, dierection);
            animator.SetTrigger("Hit");
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum EnemyStates { GUARD, PATROL, CHASE, ATTACK, DEAD, ISHIT }
public class DogController : Enemy, IEndGameObserver
{
    #region DEFINITION
    public EnemyStates enemyStates;

    [Header("Basic Settings")]
    public float sightRadiusX;//可视范围/追击范围
    public float sightRadiusY;
    public float chaseSpeed;//移动（追击）速度
    private Vector3 guardPos;//初始坐标
    private Vector3 guardLocalScale;
    private GameObject attackTarget;//攻击目标（玩家）
    private bool isFoundPlayer;//判断是否已经看到玩家
    public bool isGuard;//是否为站桩敌人

    [Header("Guard State")]
    public float watchTime;//观察时间
    private float watchTimer;//计时器：计算观察剩余时间

    [Header("Patrol State")]
    public float patrolRange;//巡逻范围
    public float stopDistance;//目标点停止距离
    public float waitTime;//到达目的地后等待时间
    private float remainWaitTime;//计时器：用于判断离结束等待还有多久
    private float randomX;//目标点X坐标

    [Header("Chase State")]
    public float lastPos;//上次记录坐标X值
    private float recordTime = 0.5f;//记录坐标间隔时间
    private float recordTimer;
    private bool isTouchWall;
    private bool isTurn;

    [Header("ISHIT State")]
    public float hitTime;//受伤时间
    public float hurtColorTime;//受伤闪烁时间
    private float lastHitTime;//计时器：计算受伤时间


    private Animator animator;
    private Collider2D col_collision;
    private Collider2D col_dash;
    private AnimatorStateInfo info;
    public float infoPrecent;

    //配合动画
    bool isWalk;
    bool isChase;
    bool isFollow;

    bool isDying;

    //bool playerDead
    [Header("===== SFX =====")]
    [SerializeField]
    private AudioData dogAttackSfx;
    [SerializeField]
    private AudioData dogDeadSfx;
    #endregion


    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        col_collision = transform.Find("AttackedArea_Dog").GetComponent<Collider2D>();
        col_dash = transform.Find("DashAttackArea_Dog").GetComponent<Collider2D>();
        guardPos = transform.position;
        guardLocalScale = transform.localScale;
        remainWaitTime = waitTime;
        watchTimer = watchTime;
    }

    protected override void Start()
    {
        base.Start();
        GameManager.Instance.AddObserver(this);//HACK:做场景切换时修改
        if (isGuard)
        {
            enemyStates = EnemyStates.GUARD;
        }
        else
        {
            GetNewWayPoint();
            enemyStates = EnemyStates.PATROL;
        }
    }

    void OnEnable()
    {
        //GameManager.Instance.AddObserver(this);
    }
    void OnDisable()
    {
        if (!GameManager.IsInitialized)
        {
            return;
        }
        GameManager.Instance.RemoveObserver(this);
    }

    private void FixedUpdate()
    {
        if (enemyStates == EnemyStates.CHASE)
        {
            if (!isTouchWall)
            {
                Dash();
            }
        }
    }
    private void Update()
    {
        if (characterStats.CurHealth <= 0f)
        {
            isDead = true;
        }
        SwtichStates();
        info = animator.GetCurrentAnimatorStateInfo(1);
        infoPrecent = info.normalizedTime - (int)info.normalizedTime;
        SwitchAnimation();
    }
    void SwtichStates()
    {
        if (isDead)
        {
            if (characterStats.IsHited)//
            {
                lastHitTime = hitTime;
                characterStats.IsHited = false;//
                AudioManager.Instance.PlaySFX(dogDeadSfx);
            }
            enemyStates = EnemyStates.DEAD;
        }
        else if (characterStats.Hitting)
        {
            if (characterStats.IsHited)//
            {
                animator.SetTrigger("Hited");
                lastHitTime = hitTime;
                characterStats.IsHited = false;//
            }
            enemyStates = EnemyStates.ISHIT;
        }
        else if (FoundPlayer())
        {
            if (!isFoundPlayer)
            {
                AudioManager.Instance.PlaySFX(dogAttackSfx);
                FlipTo(attackTarget.transform);
            }
            isFoundPlayer = true;
            enemyStates = EnemyStates.CHASE;
        }

        switch (enemyStates)
        {
            case EnemyStates.GUARD:
                {
                    isChase = false;
                    isFollow = false;
                    rb.velocity = new Vector2(0, rb.velocity.y);
                    if (watchTimer < 0)
                    {
                        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                        watchTimer = watchTime;
                    }
                    else if (watchTimer >= 0)
                    {
                        watchTimer -= Time.deltaTime;
                    }
                    break;
                }
            case EnemyStates.PATROL:
                {
                    break;
                }
            case EnemyStates.CHASE:
                {
                    isWalk = false;
                    col_dash.enabled = true;
                    if (!isTouchWall)
                    {
                        isChase = true;
                        isFollow = true;
                    }
                    if (recordTimer > 0)
                    {
                        recordTimer -= Time.deltaTime;
                    }
                    else if (recordTimer <= 0)
                    {
                        if (lastPos == transform.position.x)//判断撞墙
                        {
                            //Debug.Log("IS TOUCH WALL!");
                            if (!isTouchWall)
                            {
                                remainWaitTime = waitTime;
                            }
                            isTouchWall = true;
                        }
                        else if (lastPos != transform.position.x)
                        {
                            lastPos = transform.position.x;
                        }
                        recordTimer = recordTime;
                    }

                    if (isTouchWall)
                    {
                        isChase = false;
                        isFollow = false;
                        remainWaitTime -= Time.deltaTime;
                        rb.velocity = new Vector2(0, rb.velocity.y);
                        if (remainWaitTime <= 0 && !isTurn)
                        {
                            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                            isTurn = true;
                            remainWaitTime = waitTime;
                        }
                        else if (remainWaitTime <= 0 && isTurn)
                        {
                            isTouchWall = false;
                            isTurn = false;
                        }
                    }
                    break;
                }
            case EnemyStates.ATTACK:
                {
                    break;
                }
            case EnemyStates.DEAD:
                {
                    lastHitTime -= Time.deltaTime;
                    if (lastHitTime >= (hitTime - hurtColorTime))//颜色变化时间
                    {
                        rbSprite.material.SetFloat("_FlashAmount", 1);
                    }
                    else if (lastHitTime < (hitTime - hurtColorTime))
                    {
                        rbSprite.material.SetFloat("_FlashAmount", 0);
                    }
                    if (!isDying)
                    {
                        rb.velocity = new Vector2(0, rb.velocity.y);
                        rb.AddForce(new Vector2(2f * characterStats.characterData.hitedDirection, 12f), ForceMode2D.Impulse);
                        isDying = true;
                    }
                    col.enabled = false;
                    col_dash.enabled = false;
                    col_collision.enabled = false;
                    Destroy(gameObject, 2f);
                    break;
                }
            case EnemyStates.ISHIT:
                {
                    rb.velocity = new Vector2(hurtedSpeed * hurtedDirection.x, hurtedUpSpeed);

                    col_collision.enabled = false;
                    col_dash.enabled = false;

                    lastHitTime -= Time.deltaTime;
                    if (lastHitTime >= (hitTime - hurtColorTime))//颜色变化时间
                    {
                        rbSprite.material.SetFloat("_FlashAmount", 1);
                    }
                    else if (lastHitTime < (hitTime - hurtColorTime))
                    {
                        rbSprite.material.SetFloat("_FlashAmount", 0);
                    }

                    if (lastHitTime <= 0f)
                    {
                        characterStats.characterData.Hitting = false;
                        col_collision.enabled = true;
                        if (isGuard)
                        {
                            watchTimer = watchTime;
                            enemyStates = EnemyStates.GUARD;
                        }
                        else if (!isGuard)
                        {
                            enemyStates = EnemyStates.PATROL;
                        }
                    }
                    break;
                }
        }
    }


    bool FoundPlayer()//寻找玩家是否在追击范围内
    {
        var colliders = Physics2D.OverlapAreaAll(new Vector2(transform.position.x + sightRadiusX * transform.localScale.x, transform.position.y + sightRadiusY), new Vector2(transform.position.x - sightRadiusX * 0.1f * transform.localScale.x, transform.position.y - sightRadiusY));
        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))//HACK:优化：此处标签后期可改为player受伤判定的碰撞体
            {
                attackTarget = target.gameObject;
                return true;
            }
        }
        attackTarget = null;

        return false;
    }

    private void Dash()//冲锋
    {
        rb.velocity = new Vector2(transform.localScale.x * chaseSpeed, rb.velocity.y);
    }


    void SwitchAnimation()
    {
        animator.SetBool("Dead", isDead);
        animator.SetBool("Hitting", characterStats.characterData.Hitting);
        animator.SetBool("Walk", isWalk);
        animator.SetBool("Chase", isChase);
        animator.SetBool("Follow", isFollow);
    }

    public void FlipTo(Transform target)
    {
        if (target != null)
        {
            if (target.position.x < transform.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (target.position.x > transform.position.x)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    private void GetNewWayPoint()//获取下一个巡逻目标点
    {
        remainWaitTime = waitTime;//还原计时器
        randomX = Random.Range(guardPos.x - patrolRange, guardPos.x + patrolRange);//HACK:优化：此处可能获取到无法移动的点，可做撞墙检测
        Vector2 randomPoint = new Vector2(transform.position.x + randomX, transform.position.y);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(new Vector3((transform.position.x + sightRadiusX * transform.localScale.x + transform.position.x - sightRadiusX * transform.localScale.x * 0.1f) * 0.5f, transform.position.y, transform.position.z), new Vector3(sightRadiusX * 1.1f, sightRadiusY * 2, 0));
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(guardPos.x - patrolRange, guardPos.y, guardPos.z), new Vector3(guardPos.x + patrolRange, guardPos.y, guardPos.z));
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + transform.localScale.x * 1, transform.position.y, transform.position.z));
    }

    public void EndNotify()
    {
        isChase = false;
        isWalk = false;
        //playerDead = true;
        attackTarget = null;
        rb.velocity = new Vector2(0, rb.velocity.y);
        Debug.Log("Game Over");
    }
}

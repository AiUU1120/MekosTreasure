using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeController : Enemy, IEndGameObserver
{
    #region DEFINITION
    public EnemyStates enemyStates;

    [Header("Basic Settings")]
    public float sightRadiusX;//可视范围/追击范围
    public float sightRadiusY;
    private Vector3 guardPos;//初始坐标
    private Vector3 guardLocalScale;
    private bool isFoundPlayer;//判断是否已经看到玩家
    public bool isGuard;//是否为站桩敌人
    private float localGravity;//记录全局初始重力

    [Header("Patrol State")]
    [SerializeField] float patrolRangeX;//巡逻范围
    [SerializeField] float patrolRangeY;
    public float stopDistance;//目标点停止距离
    public float waitTime;//到达目的地后等待时间
    private float remainWaitTime;//计时器：用于判断离结束等待还有多久
    private Vector2 randomPos;//目标点X坐标
    private Vector2 lastPos;//上次记录坐标X值
    private float recordTime = 0.5f;//记录坐标间隔时间
    private float recordTimer;
    private bool isTouchWall;
    private bool isArrive;//是否到达目的地

    [Header("Attack State")]
    [SerializeField] float chaseSpeed;//移动（追击）速度
    private GameObject attackTarget;//攻击目标（玩家）
    private Vector2 tempTargetPos;//临时攻击坐标
    private Vector2 beforePos;//攻击前坐标
    private bool isAttack;//是否已经完成攻击
    private float lastAttackTime;//计时器：用于计算下次攻击CD
    private bool isReturn;//已经攻击并返回

    [Header("ISHIT State")]
    public float hitTime;//受伤时间
    public float hurtColorTime;//受伤闪烁时间
    private float lastHitTime;//计时器：计算受伤时间


    private Animator animator;
    private Collider2D col_collision;
    private Collider2D col_dash;
    private AnimatorStateInfo info;
    private float infoPrecent;

    //配合动画
    bool isWalk;
    bool isChase;
    bool isFollow;

    bool isDying;

    bool playerDead;
    #endregion


    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        col_collision = transform.Find("AttackedArea_Bee").GetComponent<Collider2D>();
        col_dash = transform.Find("DashAttackArea_Bee").GetComponent<Collider2D>();
        guardPos = transform.position;
        guardLocalScale = transform.localScale;
        remainWaitTime = waitTime;
        localGravity = rb.gravityScale;
        rb.gravityScale = 0;//使其不受重力影响
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
    private void Update()
    {
        if (characterStats.CurHealth <= 0f)
        {
            isDead = true;
        }
        if (!playerDead)
        {
            SwtichStates();
        }
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
        else if (FoundPlayer() && enemyStates != EnemyStates.ATTACK && lastAttackTime < 0)
        {
            if (!isFoundPlayer)
            {
                FlipTo(attackTarget.transform);
                tempTargetPos = attackTarget.transform.position;
                beforePos = transform.position;
                characterStats.characterData.runSpeed = chaseSpeed;
                waitTime = waitTime * 0.5f;
            }
            isFoundPlayer = true;
            enemyStates = EnemyStates.ATTACK;
        }
        else if (!FoundPlayer())
        {
            isFoundPlayer = false;
        }

        switch (enemyStates)
        {
            case EnemyStates.GUARD:
                {
                    isChase = false;
                    isFollow = false;
                    if (Mathf.Abs(guardPos.x - transform.position.x) >= 0.2f)//判断是否到了初始点
                    {
                        float localScaleX;
                        localScaleX = (guardPos.x - transform.position.x) > 0f ? 1f : -1f;
                        transform.localScale = new Vector3(localScaleX, 1, 1);

                        isWalk = true;
                        rb.velocity = new Vector2(transform.localScale.x * chaseSpeed * 0.5f, rb.velocity.y);
                    }
                    else if (Mathf.Abs(guardPos.x - transform.position.x) < 0.2f)
                    {
                        isWalk = false;
                        transform.localScale = guardLocalScale;
                        rb.velocity = new Vector2(0, rb.velocity.y);//停止，恢复站桩
                    }
                    break;
                }
            case EnemyStates.PATROL:
                {
                    float localScaleX;
                    localScaleX = (randomPos.x - transform.position.x) > 0f ? 1f : -1f;
                    transform.localScale = new Vector3(localScaleX, 1, 1);
                    isChase = false;
                    isFollow = false;

                    lastAttackTime -= Time.deltaTime;//攻击CD

                    if (recordTimer > 0 && !isArrive)
                    {
                        recordTimer -= Time.deltaTime;
                    }
                    else if (recordTimer <= 0 && !isArrive)
                    {
                        if (Vector2.Distance(transform.position, lastPos) <= 0.2f)//判断撞墙
                        {
                            Debug.Log("IS TOUCH WALL!");
                            if (!isTouchWall)
                            {
                                remainWaitTime = waitTime;
                            }
                            isTouchWall = true;
                        }
                        else if (Vector2.Distance(transform.position, lastPos) > 0.2f)
                        {
                            lastPos = transform.position;
                        }
                        recordTimer = recordTime;
                    }
                    if (!isTouchWall)
                    {
                        if (Vector2.Distance(transform.position, randomPos) >= stopDistance)//判断是否到了巡逻目的点
                        {
                            //Debug.Log("Moving");
                            isWalk = true;
                            transform.position = Vector2.MoveTowards(transform.position, randomPos, characterStats.characterData.runSpeed * Time.deltaTime);
                        }
                        else if (Vector2.Distance(transform.position, randomPos) < stopDistance)
                        {
                            //Debug.Log("stop"); ;
                            isWalk = false;
                            isArrive = true;
                            rb.velocity = new Vector2(0, rb.velocity.y);
                            if (remainWaitTime > 0)//等待时间
                            {
                                remainWaitTime -= Time.deltaTime;
                            }
                            else
                            {
                                isArrive = false;
                                GetNewWayPoint();
                            }
                        }
                    }
                    else if (isTouchWall)
                    {
                        GetNewWayPoint();
                        isTouchWall = false;
                    }
                    break;
                }
            case EnemyStates.CHASE:
                {
                    break;
                }
            case EnemyStates.ATTACK:
                {
                    isWalk = false;
                    isChase = true;
                    isFollow = false;
                    col_dash.enabled = true;

                    Attack();

                    if (recordTimer > 0)
                    {
                        recordTimer -= Time.deltaTime;
                    }
                    else if (recordTimer <= 0)
                    {
                        if (Vector2.Distance(transform.position, lastPos) <= 0.2f)//判断撞墙
                        {
                            if (!isTouchWall)
                            {
                                Debug.Log("IS TOUCH WALL!");
                                remainWaitTime = waitTime;
                            }
                            isTouchWall = true;
                        }
                        else if (Vector2.Distance(transform.position, lastPos) > 0.2f)
                        {
                            lastPos = transform.position;
                        }
                        recordTimer = recordTime;
                    }

                    if (!isAttack && Vector2.Distance(transform.position, tempTargetPos) > 0.05f && !isTouchWall)
                    {
                        transform.position = Vector2.MoveTowards(transform.position, tempTargetPos, chaseSpeed * Time.deltaTime);
                    }
                    else if (!isAttack && Vector2.Distance(transform.position, tempTargetPos) <= 0.05f || isTouchWall)
                    {
                        isAttack = true;
                    }

                    if (isAttack && Vector2.Distance(transform.position, beforePos) > 0.05f)
                    {
                        transform.position = Vector2.MoveTowards(transform.position, beforePos, chaseSpeed * Time.deltaTime);
                    }
                    else if (isAttack && Vector2.Distance(transform.position, beforePos) <= 0.05f)
                    {
                        isReturn = true;
                    }

                    if (isReturn)
                    {
                        isAttack = false;
                        if (isGuard)//等待时间结束返回默认状态
                        {
                            remainWaitTime = waitTime;
                            col_dash.enabled = false;
                            enemyStates = EnemyStates.GUARD;
                        }
                        else if (!isGuard)
                        {
                            remainWaitTime = waitTime;
                            col_dash.enabled = false;
                            enemyStates = EnemyStates.PATROL;
                        }
                        isReturn = false;
                        lastAttackTime = characterStats.enemyAttackData.basicCoolDown_Enemy;
                    }
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
                    rb.gravityScale = localGravity;
                    if (!isDying)
                    {
                        rb.velocity = new Vector2(0, rb.velocity.y);
                        rb.AddForce(new Vector2(2f * characterStats.characterData.hitedDirection, 12f), ForceMode2D.Impulse);
                        isDying = true;
                    }
                    col.enabled = false;
                    col_collision.enabled = false;
                    Destroy(gameObject, 2f);
                    break;
                }
            case EnemyStates.ISHIT:
                {
                    rb.velocity = new Vector2(hurtedSpeed * hurtedDirection.x, hurtedUpSpeed);

                    col_collision.enabled = false;

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

    void Attack()
    {
        if (attackTarget != null)
        {
            FlipTo(attackTarget.transform);//HACK:脱战后空引用bug(修改1）
        }

    }

    bool FoundPlayer()//寻找玩家是否在追击范围内
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, sightRadiusX);
        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))//HACK:优化：此处标签后期可改为player受伤判定的碰撞体
            {
                attackTarget = target.gameObject;
                if (infoPrecent > 0.95f)
                {
                    FlipTo(target.transform);
                }
                return true;
            }
        }
        attackTarget = null;

        return false;
    }

    bool TargetInAttackRange()//判断是否玩家进入攻击范围
    {
        if (attackTarget != null)
        {
            return Mathf.Abs(attackTarget.transform.position.x - transform.position.x) <= characterStats.enemyAttackData.basicAttackRange_Enemy;
        }
        else
        {
            return false;
        }
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
        randomPos = new Vector2(Random.Range(guardPos.x - patrolRangeX, guardPos.x + patrolRangeX), Random.Range(guardPos.y - patrolRangeY, guardPos.y + patrolRangeY));//HACK:优化：此处可能获取到无法移动的点，可做撞墙检测
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadiusX);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(guardPos, new Vector3(patrolRangeX * 2, patrolRangeY * 2, 0));
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + transform.localScale.x * 1, transform.position.y, transform.position.z));
    }

    public void EndNotify()
    {
        isChase = false;
        isWalk = false;
        playerDead = true;
        attackTarget = null;
        //Debug.Log("Game Over");
    }
}

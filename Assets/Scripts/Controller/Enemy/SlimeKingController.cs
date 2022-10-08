using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeKingController : Enemy, IEndGameObserver
{
    #region DEFINITION
    public EnemyStates enemyStates;
    [SerializeField] GameObject SkillBullet;

    [Header("Basic Settings")]
    public float sightRadiusX;//可视范围/追击范围
    public float sightRadiusY;
    public float chaseSpeed;//移动（追击）速度
    private Vector3 guardPos;//初始坐标
    private Vector3 guardLocalScale;
    private GameObject attackTarget;//攻击目标（玩家）
    private bool isFoundPlayer;//判断是否已经看到玩家
    public bool isGuard;//是否为站桩敌人
    public float groundLength;//触地检测长度
    public Vector3 colliderOffset;//触地检测宽度
    private Vector3 skillPos;//生成弹丸位置

    [Header("Patrol State")]
    public float patrolRange;//巡逻范围
    public float stopDistance;//目标点停止距离
    public float waitTime;//到达目的地后等待时间
    private float remainWaitTime;//计时器：用于判断离结束等待还有多久
    private float lastAttackTime;//计时器：用于计算下次攻击CD
    private float randomX;//目标点X坐标
    bool isArrive;

    [Header("Chase State")]
    [SerializeField] float perceiveRangeX;//战斗状态感知范围
    [SerializeField] float perceiveRangeY;
    [SerializeField] float attackWaitTime;//攻击后停顿时间
    [SerializeField] float throwRange;//远程攻击范围
    float attackWaitTimer;//计时器：计算攻击停顿时间
    float lastPos;//上次记录坐标X值
    float recordTime = 0.5f;//记录坐标间隔时间
    float recordTimer;
    //float onTopTime = 1f;//玩家在上方判断时间间隔
    //float onTopTimer;//计时器：计算玩家在上方判断时间
    bool isTouchWall;//判断撞墙
    bool isOnGroud;//判断落地
    bool isTurn;

    [Header("ISHIT State")]
    public float hitTime;//受伤时间
    public float hurtColorTime;//受伤闪烁时间
    private float lastHitTime;//计时器：计算受伤时间


    public LayerMask ground;
    private Animator animator;
    private Collider2D col_collision;
    private AnimatorStateInfo info;
    public float infoPrecent;

    //配合动画
    bool isWalk;
    bool isChase;
    bool isFollow;

    bool isDying;

    bool playerDead;

    [Header("===== SFX =====")]
    [SerializeField]
    private AudioData slimeKingAttackSfx;
    #endregion


    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        col_collision = transform.Find("AttackedArea_SlimeKing").GetComponent<Collider2D>();
        guardPos = transform.position;
        guardLocalScale = transform.localScale;
        remainWaitTime = waitTime;
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
        //onTopTimer = onTopTime;
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
        isOnGroud = Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, ground) || Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, ground);
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
        lastAttackTime -= Time.deltaTime;
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
        else if (FoundPlayer() && enemyStates != EnemyStates.ATTACK)
        {
            if (!isFoundPlayer)
            {
                FlipTo(attackTarget.transform);
            }
            isFoundPlayer = true;
            enemyStates = EnemyStates.CHASE;
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
                        rb.velocity = new Vector2(transform.localScale.x * characterStats.characterData.runSpeed * transform.localScale.x, rb.velocity.y);
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
                    localScaleX = (randomX - transform.position.x) > 0f ? 1f : -1f;
                    transform.localScale = new Vector3(localScaleX, 1, 1);
                    isChase = false;
                    isFollow = false;
                    if (!isArrive)
                    {
                        IsTouchWall();
                    }

                    if (!isTouchWall)
                    {
                        if (Mathf.Abs(randomX - transform.position.x) < stopDistance)
                        {
                            isWalk = false;
                            isArrive = true;
                            rb.velocity = new Vector2(0, rb.velocity.y);
                            if (remainWaitTime > 0)//等待时间
                            {
                                remainWaitTime -= Time.deltaTime;
                            }
                            else if (remainWaitTime <= 0)
                            {
                                isArrive = false;
                                GetNewWayPoint();
                            }
                        }
                        else if (Mathf.Abs(randomX - transform.position.x) >= stopDistance)//判断是否到了巡逻目的点
                        {
                            isWalk = true;
                            rb.velocity = new Vector2(characterStats.characterData.runSpeed * transform.localScale.x, rb.velocity.y);
                        }
                    }
                    else if (isTouchWall)
                    {
                        isWalk = false;
                        isArrive = true;
                        rb.velocity = new Vector2(0, rb.velocity.y);
                        if (remainWaitTime > 0)//等待时间
                        {
                            remainWaitTime -= Time.deltaTime;
                        }
                        else if (remainWaitTime <= 0)
                        {
                            isArrive = false;
                            GetNewWayPoint();
                            isTouchWall = false;
                        }
                    }
                    break;
                }
            case EnemyStates.CHASE:
                {
                    isWalk = false;
                    isChase = true;
                    if (!FoundPlayer())//追击
                    {
                        if (PerceivePlayer())
                        {
                            FlipTo(attackTarget.transform);
                        }
                        if (infoPrecent > 0.95f)
                        {
                            isFollow = false;
                        }

                        if (!isFollow && remainWaitTime > 0f)//脱战后等待时间
                        {
                            remainWaitTime -= Time.deltaTime;
                            rb.velocity = new Vector2(0, rb.velocity.y);
                        }
                        else if (!isFollow && isGuard && remainWaitTime <= 0f)//等待时间结束返回默认状态
                        {
                            remainWaitTime = waitTime;
                            enemyStates = EnemyStates.GUARD;
                        }
                        else if (!isFollow && !isGuard)
                        {
                            remainWaitTime = waitTime;
                            enemyStates = EnemyStates.PATROL;
                        }
                    }
                    else
                    {
                        isFollow = true;
                        if (isOnGroud)
                        {
                            rb.velocity = new Vector2(transform.localScale.x * chaseSpeed, rb.velocity.y);
                        }
                        else if (!isOnGroud)
                        {
                            rb.velocity = new Vector2(transform.localScale.x * characterStats.characterData.runSpeed * 1.3f, rb.velocity.y);
                        }
                    }

                    if (TargetInSkillRange() && isOnGroud)//攻击
                    {
                        rb.velocity = new Vector2(0, rb.velocity.y);
                        attackWaitTimer = attackWaitTime;
                        enemyStates = EnemyStates.ATTACK;
                    }
                    break;
                }
            case EnemyStates.ATTACK:
                {
                    isFollow = false;
                    rb.velocity = new Vector2(0, rb.velocity.y);
                    if (!TargetInSkillRange())
                    {
                        if (attackWaitTimer > 0)//脱战后等待时间
                        {
                            attackWaitTimer -= Time.deltaTime;
                            rb.velocity = new Vector2(0, rb.velocity.y);
                        }
                        else if (attackWaitTimer <= 0)
                        {
                            if (PerceivePlayer())
                            {
                                FlipTo(attackTarget.transform);
                            }
                            remainWaitTime = waitTime;
                            enemyStates = EnemyStates.CHASE;
                        }
                    }
                    else if (lastAttackTime < 0f && isOnGroud)
                    {
                        lastAttackTime = characterStats.enemyAttackData.basicCoolDown_Enemy;
                        Attack();
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

        if (TargetInAttackRange())
        {
            animator.SetTrigger("Attack");
            Invoke("PlaySlimeKingSFX", 0.7f);
        }
        else if (!TargetInAttackRange())
        {
            animator.SetTrigger("Skill");
        }
    }

    void PlaySlimeKingSFX()
    {
        AudioManager.Instance.PlaySFX(slimeKingAttackSfx);
    }


    bool FoundPlayer()//寻找玩家是否在追击范围内
    {
        var colliders = Physics2D.OverlapAreaAll(new Vector2(transform.position.x + sightRadiusX * transform.localScale.x, transform.position.y + sightRadiusY), new Vector2(transform.position.x - sightRadiusX * 0.04f * transform.localScale.x, transform.position.y - sightRadiusY));
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

    bool PerceivePlayer()
    {
        var colliders = Physics2D.OverlapAreaAll(new Vector2(transform.position.x + perceiveRangeX, transform.position.y + perceiveRangeY), new Vector2(transform.position.x - perceiveRangeX, transform.position.y - perceiveRangeY));
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

    bool TargetInStopRange()//判断玩家进入停止范围
    {
        if (attackTarget != null)
        {
            return (Mathf.Abs(attackTarget.transform.position.x - transform.position.x) <= characterStats.enemyAttackData.basicAttackRange_Enemy);
        }
        else
        {
            return false;
        }
    }
    bool TargetInAttackRange()//判断是否玩家进入攻击范围
    {
        if (attackTarget != null)
        {
            return (Mathf.Abs(attackTarget.transform.position.x - transform.position.x) <= characterStats.enemyAttackData.basicAttackRange_Enemy && Mathf.Abs(attackTarget.transform.position.y - transform.position.y) <= characterStats.enemyAttackData.basicAttackRange_Enemy);
        }
        else
        {
            return false;
        }
    }

    bool TargetInSkillRange()//判断玩家进入技能范围
    {
        if (attackTarget != null)
        {
            return (Mathf.Abs(attackTarget.transform.position.x - transform.position.x) <= throwRange);
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
        randomX = Random.Range(guardPos.x - patrolRange, guardPos.x + patrolRange);//HACK:优化：此处可能获取到无法移动的点，可做撞墙检测
    }

    private bool IsTouchWall()//撞墙检测
    {
        if (recordTimer > 0)
        {
            recordTimer -= Time.deltaTime;
        }
        else if (recordTimer <= 0)
        {
            if (lastPos == transform.position.x)//判断撞墙
            {
                if (!isTouchWall)
                {
                    Debug.Log("IS TOUCH WALL!");
                    remainWaitTime = waitTime;
                }
                isTouchWall = true;
            }
            else if (lastPos != transform.position.x)
            {
                lastPos = transform.position.x;
            }
            recordTimer = recordTime;
            return false;
        }
        return false;
    }

    public void SpawnBullet()
    {
        skillPos = new Vector3(transform.position.x + 0.9f * transform.localScale.x, transform.position.y + 0.14f, transform.position.z);
        GameObject bullet = Instantiate(SkillBullet, skillPos, Quaternion.identity);
        bullet.GetComponent<Bullet_SlimeKing>().attackTarget = attackTarget;
        if(attackTarget == null)
        {
            bullet.GetComponent<Bullet_SlimeKing>().attackTarget = GameObject.Find("Meko");
        }
    }

    //private bool IsOnTop()//判断玩家是否在上方
    //{
    //    if (attackTarget != null)
    //    {
    //        if ((Mathf.Abs(attackTarget.transform.position.x - transform.position.x) <= (characterStats.enemyAttackData.basicAttackRange_Enemy * 1.3f)) && (Mathf.Abs(attackTarget.transform.position.y - transform.position.y) > (characterStats.enemyAttackData.basicAttackRange_Enemy) * 1.7f))
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    //private void Jump()
    //{
    //    if (!isTouchWall)
    //    {
    //        isTouchWall = true;
    //        rb.AddForce(new Vector2(0, characterStats.characterData.jumpSpeed), ForceMode2D.Impulse);
    //    }
    //}
    //private void PowerJump()
    //{
    //    if (!isOnTop)
    //    {
    //        isOnTop = true;
    //        rb.AddForce(new Vector2(characterStats.characterData.runSpeed, characterStats.characterData.jumpSpeed * 1.35f), ForceMode2D.Impulse);
    //    }
    //}

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(new Vector3((transform.position.x + sightRadiusX * transform.localScale.x + transform.position.x - sightRadiusX * transform.localScale.x * 0.04f) * 0.5f, transform.position.y, transform.position.z), new Vector3(sightRadiusX * 1.04f, sightRadiusY * 2, 0));
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(guardPos.x - patrolRange, guardPos.y, guardPos.z), new Vector3(guardPos.x + patrolRange, guardPos.y, guardPos.z));
        Gizmos.DrawWireCube(transform.position, new Vector3(perceiveRangeX * 2, perceiveRangeY * 2, 0));//感知范围
        Gizmos.DrawWireSphere(new Vector2(randomX, transform.position.y), 0.2f);//巡逻点
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z), new Vector3(transform.position.x + throwRange * transform.localScale.x, transform.position.y + 0.2f, transform.position.z));//远程攻击距离
        //Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + transform.localScale.x * characterStats.enemyAttackData.basicAttackRange_Enemy, transform.position.y, transform.position.z));//近战攻击距离
        Gizmos.DrawLine(transform.position + colliderOffset, transform.position + colliderOffset + Vector3.down * groundLength);//触地检测
        Gizmos.DrawLine(transform.position - colliderOffset, transform.position - colliderOffset + Vector3.down * groundLength);
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


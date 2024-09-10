using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeController : Enemy, IEndGameObserver
{
    #region DEFINITION
    public EnemyStates enemyStates;

    [Header("Basic Settings")]
    public float sightRadiusX;//���ӷ�Χ/׷����Χ
    public float sightRadiusY;
    private Vector3 guardPos;//��ʼ����
    private Vector3 guardLocalScale;
    private bool isFoundPlayer;//�ж��Ƿ��Ѿ��������
    public bool isGuard;//�Ƿ�Ϊվ׮����
    private float localGravity;//��¼ȫ�ֳ�ʼ����

    [Header("Patrol State")]
    [SerializeField] float patrolRangeX;//Ѳ�߷�Χ
    [SerializeField] float patrolRangeY;
    public float stopDistance;//Ŀ���ֹͣ����
    public float waitTime;//����Ŀ�ĵغ�ȴ�ʱ��
    private float remainWaitTime;//��ʱ���������ж�������ȴ����ж��
    private Vector2 randomPos;//Ŀ���X����
    private Vector2 lastPos;//�ϴμ�¼����Xֵ
    private float recordTime = 0.5f;//��¼������ʱ��
    private float recordTimer;
    private bool isTouchWall;
    private bool isArrive;//�Ƿ񵽴�Ŀ�ĵ�

    [Header("Attack State")]
    [SerializeField] float chaseSpeed;//�ƶ���׷�����ٶ�
    private GameObject attackTarget;//����Ŀ�꣨��ң�
    private Vector2 tempTargetPos;//��ʱ��������
    private Vector2 beforePos;//����ǰ����
    private bool isAttack;//�Ƿ��Ѿ���ɹ���
    private float lastAttackTime;//��ʱ�������ڼ����´ι���CD
    private bool isReturn;//�Ѿ�����������

    [Header("ISHIT State")]
    public float hitTime;//����ʱ��
    public float hurtColorTime;//������˸ʱ��
    private float lastHitTime;//��ʱ������������ʱ��


    private Animator animator;
    private Collider2D col_collision;
    private Collider2D col_dash;
    private AnimatorStateInfo info;
    private float infoPrecent;

    //��϶���
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
        rb.gravityScale = 0;//ʹ�䲻������Ӱ��
    }

    protected override void Start()
    {
        base.Start();
        GameManager.Instance.AddObserver(this);//HACK:�������л�ʱ�޸�

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
                    if (Mathf.Abs(guardPos.x - transform.position.x) >= 0.2f)//�ж��Ƿ��˳�ʼ��
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
                        rb.velocity = new Vector2(0, rb.velocity.y);//ֹͣ���ָ�վ׮
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

                    lastAttackTime -= Time.deltaTime;//����CD

                    if (recordTimer > 0 && !isArrive)
                    {
                        recordTimer -= Time.deltaTime;
                    }
                    else if (recordTimer <= 0 && !isArrive)
                    {
                        if (Vector2.Distance(transform.position, lastPos) <= 0.2f)//�ж�ײǽ
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
                        if (Vector2.Distance(transform.position, randomPos) >= stopDistance)//�ж��Ƿ���Ѳ��Ŀ�ĵ�
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
                            if (remainWaitTime > 0)//�ȴ�ʱ��
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
                        if (Vector2.Distance(transform.position, lastPos) <= 0.2f)//�ж�ײǽ
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
                        if (isGuard)//�ȴ�ʱ���������Ĭ��״̬
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
                    if (lastHitTime >= (hitTime - hurtColorTime))//��ɫ�仯ʱ��
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
                    if (lastHitTime >= (hitTime - hurtColorTime))//��ɫ�仯ʱ��
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
            FlipTo(attackTarget.transform);//HACK:��ս�������bug(�޸�1��
        }

    }

    bool FoundPlayer()//Ѱ������Ƿ���׷����Χ��
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, sightRadiusX);
        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))//HACK:�Ż����˴���ǩ���ڿɸ�Ϊplayer�����ж�����ײ��
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

    bool TargetInAttackRange()//�ж��Ƿ���ҽ��빥����Χ
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

    private void GetNewWayPoint()//��ȡ��һ��Ѳ��Ŀ���
    {
        remainWaitTime = waitTime;//��ԭ��ʱ��
        randomPos = new Vector2(Random.Range(guardPos.x - patrolRangeX, guardPos.x + patrolRangeX), Random.Range(guardPos.y - patrolRangeY, guardPos.y + patrolRangeY));//HACK:�Ż����˴����ܻ�ȡ���޷��ƶ��ĵ㣬����ײǽ���
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyStates { GUARD, PATROL, CHASE, ATTACK, DEAD, ISHIT }
public class SlimeController : Enemy, IEndGameObserver
{
    #region DEFINITION
    public EnemyStates enemyStates;

    [Header("Basic Settings")]
    public float sightRadiusX;//���ӷ�Χ/׷����Χ
    public float sightRadiusY;
    public float chaseSpeed;//�ƶ���׷�����ٶ�
    private Vector3 guardPos;//��ʼ����
    private Vector3 guardLocalScale;
    private GameObject attackTarget;//����Ŀ�꣨��ң�
    private bool isFoundPlayer;//�ж��Ƿ��Ѿ��������
    public bool isGuard;//�Ƿ�Ϊվ׮����

    [Header("Patrol State")]
    public float patrolRange;//Ѳ�߷�Χ
    public float stopDistance;//Ŀ���ֹͣ����
    public float waitTime;//����Ŀ�ĵغ�ȴ�ʱ��
    private float remainWaitTime;//��ʱ���������ж�������ȴ����ж��
    private float lastAttackTime;//��ʱ�������ڼ����´ι���CD
    private float randomX;//Ŀ���X����

    [Header("ISHIT State")]
    public float hitTime;//����ʱ��
    public float hurtColorTime;//������˸ʱ��
    private float lastHitTime;//��ʱ������������ʱ��


    private Animator animator;
    private Collider2D col_collision;
    private AnimatorStateInfo info;
    public float infoPrecent;

    //��϶���
    bool isWalk;
    bool isChase;
    bool isFollow;

    bool isDying;

    bool playerDead;

    [Header("===== SFX =====")]
    [SerializeField]
    private AudioData slimeAttackSfx;
    #endregion


    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        col_collision = transform.Find("AttackedArea_Slime").GetComponent<Collider2D>();
        guardPos = transform.position;
        guardLocalScale = transform.localScale;
        remainWaitTime = waitTime;
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
                    localScaleX = (randomX - transform.position.x) > 0f ? 1f : -1f;
                    transform.localScale = new Vector3(localScaleX, 1, 1);
                    isChase = false;
                    isFollow = false;

                    if (Mathf.Abs(randomX - transform.position.x) >= stopDistance)//�ж��Ƿ���Ѳ��Ŀ�ĵ�
                    {
                        isWalk = true;
                        rb.velocity = new Vector2(transform.localScale.x * chaseSpeed * 0.5f, rb.velocity.y);
                    }
                    else if (Mathf.Abs(randomX - transform.position.x) < stopDistance)
                    {
                        isWalk = false;
                        rb.velocity = new Vector2(0, rb.velocity.y);
                        if (remainWaitTime > 0)//�ȴ�ʱ��
                        {
                            remainWaitTime -= Time.deltaTime;
                        }
                        else
                        {
                            GetNewWayPoint();
                        }
                    }
                    break;
                }
            case EnemyStates.CHASE:
                {
                    isWalk = false;
                    isChase = true;
                    if (!FoundPlayer())//׷��
                    {
                        if (infoPrecent > 0.95f)
                        {
                            isFollow = false;
                        }

                        if (!isFollow && remainWaitTime > 0f)//��ս��ȴ�ʱ��
                        {
                            remainWaitTime -= Time.deltaTime;
                            rb.velocity = new Vector2(0, rb.velocity.y);
                        }
                        else if (!isFollow && isGuard && remainWaitTime <= 0f)//�ȴ�ʱ���������Ĭ��״̬
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
                        rb.velocity = new Vector2(transform.localScale.x * chaseSpeed, rb.velocity.y);
                    }

                    if (TargetInAttackRange())//����
                    {
                        rb.velocity = new Vector2(0, rb.velocity.y);
                        if (infoPrecent > 0.95f)
                        {
                            enemyStates = EnemyStates.ATTACK;
                        }
                    }
                    break;
                }
            case EnemyStates.ATTACK:
                {
                    isFollow = false;
                    if (!TargetInAttackRange())
                    {
                        if (remainWaitTime > 0)//��ս��ȴ�ʱ��
                        {
                            remainWaitTime -= Time.deltaTime;
                            rb.velocity = new Vector2(0, rb.velocity.y);
                        }
                        else if (isGuard)//�ȴ�ʱ���������Ĭ��״̬
                        {
                            enemyStates = EnemyStates.GUARD;
                        }
                        else if (!isGuard && FoundPlayer())
                        {
                            enemyStates = EnemyStates.CHASE;
                        }
                        else if (!isGuard && !FoundPlayer())
                        {
                            remainWaitTime = waitTime;
                            enemyStates = EnemyStates.PATROL;
                        }
                    }
                    else if (lastAttackTime < 0f)
                    {
                        lastAttackTime = characterStats.enemyAttackData.basicCoolDown_Enemy;
                        Attack();
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
        if (TargetInAttackRange())
        {
            animator.SetTrigger("Attack");
            Invoke("PlaySlimeSFX", 0.5f);
        }
    }

    void PlaySlimeSFX()
    {
        AudioManager.Instance.PlaySFX(slimeAttackSfx);
    }

    bool FoundPlayer()//Ѱ������Ƿ���׷����Χ��
    {
        var colliders = Physics2D.OverlapAreaAll(new Vector2(transform.position.x + sightRadiusX, transform.position.y + sightRadiusY), new Vector2(transform.position.x - sightRadiusX, transform.position.y - sightRadiusY));
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
        randomX = Random.Range(guardPos.x - patrolRange, guardPos.x + patrolRange);//HACK:�Ż����˴����ܻ�ȡ���޷��ƶ��ĵ㣬����ײǽ���
        Vector2 randomPoint = new Vector2(transform.position.x + randomX, transform.position.y);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(sightRadiusX * 2, sightRadiusY * 2, 0));
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(guardPos.x - patrolRange, guardPos.y, guardPos.z), new Vector3(guardPos.x + patrolRange, guardPos.y, guardPos.z));
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

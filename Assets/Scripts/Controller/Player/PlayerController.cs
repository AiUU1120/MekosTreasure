using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region DEFINITION
    public static PlayerController instance;
    private PlayerInput input;
    private CharacterStats characterStats;

    [Header("Components")]
    public LayerMask ground;
    public LayerMask platForm;
    public Image cdImage;//冲刺cd图像
    private Rigidbody2D rb;
    private BoxCollider2D m_collider;//角色物理碰撞体
    private Collider2D m_AttackCollider;//角色伤害碰撞体
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    PlayerFxController playerFxController;

    [Header("Status")]
    public float curSp;//
    public float hitTime;//受击硬直时间
    public float invincibleTime;//受击后无敌时间
    private float invincibleTimer;//计时器：计算无敌时间
    public float twinkleTime;//闪烁时间
    public float invincibleTwinkleTime;//无敌闪烁时间
    private float twinkleTimer;//计时器：计算闪烁时间
    private float invincibleTwinkleTimer;//计时器：计算无敌闪烁时间
    private WaitForSeconds regenTick = new WaitForSeconds(0.01f);
    private Coroutine regen;
    private bool isDead;//死亡状态
    private bool isDeadPlay;

    [Header("Horizontal Movement")]
    //private float horizontalMove;

    [Header("Vertical Movement")]
    public float jumpDelay = 0.25f;
    public bool canJumpDb = false;
    private bool isJumping;
    public int jumpCount;
    public int jumpStep;
    private float jumpTimer;

    [Header("Collision")]
    public bool onGround = false;
    private bool isOnAir;
    //public float groundLength;
    //public Vector3 colliderOffset;
    [SerializeField]
    private Vector2 boxCastOffset;
    [SerializeField]
    private Vector2 boxCastSize;
    private Vector2 colliderSize;
    private Vector2 collideroffset;
    [SerializeField]
    private float slopeCheckDistance;
    private float slopeDownAngle;//向下检测斜坡角度
    private float slopeDownAngleOld;
    private float slopeSideAngle;//坡度侧角度
    [SerializeField]
    private float maxSlopeAngle;
    private Vector2 slopeNormalPerp;//坡度法线
    private bool isOnSlope;
    [SerializeField]
    private PhysicsMaterial2D smoothMaterial;
    [SerializeField]
    private PhysicsMaterial2D toughMaterial;

    [Header("Physics")]
    public float linearDrag = 4f;
    private float gravity;
    [SerializeField]
    private float fallMultiplier;
    [SerializeField]
    private float upMultiplier;

    [Header("Attack")]
    public float atkInterval = 2f;
    public float atkSpeed;
    public float swDelay;
    public float keepButtonTime = 0.2f;
    public float swAttackTimer;
    public float fourthAtkTimer;
    public int comboStep;
    public float swAtkCost = 2;
    public float foAtkCost = 1;
    private float atkTimer;
    private bool cantCombo;
    private bool isCombo;
    private bool isAttack;
    private bool directionLock;
    private string atkType;

    [Header("Dash")]
    public float dashTime;
    public float dashSpeed;
    public float dashCD;
    public float dashCost;
    public bool isDashing;
    private float dashLeftTime = -10f;
    private float lastDashTime;

    [Header("===== SFX =====")]
    [SerializeField]
    private AudioData[] mekoAttackSFXs;
    [SerializeField]
    private AudioData[] mekoHeavyAttackSFXs;
    [SerializeField]
    private AudioData[] mekoSwordSFXs;
    [SerializeField]
    private AudioData[] mekoHeavySwordSFXs;
    [SerializeField]
    private AudioData[] mekoDoubleJumpSFXs;
    [SerializeField]
    private AudioData jumpGrassSFX;
    [SerializeField]
    private AudioData jumpFallSFX;
    [SerializeField]
    private AudioData[] combatSFXs;
    [SerializeField]
    private AudioData[] combatHeavySFXs;
    [SerializeField]
    private AudioData mekoDashSFX;
    [SerializeField]
    private AudioData dashSFX;
    [SerializeField]
    private AudioData mekoHitedSFX;
    [SerializeField]
    private AudioData mekoDeadSFX;

    #endregion

    private void Awake()
    {
        //if (instance == null)
        //{
        //    instance = this;
        //}
        //else
        //{
        //    if (instance != this)
        //    {
        //        Destroy(gameObject);
        //    }
        //}
        //DontDestroyOnLoad(gameObject);

        rb = GetComponent<Rigidbody2D>();
        m_collider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        input = GetComponent<PlayerInput>();
        characterStats = GetComponent<CharacterStats>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        cdImage = GameObject.Find("CdCover").GetComponent<Image>();
        playerFxController = GetComponent<PlayerFxController>();
    }

    private void OnEnable()
    {
        GameManager.Instance.RigisterPlayer(this.gameObject);//注册GameManager
        Debug.Log("RigisterPlayer");
    }
    void Start()
    {
        input.EnableGamePlayInputs();//激活输入
        //SaveManager.Instance.LoadPlayerData();//加载存档

        colliderSize = m_collider.size;
        collideroffset = m_collider.offset;
        m_AttackCollider = transform.Find("AttackedArea_Player").GetComponent<Collider2D>();
        characterStats.characterData.curHealth = characterStats.characterData.maxHealth;
        curSp = characterStats.MaxSp;
        characterStats.characterData.isHited = false;
        characterStats.characterData.Hitting = false;
        gravity = rb.gravityScale;
    }

    void Update()
    {
        isDead = characterStats.characterData.curHealth <= 0;
        if (isDead)
        {
            m_AttackCollider.enabled = false;
            GameManager.Instance.NotifyObservers();
            animator.SetBool("Dead", true);
            if (!isDeadPlay)
            {
                isDeadPlay = true;
                AudioManager.Instance.PlaySFX(mekoDeadSFX);
            }
            return;
        }
        invincibleTimer -= Time.deltaTime;
        twinkleTimer -= Time.deltaTime;
        cdImage.fillAmount -= 1.0f / dashCD * Time.deltaTime;
        //onGround = Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, ground) || Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, ground);
        //onGround = Physics2D.BoxCast(m_collider.bounds.center, m_collider.bounds.size, 0f, Vector2.down, 0.1f, ground);
        if (!isJumping)
        {
            onGround = Physics2D.BoxCast(transform.position + (Vector3)new Vector2(boxCastOffset.x * transform.localScale.x, boxCastOffset.y), boxCastSize, 0f, Vector2.down, 0.1f, ground) || Physics2D.BoxCast(transform.position + (Vector3)new Vector2(boxCastOffset.x * transform.localScale.x, boxCastOffset.y), boxCastSize, 0f, Vector2.down, 0.1f, platForm);
        }
        else
        {
            onGround = false;
        }

        ChangeAnimate();
        JumpCount();
        AttackTime();
        InvincibleState();

        if (hitTime > (invincibleTime - invincibleTimer))//硬直
        {
            return;
        }

        if (input.Jump)
        {
            jumpTimer = Time.time + jumpDelay;
        }

        if (input.Dash)
        {
            if (characterStats.characterData.canDash && Time.time >= (lastDashTime + dashCD) && curSp - dashCost >= 0)
            {
                ReadyToDash();
            }
        }
    }

    private void FixedUpdate()
    {
        //InvincibleState();
        if (hitTime > (invincibleTime - invincibleTimer))//硬直
        {
            rb.gravityScale = gravity;
            return;
        }
        if (isDead)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }
        Dash();
        if (isDashing)
        {
            return;
        }

        ModifyPhysics();
        GroundMovement();
        SlopeCheck();

        if (jumpTimer > Time.time && jumpCount > 0 && !isAttack)
        {
            Jump();
        }

        if (swAttackTimer > Time.time && !cantCombo && onGround && curSp - swAtkCost >= 0)
        {
            Attack();
        }

        if (fourthAtkTimer >= keepButtonTime && !cantCombo && onGround && comboStep == 4 && curSp - foAtkCost >= 0)
        {
            FourthAttack();
        }
    }

    private void ModifyPhysics()
    {
        if (onGround && isOnSlope && input.AxesX == 0 && slopeSideAngle <= maxSlopeAngle)
        {
            rb.gravityScale = 0;
            //m_collider.sharedMaterial = toughMaterial;
        }
        else if (onGround && isOnSlope && input.AxesX != 0)
        {
            if (rb.velocity.y < -0.1f)
            {
                rb.gravityScale = 1f;
            }
            else if (rb.velocity.y > 0.1f)
            {
                rb.gravityScale = 0;
            }
            else
            {
                rb.gravityScale = gravity;
            }
        }
        else
        {
            //m_collider.sharedMaterial = smoothMaterial;
            rb.gravityScale = gravity;
            rb.drag = linearDrag * 0.15f;
            if (rb.velocity.y < -0.1f)
            {
                rb.gravityScale = gravity * fallMultiplier;
            }
            else if (rb.velocity.y > 0.1f && !input.KeepJump)
            {
                rb.gravityScale = gravity * upMultiplier;
            }
        }
    }

    private void GroundMovement()
    {
        if (!isAttack)
        {
            if (onGround && !isOnSlope)
            {
                rb.velocity = new Vector2(input.AxesX * characterStats.characterData.runSpeed, 0f);
            }
            else if (onGround && isOnSlope)
            {
                rb.velocity = new Vector2(slopeNormalPerp.x * characterStats.characterData.runSpeed * -input.AxesX, slopeNormalPerp.y * characterStats.characterData.runSpeed * -input.AxesX);
            }
            else if (!onGround)
            {
                rb.velocity = new Vector2(input.AxesX * characterStats.characterData.runSpeed, rb.velocity.y);
            }
        }
        else
        {
            if (atkType == "Sword" && !isCombo)
            {
                if (onGround && !isOnSlope)
                {
                    rb.velocity = new Vector2(input.AxesX * characterStats.characterData.runSpeed, 0f);
                }
                else if (onGround && isOnSlope)
                {
                    rb.velocity = new Vector2(slopeNormalPerp.x * characterStats.characterData.runSpeed * -input.AxesX, slopeNormalPerp.y * characterStats.characterData.runSpeed * -input.AxesX);
                }
                else if (!onGround)
                {
                    rb.velocity = new Vector2(input.AxesX * characterStats.characterData.runSpeed, rb.velocity.y);
                }
            }
            if (atkType == "Sword" && isCombo)
            {
                rb.velocity = new Vector2(input.AxesX * atkSpeed, rb.velocity.y);
            }
        }

        if (input.AxesX != 0 && !directionLock)
        {
            transform.localScale = new Vector3(input.AxesX, 1, 1);
        }
        animator.SetFloat("Running", Mathf.Abs(input.AxesX));
    }

    private void Jump()
    {
        //Debug.Log("Jumpcount:" + jumpCount);
        animator.SetBool("Falling", false);
        animator.SetBool("Idle", false);
        rb.velocity = new Vector2(0f, 0f);

        if (jumpCount == characterStats.JumpAbility)
        {
            isJumping = true;
            animator.SetBool("Jumping", true);
            //rb.AddForce(Vector2.up * characterStats.characterData.jumpSpeed, ForceMode2D.Impulse);
            rb.AddForce(new Vector2(0f, characterStats.characterData.jumpSpeed), ForceMode2D.Impulse);
            AudioManager.Instance.PlayRandomSFX(jumpGrassSFX);
        }
        else if (jumpCount == characterStats.JumpAbility - 1)
        {
            isJumping = true;
            animator.SetBool("Jumping", false);
            animator.SetBool("Jumping2", true);
            rb.AddForce(Vector2.up * characterStats.characterData.jumpSpeed * 0.85f, ForceMode2D.Impulse);
            AudioManager.Instance.PlayRandomSFX(mekoDoubleJumpSFXs);
        }
        jumpCount--;
        jumpStep++;
        //Debug.Log(jumpStep);

        jumpTimer = 0;
    }

    private void JumpCount()
    {
        if (onGround)
        {
            jumpCount = characterStats.JumpAbility;
            jumpStep = 0;
        }

        if (jumpCount == characterStats.JumpAbility && animator.GetBool("Falling"))
        {
            //Debug.Log("next is DbJump");
            jumpCount = characterStats.JumpAbility - 1;
        }
    }

    void AttackTime()//攻击间隔
    {
        //if (Input.GetButtonDown("Fire1"))
        //{
        //    swAttackTimer = Time.time + swDelay;
        //    fourthAtkTimer = 0f;
        //}
        if (input.Fire1 && characterStats.characterData.canAttack)
        {
            swAttackTimer = Time.time + swDelay;
            fourthAtkTimer = 0f;
        }

        //if (Input.GetButton("Fire1"))
        //{
        //    fourthAtkTimer += Time.deltaTime;
        //}
        if (input.KeepFire1 && characterStats.characterData.canAttack)
        {
            fourthAtkTimer += Time.deltaTime;
        }

        if (atkTimer != 0)
        {
            atkTimer -= Time.deltaTime;
            if (atkTimer < 0)
            {
                atkTimer = 0;
                comboStep = 0;
                isCombo = false;
            }
        }
    }

    void Attack()//连击计算与攻击动画
    {
        directionLock = false;
        isAttack = true;
        cantCombo = true;
        if (comboStep >= 1)
        {
            isCombo = true;
        }
        atkType = "Sword";
        comboStep++;

        if (comboStep > characterStats.characterData.fullCombo || comboStep == 5)
        {
            comboStep = 1;
        }

        atkTimer = atkInterval;
        animator.SetTrigger("SwordAttack");
        animator.SetInteger("ComboStep", comboStep);
        AudioManager.Instance.PlayRandomSFX(mekoAttackSFXs);
        AudioManager.Instance.PlayRandomSFX(mekoSwordSFXs);

        curSp -= swAtkCost;
        if (regen != null)
        {
            StopCoroutine(regen);
        }
        regen = StartCoroutine(RegenStamina());

        swAttackTimer = 0;
    }

    void FourthAttack()//第四下连击技能
    {
        directionLock = false;
        isAttack = true;
        cantCombo = true;
        isCombo = true;
        atkType = "Sword";
        comboStep = 5;

        if (comboStep > characterStats.characterData.fullCombo)
        {
            comboStep = 1;
        }

        atkTimer = atkInterval;
        animator.SetTrigger("SwordAttack");
        animator.SetInteger("ComboStep", comboStep);
        AudioManager.Instance.PlayRandomSFX(mekoHeavyAttackSFXs);
        AudioManager.Instance.PlayRandomSFX(mekoHeavySwordSFXs);

        curSp -= foAtkCost;
        if (regen != null)
        {
            StopCoroutine(regen);
        }
        regen = StartCoroutine(RegenStamina());

        swAttackTimer = 0;
    }

    public void CanCombo()
    {
        cantCombo = false;
    }

    public void AttackOver()
    {
        isAttack = false;
    }

    private void OnTriggerEnter2D(Collider2D other)//攻击碰撞判定
    {
        if (other.CompareTag("Enemy"))
        {
            //PlayerFxController.instance.ChangeAttackAnimation(comboStep);
            playerFxController.ChangeAttackAnimation(comboStep);
            if (comboStep <= 4)
            {
                AudioManager.Instance.PlayRandomSFX(combatSFXs);
            }
            else if(comboStep > 4)
            {
                AudioManager.Instance.PlayRandomSFX(combatHeavySFXs);
            }
            other.GetComponent<Enemy>().isHited = true;
            if (transform.localScale.x > 0)
            {
                other.GetComponent<Enemy>().GetHit(Vector2.right);
            }
            else if (transform.localScale.x < 0)
            {
                other.GetComponent<Enemy>().GetHit(Vector2.left);
            }
        }
        else if (other.CompareTag("Boss"))
        {
            playerFxController.ChangeAttackAnimation(comboStep);
            if (comboStep <= 4)
            {
                AudioManager.Instance.PlayRandomSFX(combatSFXs);
            }
            else if (comboStep > 4)
            {
                AudioManager.Instance.PlayRandomSFX(combatHeavySFXs);
            }
            other.GetComponent<FSM_Felko>().ChangeFelkoHpBar();
        }
    }

    private IEnumerator RegenStamina()//协程回复SP
    {
        yield return new WaitForSeconds(0.7f);

        while (curSp < characterStats.MaxSp)
        {
            curSp += characterStats.MaxSp / 100;
            yield return regenTick;
        }
        regen = null;
    }

    private void ReadyToDash()
    {
        isDashing = true;
        dashLeftTime = dashTime;
        lastDashTime = Time.time;
        curSp -= dashCost;
        if (regen != null)
        {
            StopCoroutine(regen);
        }
        regen = StartCoroutine(RegenStamina());
        cdImage.fillAmount = 1;
        AudioManager.Instance.PlaySFX(mekoDashSFX);
        AudioManager.Instance.PlaySFX(dashSFX);
    }

    private void Dash()
    {
        if (isDashing)
        {
            float dashDirection = Input.GetAxisRaw("Horizontal");
            dashDirection = dashDirection > 0f ? 1f : dashDirection < 0f ? -1f : 0f;
            if (dashLeftTime > 0)
            {
                rb.gravityScale = 0f;
                m_AttackCollider.enabled = false;//冲刺无敌
                if (dashDirection != 0)
                {
                    rb.velocity = new Vector2(dashSpeed * dashDirection, 0f);
                    dashLeftTime -= Time.deltaTime;
                    ShadowPool.instance.GetFromPool();
                }
                if (dashDirection == 0)
                {
                    rb.velocity = new Vector2(dashSpeed * gameObject.transform.localScale.x, 0f);
                    dashLeftTime -= Time.deltaTime;
                    ShadowPool.instance.GetFromPool();
                }
            }
            if (dashLeftTime <= 0)
            {
                rb.gravityScale = gravity;
                m_AttackCollider.enabled = true;
                isDashing = false;
                rb.velocity = new Vector2(0f, 0f);
                if (!onGround)
                {
                    rb.velocity = new Vector2(0, characterStats.characterData.jumpSpeed / 2);
                }
            }
        }
    }

    private void InvincibleState()//无敌状态
    {
        if (characterStats.characterData.isHited)
        {
            invincibleTimer = invincibleTime;
            twinkleTimer = twinkleTime;
            animator.SetTrigger("Hited");
            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector2(characterStats.characterData.runSpeed * 0.3f * characterStats.characterData.hitedDirection, 2.5f), ForceMode2D.Impulse);
            //rb.velocity = new Vector2(characterStats.characterData.runSpeed * characterStats.characterData.hitedDirection, 2f);
            characterStats.characterData.isHited = false;
            AudioManager.Instance.PlaySFX(mekoHitedSFX);
        }

        if (hitTime >= (invincibleTime - invincibleTimer))//硬直时间
        {
            m_AttackCollider.enabled = false;
            if (twinkleTimer > 0)
            {
                spriteRenderer.material.SetFloat("_FlashAmount", 1);
            }
            else if (twinkleTimer <= 0)
            {
                spriteRenderer.material.SetFloat("_FlashAmount", 0);
            }
        }
        else if (hitTime < (invincibleTime - invincibleTimer) && characterStats.characterData.Hitting)
        {
            characterStats.characterData.Hitting = false;
        }
        else if (invincibleTimer > 0)//无敌时间
        {
            invincibleTwinkleTimer -= Time.deltaTime;

            if (invincibleTwinkleTimer <= 0)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
                invincibleTwinkleTimer = invincibleTwinkleTime;
            }
            m_AttackCollider.enabled = false;
        }
        else if (invincibleTimer <= 0)
        {
            spriteRenderer.enabled = true;
            m_AttackCollider.enabled = true;
        }
    }

    private void ChangeAnimate()
    {
        animator.SetBool("Hitting", characterStats.characterData.Hitting);
        if (rb.velocity.y <= 0.1f)
        {
            isJumping = false;
            animator.SetBool("Idle", false);
            animator.SetBool("Jumping", false);
            animator.SetBool("Jumping2", false);
            animator.SetBool("Falling", true);
        }

        if (onGround)
        {
            if (isOnAir)
            {
                isOnAir = false;
                AudioManager.Instance.PlaySFX(jumpFallSFX);
            }
            animator.SetBool("Jumping", false);
            animator.SetBool("Jumping2", false);
            animator.SetBool("Falling", false);
            animator.SetBool("Idle", true);
        }

        if (!onGround)
        {
            isOnAir = true;
        }
    }

    private void OnDrawGizmosSelected()//触地射线检测画图
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + (Vector3)new Vector2(boxCastOffset.x * transform.localScale.x, boxCastOffset.y), boxCastSize);
        //Gizmos.DrawLine(transform.position + colliderOffset, transform.position + colliderOffset + Vector3.down * groundLength);
        //Gizmos.DrawLine(transform.position - colliderOffset, transform.position - colliderOffset + Vector3.down * groundLength);
        //Gizmos.Draw
    }

    private void SlopeCheck()//斜率检测
    {
        //Vector2 checkPos = transform.position + (Vector3)(new Vector2(0f, collideroffset.y)) - (Vector3)(new Vector2(0f, colliderSize.y / 2));
        Vector2 checkPos = m_collider.bounds.center - (Vector3)(new Vector2(0f, colliderSize.y / 2)) - (Vector3)new Vector2(0f, m_collider.edgeRadius);
        Debug.DrawRay(checkPos, new Vector2(transform.localScale.x * slopeCheckDistance, 0f), Color.cyan);
        SlopeCheckHorizontal(checkPos);
        SlopeCheckVertical(checkPos);
    }

    private void SlopeCheckHorizontal(Vector2 checkPos)//水平射线检测斜坡
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, new Vector2(transform.localScale.x, 0f), slopeCheckDistance, ground);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, new Vector2(-transform.localScale.x, 0f), slopeCheckDistance, ground);

        if (slopeHitFront)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
        }
        else if (slopeHitBack)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
        }
        else
        {
            slopeSideAngle = 0f;
            isOnSlope = false;
        }
    }

    private void SlopeCheckVertical(Vector2 checkPos)//垂直射线检测斜坡
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, ground);
        if (hit)
        {
            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;
            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if (slopeDownAngle != slopeDownAngleOld)
            {
                isOnSlope = true;
            }
            slopeDownAngleOld = slopeDownAngle;

            Debug.DrawRay(hit.point, slopeNormalPerp, Color.yellow);
            Debug.DrawRay(hit.point, hit.normal, Color.yellow);
        }
    }

    public void endPlay()
    {
        animator.SetBool("Jumping2", false);
        animator.SetBool("Jumping", true);
    }

    public void DirectionLock()
    {
        directionLock = true;
    }

    public void unDirectionLock()
    {
        directionLock = false;
    }

    public void ReStartGame()
    {
        SceneController.Instance.TransitionToLoadGame();
    }
}

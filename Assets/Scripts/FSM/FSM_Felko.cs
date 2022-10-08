using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum FelkoStateType
{
    Idle, Ready, Chase, Attack, Dash, AttackHeavy, FireBall
}


[System.Serializable]
public class Parameter
{
    public GameObject felkoHpCanvasPrefab;
    public CharacterStats characterStats;
    public Transform target;
    public LayerMask targetLayer;
    public Rigidbody2D rb;
    public Transform attackPoint;//攻击范围圆心位置
    public float attackArea;//攻击圆半径
    public Animator animator;

    [Header("===== Boss States =====")]
    public bool isDead;
    public float shakingTime;
    public float shakingTimer;

    [Header("===== Skill Data =====")]
    public bool isSkilling;
    public float dashTime;
    public float dashTimer;
    public float dashSpeed;
    public BoxCollider2D dashCol;
    public GameObject fireBallPrefab;
    public Transform fireBallPoint;

    [Header("===== Skill CD =====")]
    public float idleTime;
    public float attackCD;
    public float dashCD;
    public float attackHeavyCD;
    public float fireBallCD;

    [Header("===== Skill CD Timer ======")]
    public float idleTimer;
    public float attackCDTimer;
    public float dashCDTimer;
    public float attackHeavyCDTimer;
    public float fireBallCDTimer;

    [Header("===== SFX =====")]
    public AudioData felkoAttackSfx;
    public AudioData felkoHeavyAttackSfx;
    public AudioData felkoDashSfx;
    public AudioData felkoFireBallSfx;
}


public class FSM_Felko : MonoBehaviour
{
    private IState currentState;
    public Parameter parameter;
    private Dictionary<FelkoStateType, IState> states = new Dictionary<FelkoStateType, IState>();
    [SerializeField]
    private Image hpBar;
    private GameObject hpCanvas;
    private SpriteRenderer rbSprite;
    [SerializeField]
    private GameObject felkoReplayNpc;
    [SerializeField]
    private DialogueData_SO firstBeatData;
    [SerializeField]
    private DialogueData_SO reBeatData;
    [SerializeField]
    private GameObject felkoWallPrefab;
    private GameObject felkoWall;
    

    void Start()
    {
        felkoWall = Instantiate(felkoWallPrefab, null);
        AudioManager.Instance.FelkoBgmStart();

        states.Add(FelkoStateType.Idle, new IdleState_Felko(this));
        states.Add(FelkoStateType.Ready, new ReadyState_Felko(this));
        states.Add(FelkoStateType.Chase, new ChaseState_Felko(this));
        states.Add(FelkoStateType.Attack, new AttackState_Felko(this));
        states.Add(FelkoStateType.Dash, new DashState_Felko(this));
        states.Add(FelkoStateType.AttackHeavy, new AttackHeavyState_Felko(this));
        states.Add(FelkoStateType.FireBall, new FireBallState_Felko(this));

        parameter.animator = GetComponent<Animator>();
        parameter.characterStats = GetComponent<CharacterStats>();
        parameter.rb = GetComponent<Rigidbody2D>();

        parameter.idleTime = 1.5f;
        parameter.idleTimer = parameter.idleTime;
        parameter.attackCDTimer = parameter.attackCD;
        parameter.dashCDTimer = parameter.dashCD;
        parameter.fireBallCDTimer = parameter.fireBallCD;
        parameter.attackHeavyCDTimer = parameter.attackHeavyCD;

        hpCanvas = Instantiate(parameter.felkoHpCanvasPrefab);
        hpBar = hpCanvas.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
        rbSprite = GetComponent<SpriteRenderer>();

        TransitionState(FelkoStateType.Idle);
    }

    void Update()
    {
        HitCheck();
        parameter.isDead = parameter.characterStats.characterData.curHealth <= 0.1f;
        if (parameter.isDead)
        {
            MekoWinFelko();
            return;
        }
        FoundPlayer();
        ReFreshCD();
        currentState.OnUpdate();
        if (!parameter.isSkilling)
        {
            SelectSkill();
        }
    }

    public void TransitionState(FelkoStateType type)
    {
        if (currentState != null)
        {
            currentState.OnExit();
        }
        currentState = states[type];
        currentState.OnEnter();
    }

    public void FlipTo(Transform target)
    {
        if (target != null)
        {
            if (transform.position.x > target.transform.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (transform.position.x < target.transform.position.x)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    public void FoundPlayer()
    {
        var colliders = Physics2D.OverlapAreaAll(new Vector2(170f, 28f), new Vector2(210f, 38f));
        foreach (var other in colliders)
        {
            if (other.CompareTag("Player"))//HACK:优化：此处标签后期可改为player受伤判定的碰撞体
            {
                parameter.target = other.transform;
                return;
                //Debug.Log("Find Player");
            }
        }
        parameter.target = null;
    }

    private void ReFreshCD()
    {
        parameter.attackCDTimer -= Time.deltaTime;
        parameter.dashCDTimer -= Time.deltaTime;
        parameter.fireBallCDTimer -= Time.deltaTime;
        parameter.attackHeavyCDTimer -= Time.deltaTime;
    }

    private void SelectSkill()
    {
        if (parameter.attackHeavyCDTimer < 0f)
        {
            parameter.isSkilling = true;
            Debug.Log("Select Heavy");
            TransitionState(FelkoStateType.AttackHeavy);
        }
        else if (parameter.fireBallCDTimer < 0f)
        {
            parameter.isSkilling = true;
            Debug.Log("Select FireBall");
            TransitionState(FelkoStateType.FireBall);
        }
        else if (parameter.dashCDTimer < 0f)
        {
            parameter.isSkilling = true;
            Debug.Log("Select Dash");
            TransitionState(FelkoStateType.Dash);
        }
    }

    public void ChangeFelkoHpBar()
    {
        hpBar.fillAmount = parameter.characterStats.characterData.curHealth / parameter.characterStats.characterData.maxHealth;
    }

    public void FireBallCreate()
    {
        if (transform.localScale.x > 0f)
        {
            for (int i = -2; i < 3; i++)
            {
                GameObject fireball = Instantiate(parameter.fireBallPrefab, null);
                Vector3 dir = Quaternion.Euler(0, i * 12, 0) * transform.right;

                fireball.transform.position = parameter.fireBallPoint.position + dir * 1.0f;
                Debug.Log(i);
                fireball.transform.rotation = Quaternion.Euler(0, 0, i * 12);
            }
        }
        else if(transform.localScale.x < 0f)
        {
            for (int i = -2; i < 3; i++)
            {
                GameObject fireball = Instantiate(parameter.fireBallPrefab, null);
                Vector3 dir = Quaternion.Euler(0, i * 12, 0) * -transform.right;

                fireball.transform.position = parameter.fireBallPoint.position + dir * 1.0f;
                Debug.Log(i);
                fireball.transform.rotation = Quaternion.Euler(0, 0, i * 12);
            }
        }
    }

    public void HitCheck()
    {
        if (parameter.characterStats.Hitting)
        {
            if (parameter.characterStats.IsHited)
            {
                parameter.shakingTimer = parameter.shakingTime;
                parameter.characterStats.IsHited = false;
            }
            HitShaking();
        }
    }

    public void HitShaking()
    {
        parameter.shakingTimer -= Time.deltaTime;
        if (parameter.shakingTimer > 0)//颜色变化时间
        {
            rbSprite.material.SetFloat("_FlashAmount", 1);
        }
        else if (parameter.shakingTimer <= 0)
        {
            rbSprite.material.SetFloat("_FlashAmount", 0);
            parameter.characterStats.Hitting = false;
        }
    }

    public void MekoWinFelko()
    {
        AudioManager.Instance.ForestBgmStart();
        if (!PlayerSaveData.Instance.isFelkoBeat)
        {
            PlayerSaveData.Instance.isFelkoBeat = true;
            PlayerSaveData.Instance.Save(false);
            InputDeviceDetection.GameplayUIController.Instance.UpdateDialogueData(firstBeatData);
            InputDeviceDetection.GameplayUIController.Instance.UpdateMainDialogue(firstBeatData.dialoguePieces[0]);
            Instantiate(felkoReplayNpc, transform.position, Quaternion.identity);
        }
        else if (PlayerSaveData.Instance.isFelkoBeat)
        {
            InputDeviceDetection.GameplayUIController.Instance.UpdateDialogueData(reBeatData);
            InputDeviceDetection.GameplayUIController.Instance.UpdateMainDialogue(reBeatData.dialoguePieces[0]);
            Instantiate(felkoReplayNpc, transform.position, Quaternion.identity);
        }
        Destroy(felkoWall);
        Destroy(gameObject);
        Destroy(hpCanvas);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector2(190f, 33f), new Vector2(40f, 10f));
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(parameter.attackPoint.position, parameter.attackArea);
    }
#endif
}

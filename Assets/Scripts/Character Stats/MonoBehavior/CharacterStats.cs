using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public CharacterData_SO characterData;

    public CharacterData_SO templateData;//模板数据

    public AttackData_SO attackData;

    public AttackData_SO templateAttackData;

    public EnemyAttackData_SO enemyAttackData;

    public GameObject damageCanvas;

    private void Awake()
    {
        if (templateData != null)
        {
            characterData = Instantiate(templateData);
        }
        if(templateAttackData != null)
        {
            attackData = Instantiate(templateAttackData);
        }
    }

    #region Read From Data_SO
    public float MaxHealth
    {
        get { if (characterData != null) { return characterData.maxHealth; } else return 0; }
        set { characterData.maxHealth = value; }
    }
    public float CurHealth
    {
        get { if (characterData != null) { return characterData.curHealth; } else return 0; }
        set { characterData.curHealth = value; }
    }
    public float MaxSp
    {
        get { if (characterData != null) { return characterData.maxSp; } else return 0; }
        set { characterData.maxSp = value; }
    }
    public float CurSp
    {
        get { if (characterData != null) { return characterData.curSp; } else return 0; }
        set { characterData.curSp = value; }
    }
    public int CurDefence
    {
        get { if (characterData != null) { return characterData.curDefence; } else return 0; }
        set { characterData.curDefence = value; }
    }
    public int JumpAbility
    {
        get { if (characterData != null) { return characterData.jumpAbility; } else return 0; }
        set { characterData.jumpAbility = value; }
    }
    public int FullCombo
    {
        get { if (characterData != null) { return characterData.fullCombo; } else return 0; }
        set { characterData.fullCombo = value; }
    }
    public float RunSpeed
    {
        get { if (characterData != null) { return characterData.runSpeed; } else return 0; }
        set { characterData.runSpeed = value; }
    }
    public float JumpSpeed
    {
        get { if (characterData != null) { return characterData.jumpSpeed; } else return 0; }
        set { characterData.jumpSpeed = value; }
    }
    public bool CanDash
    {
        get { if (characterData != null) { return characterData.canDash; } else return false; }
        set { characterData.canDash = value; }
    }
    public int BasicAttackDamage
    {
        get { if (characterData != null) { return attackData.basicAttackDamage; } else return 0; }
        set { attackData.basicAttackDamage = value; }
    }
    public int BasicMinDamage
    {
        get { if (characterData != null) { return attackData.basicMinDamage; } else return 0; }
        set { attackData.basicMinDamage = value; }
    }
    public int BasicMaxDamage
    {
        get { if (characterData != null) { return attackData.basicMaxDamage; } else return 0; }
        set { attackData.basicMaxDamage = value; }
    }
    public int FourthAttackDamage
    {
        get { if (characterData != null) { return attackData.fourthAttackDamage; } else return 0; }
        set { attackData.fourthAttackDamage = value; }
    }
    public int FourthMinDamage
    {
        get { if (characterData != null) { return attackData.fourthMinDamage; } else return 0; }
        set { attackData.fourthMinDamage = value; }
    }
    public int FourthMaxDamage
    {
        get { if (characterData != null) { return attackData.fourthMaxDamage; } else return 0; }
        set { attackData.fourthMaxDamage = value; }
    }
    public float BasicAttackRange_Enemy
    {
        get { if (characterData != null) { return enemyAttackData.basicAttackRange_Enemy; } else return 0; }
        set { enemyAttackData.basicAttackRange_Enemy = value; }
    }
    public int BasicAttackDamage_Enemy
    {
        get { if (characterData != null) { return enemyAttackData.basicAttackDamage_Enemy; } else return 0; }
        set { enemyAttackData.basicAttackDamage_Enemy = value; }
    }
    public int BasicMinDamage_Enemy
    {
        get { if (characterData != null) { return enemyAttackData.basicMinDamage_Enemy; } else return 0; }
        set { enemyAttackData.basicMinDamage_Enemy = value; }
    }
    public int BasicMaxDamage_Enemy
    {
        get { if (characterData != null) { return enemyAttackData.basicMaxDamage_Enemy; } else return 0; }
        set { enemyAttackData.basicMaxDamage_Enemy = value; }
    }
    public float BasicCoolDown_Enemy
    {
        get { if (characterData != null) { return enemyAttackData.basicCoolDown_Enemy; } else return 0; }
        set { enemyAttackData.basicCoolDown_Enemy = value; }
    }
    public int CollisionMinDamage_Enemy
    {
        get { if (characterData != null) { return enemyAttackData.collisionMinDamage_Enemy; } else return 0; }
        set { enemyAttackData.collisionMinDamage_Enemy = value; }
    }
    public int CollisionMaxDamage_Enemy
    {
        get { if (characterData != null) { return enemyAttackData.collisionMaxDamage_Enemy; } else return 0; }
        set { enemyAttackData.collisionMaxDamage_Enemy = value; }
    }
    public bool IsHited
    {
        get { if (characterData != null) { return characterData.isHited; } else return false; }
        set { characterData.isHited = value; }
    }
    public bool Hitting
    {
        get { if (characterData != null) { return characterData.Hitting; } else return false; }
        set { characterData.Hitting = value; }
    }
    public float HitedDirection
    {
        get { if (characterData != null) { return characterData.hitedDirection; } else return 0; }
        set { characterData.hitedDirection = value; }
    }
    public int Coins
    {
        get { if (characterData != null) { return characterData.coins; } else return 0; }
        set { characterData.coins = value; }
    }
    public int KillMinCoins
    {
        get { if (characterData != null) { return characterData.killMinCoins; } else return 0; }
        set { characterData.killMinCoins = value; }
    }
    public int KillMaxCoins
    {
        get { if (characterData != null) { return characterData.killMaxCoins; } else return 0; }
        set { characterData.killMaxCoins = value; }
    }


    #endregion

    #region Character Combat
    public void TakeDamage(CharacterStats attacker, CharacterStats defender, int minDamage, int maxDamage)
    {
        int damage = attacker.CurDamage(minDamage, maxDamage);
        damage = (int)(damage * (1 - characterData.curDefence * 0.05f));
        CurHealth = Mathf.Max(CurHealth - damage, 0);//更新血量并保证最低血量不为负值
        HitedDirection = attacker.gameObject.transform.localScale.x;
        IsHited = true;
        Hitting = true;
        DamageNum damagable = Instantiate(damageCanvas, defender.gameObject.transform.position, Quaternion.identity).GetComponent<DamageNum>();
        damagable.ShowUIDamge(damage);
        //Debug.Log(damage);
        if (CurHealth <= 0)
        {
            attacker.characterData.CollectCoins(characterData.killMinCoins, characterData.killMaxCoins);
        }
    }

    public void TakeDamage(int minDamage, int maxDamage, CharacterStats defender, int direction)
    {
        int damage = CurDamage(minDamage, maxDamage);
        damage = (int)(damage * (1 - characterData.curDefence * 0.05f));
        CurHealth = Mathf.Max(CurHealth - damage, 0);//更新血量并保证最低血量不为负值
        HitedDirection = direction;
        IsHited = true;
        Hitting = true;
        DamageNum damagable = Instantiate(damageCanvas, defender.gameObject.transform.position, Quaternion.identity).GetComponent<DamageNum>();
        damagable.ShowUIDamge(damage);
    }

    private int CurDamage(int minDamage, int maxDamage)
    {
        int curDamage = UnityEngine.Random.Range(minDamage, maxDamage);//随机取伤害值
        return curDamage;
    }

    #endregion

    #region Apply Data Change
    public void ApplyHealth(int amount)//回复生命值
    {
        if (CurHealth + amount <= MaxHealth)
        {
            CurHealth += amount;
        }
        else
        {
            CurHealth = MaxHealth;
        }
    }

    public void ApplyHealth(int minAmount, int maxAmount)
    {
        //int curAmount = UnityEngine.Random.Range(minAmount, maxAmount);
        var curAmount = GetRandomValueFrom(minAmount, minAmount, maxAmount);
        if (CurHealth + curAmount <= MaxHealth)
        {
            CurHealth += curAmount;
        }
        else
        {
            CurHealth = MaxHealth;
        }
    }

    public void CostCoins(int price)//花钱
    {
        Coins -= price;
    }

    public void UseKeyToSkillLock(SkillKeyType skillKeyType)//解锁技能
    {
        if (skillKeyType == SkillKeyType.AttackImprove)
        {
            FullCombo = 5;
        }
        else if (skillKeyType == SkillKeyType.DoubleJump)
        {
            JumpAbility = 2;
        }
        else if (skillKeyType == SkillKeyType.Dash)
        {
            CanDash = true;
        }
    }

    public void ChangeMaxHealth(int setMaxHealth)
    {
        if (MaxHealth + setMaxHealth <= 0)
        {
            MaxHealth = 1;
            CurHealth = MaxHealth;
        }
        else
        {
            MaxHealth += setMaxHealth;
            if(CurHealth > MaxHealth)
            {
                CurHealth = MaxHealth;
            }
        }
    }

    public void ChangeATK(int setATK)
    {
        BasicAttackDamage += setATK;
        BasicMinDamage += setATK;
        BasicMaxDamage += setATK;
        FourthAttackDamage += (int)(setATK * 1.3f);
        FourthMinDamage += (int)(setATK * 1.3f);
        FourthMaxDamage += (int)(setATK * 1.3f);
    }

    #endregion
    public static T GetRandomValueFrom<T>(params T[] values)
    {
        var valueArray = values as Array;
        return values[UnityEngine.Random.Range(0, valueArray.Length)];
    }

}

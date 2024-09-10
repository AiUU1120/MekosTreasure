using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public CharacterStats characterStats;

    public float hurtedSpeed;//被攻击后击退速度
    public float hurtedUpSpeed;
    public Vector2 hurtedDirection;//受击方向
    public bool isHited;//受击状态
    public bool isDead;//死亡状态
    protected Rigidbody2D rb;
    protected Collider2D col;
    protected SpriteRenderer rbSprite;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        characterStats = GetComponent<CharacterStats>();
        rbSprite = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    protected virtual void Start()
    {
        characterStats.characterData.curHealth = characterStats.characterData.maxHealth;
        characterStats.IsHited = false;
        characterStats.Hitting = false;
    }



    public void GetHit(Vector2 direction)
    {
        if(transform.localScale.x == direction.x)
        {
            transform.localScale = new Vector3(-direction.x, transform.localScale.y, transform.localScale.z);
        }
        
        this.hurtedDirection = direction;//使面向攻击对象
        GetComponentInChildren<EnemyHpBar>().ShowHp();
        GetComponentInChildren<EnemyHpBar>().ChangeHpBar(characterStats.characterData.curHealth, characterStats.characterData.maxHealth);
    }

    
}

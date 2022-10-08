using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "Attack/EnemyAttackData")]
public class EnemyAttackData_SO : ScriptableObject
{
    [Header("Basic Attack")]
    public float basicAttackRange_Enemy;//基础攻击距离
    public int basicAttackDamage_Enemy;//基础攻击力
    public int basicMinDamage_Enemy;//最小攻击力
    public int basicMaxDamage_Enemy;//最大攻击力
    public float basicCoolDown_Enemy;//基础冷却时间
    public int collisionMinDamage_Enemy;//最小碰撞伤害
    public int collisionMaxDamage_Enemy;//最大碰撞伤害
}

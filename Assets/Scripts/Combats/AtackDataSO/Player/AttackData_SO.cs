using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "Attack/Attack Data")]
public class AttackData_SO : ScriptableObject
{
    [Header("Basic Attack")]
    public int basicAttackDamage;//基础攻击力
    public int basicMinDamage;//最小攻击力
    public int basicMaxDamage;//最大攻击力
    public float criticalMultiplier;//暴击加成百分比
    public float criticalChance;//暴击率

    [Header("Player Skills Attack")]
    public int fourthAttackDamage;//第四下连击攻击力
    public int fourthMinDamage;
    public int fourthMaxDamage;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "Attack/Attack Data")]
public class AttackData_SO : ScriptableObject
{
    [Header("Basic Attack")]
    public int basicAttackDamage;//����������
    public int basicMinDamage;//��С������
    public int basicMaxDamage;//��󹥻���
    public float criticalMultiplier;//�����ӳɰٷֱ�
    public float criticalChance;//������

    [Header("Player Skills Attack")]
    public int fourthAttackDamage;//����������������
    public int fourthMinDamage;
    public int fourthMaxDamage;
}

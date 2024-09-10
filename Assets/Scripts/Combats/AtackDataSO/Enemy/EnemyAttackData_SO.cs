using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "Attack/EnemyAttackData")]
public class EnemyAttackData_SO : ScriptableObject
{
    [Header("Basic Attack")]
    public float basicAttackRange_Enemy;//������������
    public int basicAttackDamage_Enemy;//����������
    public int basicMinDamage_Enemy;//��С������
    public int basicMaxDamage_Enemy;//��󹥻���
    public float basicCoolDown_Enemy;//������ȴʱ��
    public int collisionMinDamage_Enemy;//��С��ײ�˺�
    public int collisionMaxDamage_Enemy;//�����ײ�˺�
}

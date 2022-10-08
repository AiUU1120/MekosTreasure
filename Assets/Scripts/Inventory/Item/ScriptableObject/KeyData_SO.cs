using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SkillKeyType { AttackImprove, DoubleJump, Dash }
[CreateAssetMenu(fileName = "Key Item", menuName = "Inventory/Key Item Data")]
public class KeyData_SO : ScriptableObject
{
    public SkillKeyType skillKeyType;
}

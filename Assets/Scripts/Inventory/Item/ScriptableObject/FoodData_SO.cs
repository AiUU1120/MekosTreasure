using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Food Item", menuName = "Inventory/Food Item Data")]
public class FoodData_SO : ScriptableObject
{
    [Header("===== Health Point =====")]
    public int healthPoint;
    public int minHealthPoint;
    public int maxHealthPoint;
    public bool isRandom;

    [Header("===== Status Point =====")]
    public int setMaxHealth;
    public int setATK;
}

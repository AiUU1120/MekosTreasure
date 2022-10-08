using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Data", menuName = "Character Stats/Data")]
[System.Serializable]public class CharacterData_SO : ScriptableObject
{
    [Header("===== Body Stats =====")]
    public float maxHealth;
    public float curHealth;
    [Range(0, 20)] public int curDefence;//基础防御力
    public float maxSp;
    public float curSp;
    public bool isHited;
    public bool Hitting;
    public float hitedDirection;

    [Header("===== Sundry Data =====")]
    public int coins;
    public int killMinCoins;
    public int killMaxCoins;

    [Header("===== Movement Stats =====")]
    public float runSpeed;
    public float jumpSpeed;

    [Header("===== Skills Key =====")]
    public int fullCombo;//最大连击数
    public int jumpAbility;
    public bool canAttack;
    public bool canDash;


    public void CollectCoins(int minCoins, int maxCoins)
    {
        int curCoins = Random.Range(minCoins, maxCoins);
        coins += curCoins;
        Debug.Log("Now Coins: " + coins);
    }
}

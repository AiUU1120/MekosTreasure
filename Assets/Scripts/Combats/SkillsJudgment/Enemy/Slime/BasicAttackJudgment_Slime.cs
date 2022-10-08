using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttackJudgment_Slime : MonoBehaviour
{
    private CharacterStats characterStats;

    private void Awake()
    {
        characterStats = GetComponentInParent<CharacterStats>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttacked"))
        {
            Debug.Log("Slime Attack!");
            var targetStats = other.GetComponentInParent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats, characterStats.enemyAttackData.basicMinDamage_Enemy, characterStats.enemyAttackData.basicMaxDamage_Enemy);
        }
    }
}

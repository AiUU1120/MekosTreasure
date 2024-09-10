using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackedJudgment : MonoBehaviour
{
    private CharacterStats characterStats;

    private void Awake()
    {
        characterStats = GetComponentInParent<CharacterStats>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyCollision"))
        {
            Debug.Log("Meko be Collided!");
            var attackerStats = other.GetComponentInParent<CharacterStats>();
            characterStats.TakeDamage(attackerStats, characterStats, attackerStats.enemyAttackData.collisionMinDamage_Enemy, attackerStats.enemyAttackData.collisionMaxDamage_Enemy);
        }
    }
}

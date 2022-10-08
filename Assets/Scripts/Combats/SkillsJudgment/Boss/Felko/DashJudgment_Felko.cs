using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashJudgment_Felko : MonoBehaviour
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
            Debug.Log("Felko Dash!");
            var targetStats = other.GetComponentInParent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats, (int)(characterStats.enemyAttackData.basicMinDamage_Enemy * 1.5f), (int)(characterStats.enemyAttackData.basicMaxDamage_Enemy * 1.5f));
        }
    }
}

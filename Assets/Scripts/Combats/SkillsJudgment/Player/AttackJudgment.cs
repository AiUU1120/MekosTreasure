using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackJudgment : MonoBehaviour
{
    private PlayerController player;
    private CharacterStats characterStats;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        characterStats = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterStats>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Enemy") || other.CompareTag("Boss"))
        {
            Debug.Log("Meko Basic Attack!");
            var targetStats = other.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats, characterStats.attackData.basicMinDamage, characterStats.attackData.basicMaxDamage);
        }
    }
}

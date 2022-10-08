using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourthAttackJudgment : MonoBehaviour
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
        if (other.CompareTag("Enemy") || other.CompareTag("Boss"))
        {
            Debug.Log("Meko Fourth Attack!");
            var targetStats = other.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, other.GetComponent<CharacterStats>(), characterStats.attackData.fourthMinDamage, characterStats.attackData.fourthMaxDamage);
        }
    }
}

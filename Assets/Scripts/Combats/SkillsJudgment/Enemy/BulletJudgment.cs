using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletJudgment : MonoBehaviour
{
    [SerializeField] int minDamage;
    [SerializeField] int maxDamage;
    private void OnTriggerEnter2D(Collider2D other)
    {
        //if (other.CompareTag("PlayerAttacked"))
        //{
        //    Debug.Log("SlimeBall Attack!");
        //    var targetStats = other.GetComponentInParent<CharacterStats>();
        //    targetStats.TakeDamage(minDamage, maxDamage, targetStats, dierection);
        //    animator.SetTrigger("Hit");
        //    rb.gravityScale = 0;
        //}
    }
}

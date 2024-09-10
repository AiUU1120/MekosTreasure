using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//此脚本用于出入房间时敌人的刷新
public class EnemyPoint : MonoBehaviour
{
    public GameObject enemyPrefab;
    public bool isFlip;

    public void SpawnEnemy()
    {
        Debug.Log("Spawn Enemy!");
        GameObject enemy = Instantiate(enemyPrefab, transform);
        if (isFlip)
        {
            Debug.Log("Flip");
            enemy.transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}

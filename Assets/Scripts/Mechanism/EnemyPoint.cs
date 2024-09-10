using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//�˽ű����ڳ��뷿��ʱ���˵�ˢ��
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

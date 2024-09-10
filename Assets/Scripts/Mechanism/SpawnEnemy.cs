using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    [SerializeField] GameObject[] enemyPoints;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            SceneController.Instance.ChangeRoomFade();
            foreach (GameObject p in enemyPoints)
            {
                if (p.transform.childCount != 0)
                {
                    Destroy(p.transform.GetChild(0).gameObject);
                }
                else if (p.transform.childCount == 0)
                {
                    p.GetComponent<EnemyPoint>().SpawnEnemy();
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            foreach (GameObject p in enemyPoints)
            {
                if (p.transform.childCount != 0)
                {
                    Destroy(p.transform.GetChild(0).gameObject);
                }
            }
        }
    }
}

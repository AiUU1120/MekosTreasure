using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordTip : MonoBehaviour
{
    void Update()
    {
        if (GameManager.Instance.playStats.characterData.canAttack)
        {
            Destroy(gameObject);
        }
    }
}

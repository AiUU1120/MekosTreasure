using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FelkoWall : MonoBehaviour
{
    private void Update()
    {
        if (PlayerSaveData.Instance.isFelkoBeat)
        {
            Destroy(gameObject);
        }
    }
}

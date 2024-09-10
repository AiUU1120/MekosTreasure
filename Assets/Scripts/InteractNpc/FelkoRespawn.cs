using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FelkoRespawn : Singleton<FelkoRespawn>
{
    [SerializeField]
    private GameObject felkoBossPrefab;

    private void Update()
    {
        if (PlayerSaveData.Instance.isFelkoBeat)
        {
            Destroy(gameObject);
        }
    }

    public void SpawnFelko()
    {
        Instantiate(felkoBossPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}

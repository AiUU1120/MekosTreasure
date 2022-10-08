using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FelkoReplayNpc : MonoBehaviour
{
    [SerializeField]
    private GameObject felkoBossPrefab;
    private PlayerInput input;
    private bool canInteract;

    private void Start()
    {
        input = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (!PlayerSaveData.Instance.isFelkoBeat)
        {
            Destroy(gameObject);
            return;
        }

        if (canInteract && input.Interact)
        {
            SpawnFelko();
        }
    }

    public void SpawnFelko()
    {
        Instantiate(felkoBossPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = false;
        }
    }
}

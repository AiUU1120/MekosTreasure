using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FelkoReactStory : MonoBehaviour
{
    private bool isStorying;

    void Update()
    {
        if (!PlayerSaveData.Instance.isFelkoReact && isStorying)
        {
            OpenStory();
        }

        if (PlayerSaveData.Instance.isFelkoReact && !isStorying)
        {
            Destroy(gameObject);
        }
    }

    private void OpenStory()
    {
        PlayerSaveData.Instance.isFelkoReact = true;
        Invoke("CloseStory", 0.5f);
    }

    private void CloseStory() => isStorying = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isStorying = true;
        }
    }


}

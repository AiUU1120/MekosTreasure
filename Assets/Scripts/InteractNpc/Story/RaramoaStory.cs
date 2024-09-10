using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaramoaStory : MonoBehaviour
{
    private bool isStorying;

    void Update()
    {
        if (!PlayerSaveData.Instance.isGameTutorial && isStorying)
        {
            OpenStory();
        }

        if (PlayerSaveData.Instance.isGameTutorial && !isStorying)
        {
            Destroy(gameObject);
        }
    }

    private void OpenStory()
    {
        PlayerSaveData.Instance.isGameTutorial = true;
        PlayerSaveData.Instance.Save(false);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YuriTownReactStory : MonoBehaviour
{
    private bool isStorying;

    void Update()
    {
        if (!PlayerSaveData.Instance.isYuriTownReact && isStorying)
        {
            OpenStory();
        }

        if (PlayerSaveData.Instance.isYuriTownReact && !isStorying)
        {
            Destroy(gameObject);
        }
    }

    private void OpenStory()
    {
        PlayerSaveData.Instance.isYuriTownReact = true;
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

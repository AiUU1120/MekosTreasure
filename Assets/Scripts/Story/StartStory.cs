using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartStory : MonoBehaviour
{
    private PlayerInput input;

    private Collider2D col;

    private bool isStorying;
    void Start()
    {
        input = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
        col = GetComponent<Collider2D>();
        col.enabled = false;
    }


    void Update()
    {
        if (!PlayerSaveData.Instance.isStart)
        {
            Debug.Log("Lock Input");
            isStorying = true;
            input.DisableAllInputs();
            Invoke("OpenStartStory", 5f);
            PlayerSaveData.Instance.isStart = true;
        }

        if (PlayerSaveData.Instance.isStart && !isStorying)
        {
            Destroy(gameObject);
        }
    }

    private void OpenStartStory()
    {
        col.enabled = true;
        PlayerSaveData.Instance.Save(false);
        Invoke("CloseStartStory", 0.5f);
    }

    private void CloseStartStory()
    {
        col.enabled = false;
        isStorying = false;
    }
}

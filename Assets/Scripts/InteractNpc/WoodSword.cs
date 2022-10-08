using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodSword : MonoBehaviour
{
    private PlayerInput input;
    private bool canPickUp;
    private bool isStorying;

    private void Update()
    {
        if (GameManager.Instance.playStats.characterData.canAttack && !isStorying)
        {
            Destroy(gameObject);
        }

        if (canPickUp && input.Interact)
        {
            isStorying = true;
            GameManager.Instance.playStats.characterData.canAttack = true;
            SaveManager.Instance.playerSaveData.Save(false);
            Invoke("CloseStory", 0.5f);
        }
    }

    private void Start()
    {
        input = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Can Pick");
            canPickUp = true;
        }
    }
    private void CloseStory() => isStorying = false;

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canPickUp = false;
        }
    }

}


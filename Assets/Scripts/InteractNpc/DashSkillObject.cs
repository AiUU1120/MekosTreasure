using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashSkillObject : MonoBehaviour
{
    private PlayerInput input;
    private bool canPickUp;
    [SerializeField]
    private ItemData_SO itemData;

    [SerializeField]
    private AudioData pickAudio;

    private void Update()
    {
        if (GameManager.Instance.playStats.characterData.canDash)
        {
            Destroy(gameObject);
        }

        if (canPickUp && input.Interact)
        {
            AudioManager.Instance.PlaySFX(pickAudio);
            InventoryManager.Instance.inventoryData.AddItem(itemData, itemData.itemAmount);
            InventoryManager.Instance.inventoryUI.ReFreshUI(false);
            Destroy(gameObject);
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
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canPickUp = false;
        }
    }
}

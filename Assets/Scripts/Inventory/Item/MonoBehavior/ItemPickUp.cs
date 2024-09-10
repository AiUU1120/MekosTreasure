using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemType { FOOD, SKILLKEY }
public class ItemPickUp : MonoBehaviour
{
    [SerializeField]
    private ItemData_SO itemData;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager.Instance.inventoryData.AddItem(itemData, itemData.itemAmount);
            InventoryManager.Instance.inventoryUI.ReFreshUI(false);
            Destroy(gameObject);
        }
    }
}

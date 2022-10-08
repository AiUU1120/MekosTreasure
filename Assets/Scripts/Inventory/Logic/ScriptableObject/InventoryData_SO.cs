using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/Inventory Data")]
public class InventoryData_SO : ScriptableObject
{
    private ActionHolder actionHolder;
    public List<InventoryItem> items = new List<InventoryItem>();

    public void AddItem(ItemData_SO newItemData, int amount)
    {
        bool found = false;//是否在背包里寻找到同类物体
        if (newItemData.stackable)
        {
            foreach(var item in items)
            {
                if(item.itemData == newItemData)
                {
                    item.amount += amount;
                    found = true;
                    break;
                }
            }
        }

        for(int i = 0; i < items.Count; i++)
        {
            if(items[i].itemData == null && !found)
            {
                items[i].itemData = newItemData;
                items[i].amount = amount;
                break;
            }
        }
    }
    public void SetItem(ItemData_SO newItemData, int amount)
    {
        actionHolder = FindObjectOfType<ActionHolder>();
        //items[0].theBagItem = theBagItem;
        items[0].itemData = newItemData;
        items[0].amount = amount;
    }
}

[System.Serializable]
public class InventoryItem
{
    public ItemData_SO itemData;

    public int amount;
}

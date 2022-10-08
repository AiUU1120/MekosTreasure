using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHolder : MonoBehaviour
{
    public SlotType slotType;
    public ItemUI itemUI;
    public InventoryItem copyItemData;
    private PlayerInput input;
    [SerializeField]
    private AudioData itemUseSfx;

    //[Header("===== Item Select UI =====")]

    private void Start()
    {
        input = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (input.useAction)
        {
            UseItem();
        }
    }

    public void UseItem()//使用物品
    {
        if (itemUI.GetItemData() != null && itemUI.GetItemData().itemType == ItemType.FOOD && itemUI.Bag.items[itemUI.Index].amount > 0)//当前物品是食物并且数量大于0
        {
            AudioManager.Instance.PlaySFX(itemUseSfx);
            Debug.Log("Use it");
            if (!itemUI.GetItemData().foodData.isRandom)
            {
                GameManager.Instance.playStats.ApplyHealth(itemUI.GetItemData().foodData.healthPoint);
                GameManager.Instance.playStats.ChangeATK(itemUI.GetItemData().foodData.setATK);
                GameManager.Instance.playStats.ChangeMaxHealth(itemUI.GetItemData().foodData.setMaxHealth);
            }
            else
            {
                GameManager.Instance.playStats.ApplyHealth(itemUI.GetItemData().foodData.minHealthPoint, itemUI.GetItemData().foodData.maxHealthPoint);
            }
            itemUI.Bag.items[itemUI.Index].amount -= 1;//使用后数量-1
        }
        UpdateItem();
    }

    //public void GetSetItem(InventoryItem bagItem)//获取设置成快捷使用的物品
    //{
    //    copyItemData = bagItem;
    //    Debug.Log(bagItem.amount);

    //    UpdateItem();
    //}
    public void UpdateItem()
    {
        switch (slotType)
        {
            case SlotType.BAG:
                {
                    itemUI.Bag = InventoryManager.Instance.inventoryData;
                    break;
                }
            case SlotType.WEAPON:
                {
                    break;
                }
            case SlotType.ACTION:
                {
                    itemUI.Bag = InventoryManager.Instance.actionData;
                    break;
                }
        }

        var item = itemUI.Bag.items[itemUI.Index];
        itemUI.SetupItemUI(item.itemData, item.amount);
    }
}

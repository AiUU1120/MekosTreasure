using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    [SerializeField]
    private Image icon = null;
    [SerializeField]
    private TMP_Text amount = null;

    public InventoryData_SO Bag { get; set; }
    public int Index { get; set; } = -1;

    public void SetupItemUI(ItemData_SO item, int itemAmount)//设置格子显示
    {
        if(itemAmount == 0)//物品数量为0时关闭该格子
        {
            Bag.items[Index].itemData = null;
            icon.gameObject.SetActive(false);
            return;
        }

        if (item != null)
        {
            icon.sprite = item.itemIcon;
            amount.text = itemAmount.ToString();
            icon.gameObject.SetActive(true);
        }
        else
        {
            icon.gameObject.SetActive(false);
        }
    }

    public ItemData_SO GetItemData()//返回当前格子物品的ItemData
    {
        return Bag.items[Index].itemData;
    }
}

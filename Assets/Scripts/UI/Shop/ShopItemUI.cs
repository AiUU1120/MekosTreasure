using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    [SerializeField]
    private Image icon = null;
    [SerializeField]
    private TMP_Text itemPrice;
    [SerializeField]
    private TMP_Text itemName;
    [SerializeField]
    private TMP_Text itemInfo;

    public void SetItemUI(ItemData_SO item)
    {
        if (item != null)
        {
            icon.sprite = item.itemIcon;
            icon.gameObject.SetActive(true);

            itemPrice.text = item.itemPrice.ToString();
            itemPrice.gameObject.SetActive(true);

            itemName.text = item.itemName.ToString();
            itemName.gameObject.SetActive(true);

            itemInfo.text = item.itemDescription.ToString();
            itemInfo.gameObject.SetActive(true);
        }
        else
        {
            icon.gameObject.SetActive(false);
            itemPrice.gameObject.SetActive(false);
            itemName.gameObject.SetActive(false);
            itemInfo.gameObject.SetActive(false);
        }
    }
}

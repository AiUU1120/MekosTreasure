using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemPrice : MonoBehaviour
{
    [SerializeField]
    private Text priceNum;
    public void SetItemUIPrice(ItemData_SO item)
    {
        if (item != null)
        {
            priceNum.gameObject.SetActive(true);
            priceNum.text = item.itemPrice.ToString();
        }
        else
        {
            priceNum.gameObject.SetActive(false);
        }
    }
}

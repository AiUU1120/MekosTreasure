using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemHolder : MonoBehaviour
{
    [SerializeField]
    private Button buyItemButton;
    [SerializeField]
    private ItemData_SO itemData;
    [SerializeField]
    private ShopItemUI shopItemUI;
    [SerializeField]
    private ShopItemPrice shopItemPrice;

    [SerializeField]
    private AudioData buySfx;

    private void OnEnable()
    {
        buyItemButton.onClick.AddListener(OnBuyItemButtonClick);
    }

    private void Start()
    {
        shopItemUI.SetItemUI(itemData);
        //shopItemPrice.SetItemUIPrice(itemData);
    }

    private void OnDisable()
    {
        buyItemButton.onClick.RemoveAllListeners();
    }

    private void OnBuyItemButtonClick()
    {
        if (GameManager.Instance.playStats.Coins >= itemData.itemPrice)//ÂòµÃÆð
        {
            AudioManager.Instance.PlaySFX(buySfx);
            GameManager.Instance.playStats.CostCoins(itemData.itemPrice);
            InventoryManager.Instance.inventoryData.AddItem(itemData, 1);
            InventoryManager.Instance.inventoryUI.ReFreshUI(false);
            InputDeviceDetection.GameplayUIController.Instance.OpenSuccessCanvas();
        }
        else
        {
            InputDeviceDetection.GameplayUIController.Instance.OpenFailCanvas();
        }
    }
}

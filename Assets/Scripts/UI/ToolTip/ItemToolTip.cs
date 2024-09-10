using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemToolTip : MonoBehaviour
{
    [SerializeField]
    private TMP_Text itemNameText;
    [SerializeField]
    private TMP_Text itemInfoText;

    public void SetUpItemTip(ItemData_SO item)
    {
        itemNameText.text = item.itemName;
        itemInfoText.text = item.itemDescription;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item Data")]
public class ItemData_SO : ScriptableObject
{
    public ItemType itemType;

    public string itemName;

    public Sprite itemIcon;

    public int itemAmount;

    public bool stackable;//¿É¶Ñµþ
    [TextArea]
    public string itemDescription = "";

    [Header("===== Food Item =====")]
    public FoodData_SO foodData;

    [Header("===== Skill Key Item =====")]
    public KeyData_SO keyData;

    [Header("===== Shop Data")]
    public int itemPrice;
}

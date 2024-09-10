using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    //TODO:������ģ��洢����
    [Header("===== Inventory Data =====")]
    public InventoryData_SO inventoryData;
    public InventoryData_SO actionData;
    public InventoryData_SO inventoryTemplate;
    public InventoryData_SO actionTemplate;

    [Header("ContainerS")]
    public ContainerUI inventoryUI;
    public ContainerUI actionUI;

    protected override void Awake()
    {
        base.Awake();
        if(inventoryTemplate != null)
        {
            inventoryData = Instantiate(inventoryTemplate);
        }
        if (actionTemplate != null)
        {
            actionData = Instantiate(actionTemplate);
        }
    }

    private void Start()
    {
        inventoryUI.ReFreshUI(false);
        actionUI.ReFreshUI(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerUI : MonoBehaviour
{
    [SerializeField]
    private SlotHolder[] slotHolders;
    [SerializeField]
    private ActionHolder[] actionHolders;
    [SerializeField]
    private ShopItemHolder[] shopItemHolders;

    public void ReFreshUI(bool isAction)
    {
        if (!isAction)
        {
            for (int i = 0; i < slotHolders.Length; i++)
            {
                //Debug.Log(i);
                slotHolders[i].itemUI.Index = i;
                slotHolders[i].UpdateItem();
            }
        }
        else
        {
            for(int i = 0; i < actionHolders.Length; i++)
            {
                actionHolders[i].itemUI.Index = i;
                actionHolders[i].UpdateItem();
            }
        }
    }
}

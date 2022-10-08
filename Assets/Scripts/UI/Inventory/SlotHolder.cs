using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;


public enum SlotType { BAG, WEAPON, ACTION }
public class SlotHolder : MonoBehaviour
{
    public SlotType slotType;
    public ItemUI itemUI;
    private PlayerInput input;
    private ActionHolder actionHolder;

    [Header("===== Item Select UI =====")]
    [SerializeField]
    private Button itemSelectButton;
    [SerializeField]
    private Canvas itemSelectMenu;
    [SerializeField]
    private Button useItemButton;
    [SerializeField]
    private Button setActionButton;
    [SerializeField]
    private RectTransform itemSelectRect;
    [SerializeField]
    private GameObject itemSelectMenuFirstButton;
    [SerializeField]
    private GameObject lastSelectButton;
    [SerializeField]
    private ItemToolTip itemToolTip;
    private bool isSelectMenu;
    [SerializeField]
    private AudioData normalClickSfx;
    [SerializeField]
    private AudioData itemUseSfx;
    [SerializeField]
    private AudioData itemSetSfx;
    [SerializeField]
    private AudioData itemfailSfx;

    //private bool isSetAction;


    private void OnEnable()
    {
        itemSelectButton.onClick.AddListener(OnItemSelectButtonClick);
        useItemButton.onClick.AddListener(OnUseItemButtonClick);
        setActionButton.onClick.AddListener(OnSetActionButtonClick);
    }

    private void Start()
    {
        input = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
    }
    private void OnDisable()
    {
        itemSelectButton.onClick.RemoveAllListeners();
        useItemButton.onClick.RemoveAllListeners();
        setActionButton.onClick.RemoveAllListeners();
    }

    private void Update()
    {
        if (input.goBack && isSelectMenu)
        {
            CloseSelectMenu();
        }
    }

    private void OnItemSelectButtonClick()//ѡ�񵥸���Ʒ
    {
        AudioManager.Instance.PlaySFX(normalClickSfx);
        if (itemUI.GetItemData() != null)
        {
            //Debug.Log("Open Item");
            OpenSelectMenu();
        }
    }

    private void OnUseItemButtonClick()//����ʹ�ð�ť
    {
        Debug.Log("Use Item");
        UseItem();
    }
    private void OnSetActionButtonClick()//�������ð�ť
    {
        Debug.Log("Set Action");
        SetAction();
    }

    private void OpenSelectMenu()//����Ʒѡ��˵�
    {
        AudioManager.Instance.PlaySFX(normalClickSfx);
        isSelectMenu = true;
        InputDeviceDetection.GameplayUIController.Instance.isSecondaryMenu = true;
        itemToolTip.SetUpItemTip(itemUI.Bag.items[itemUI.Index].itemData);
        //itemSelectMenu.SetActive(true);
        itemSelectMenu.enabled = true;
        itemSelectRect.localScale = Vector3.zero;
        if (InputDeviceDetection.GameplayUIController.Instance.isPad)
        {
            EventSystem.current.SetSelectedGameObject(itemSelectMenuFirstButton);
        }
        itemSelectRect.DOScale(Vector3.one, 0.1f);
    }

    private void CloseSelectMenu()//�ر�ѡ��˵�
    {
        AudioManager.Instance.PlaySFX(normalClickSfx);
        StartCoroutine(CloseMenu());
    }

    IEnumerator CloseMenu()//Э�̹رղ˵�
    {
        isSelectMenu = false;
        itemSelectRect.DOScale(Vector3.zero, 0.1f);
        yield return new WaitForSeconds(0.1f);
        InputDeviceDetection.GameplayUIController.Instance.isSecondaryMenu = false;
        //itemSelectMenu.SetActive(false);
        itemSelectMenu.enabled = false;
        EventSystem.current.SetSelectedGameObject(lastSelectButton);
        yield break;
    }

    public void UseItem()//ʹ����Ʒ
    {
        AudioManager.Instance.PlaySFX(itemUseSfx);
        if (itemUI.GetItemData() != null && itemUI.GetItemData().itemType == ItemType.FOOD && itemUI.Bag.items[itemUI.Index].amount > 0)//��ǰ��Ʒ��ʳ�ﲢ����������0
        {
            //Debug.Log("Use it");
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
            itemUI.Bag.items[itemUI.Index].amount -= 1;//ʹ�ú�����-1
        }
        if (itemUI.GetItemData() != null && itemUI.GetItemData().itemType == ItemType.SKILLKEY && itemUI.Bag.items[itemUI.Index].amount > 0)//��ǰ��Ʒ�Ǽ���Կ�ײ�����������0
        {
            //Debug.Log("Use it");
            GameManager.Instance.playStats.UseKeyToSkillLock(itemUI.GetItemData().keyData.skillKeyType);
            itemUI.Bag.items[itemUI.Index].amount -= 1;//ʹ�ú�����-1
        }
        UpdateItem();
        CloseSelectMenu();
    }

    public void SetAction()
    {
        if (itemUI.GetItemData() != null && itemUI.GetItemData().itemType == ItemType.FOOD && itemUI.Bag.items[itemUI.Index].amount > 0)
        {
            AudioManager.Instance.PlaySFX(itemSetSfx);
            actionHolder = FindObjectOfType<ActionHolder>();
            Debug.Log("Set it");
            //isSetAction = true;
            //������Ʒ��������
            var targetItem = actionHolder.itemUI.Bag.items[0];
            var tempItem = itemUI.Bag.items[itemUI.Index];

            bool isSameItem = tempItem.itemData == targetItem.itemData;
            if (isSameItem && targetItem.itemData.stackable)//���������붯������Ʒ
            {
                Debug.Log("is Same Item");
                targetItem.amount += tempItem.amount;
                tempItem.itemData = null;
                tempItem.amount = 0;
            }
            else
            {
                Debug.Log("Trans it");
                itemUI.Bag.items[itemUI.Index] = targetItem;
                actionHolder.itemUI.Bag.items[0] = tempItem;
            }
            UpdateItem();
            actionHolder.UpdateItem();
            CloseSelectMenu();
        }
        else
        {
            AudioManager.Instance.PlaySFX(itemfailSfx);
        }
    }
    public void UpdateItem()//ˢ����Ʒ
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanelUIController : MonoBehaviour
{
    public GameObject ActivePanel;

    [Header("Panels")]
    public GameObject BackInfoPanel;
    public GameObject SplittingPanel;
    public GameObject EatFoodPanel;

    [Header("General")]
    public Text ItemNameText;

    [Header("Eat Food Panel Cache")]
    public Image EatFoodItemSprite;
    public Text EatFoodItemInfoText;
    public Text EatFoodHealingAmountText;
    public Text EatFoodCaloriesAmountText;
    public Text EatFoodHydrationAmountText;
    public Button EatFoodButton;
    public Button DropFoodButton;

    private InventoryManager invMan;

    private void Start()
    {
        invMan = InventoryManager.Instance;
        ResetAllPanels();
    }

    private void Update()
    {
        //ChangeActivePanel();
    }

    public void ChangeActivePanel()
    {
        if (!invMan.CurrentlySelectedSlot) return;

        var curHeldItem = invMan.CurrentlySelectedSlot.ItemHolder.CurrentlyHeldItem;

        if (!curHeldItem) return;

        switch (curHeldItem.ItemBaseInfo.itemType)
        {
            case ItemBaseInfo.ItemType.Consumable:

                BackInfoPanel.SetActive(true);
                ActivePanel = EatFoodPanel;
                EatFoodPanel.SetActive(true);

                Debug.Log("Opening Consumable Type Info Panel");

                break;
            case ItemBaseInfo.ItemType.Weapon:

                Debug.Log("Opening Weapon Type Info Panel");
                break;
            case ItemBaseInfo.ItemType.Armor:

                Debug.Log("Opening Armor Type Info Panel");
                break;
            case ItemBaseInfo.ItemType.Placeable:

                Debug.Log("Opening Placeable Type Info Panel");
                break;
            case ItemBaseInfo.ItemType.Resource:

                Debug.Log("Opening Resource Type Info Panel");
                break;
            default:
                ResetAllPanels();

                break;
        }

        PopulatePanelData(curHeldItem.ItemBaseInfo.itemType, curHeldItem);
    }

    private void PopulatePanelData(ItemBaseInfo.ItemType itemType, Item curHeldItem)
    {
        switch (itemType)
        {
            case ItemBaseInfo.ItemType.Consumable:

                ItemNameText.text = curHeldItem.ItemBaseInfo.ItemName;
                EatFoodItemSprite.sprite = curHeldItem.ItemBaseInfo.ItemSprite;
                EatFoodItemInfoText.text = curHeldItem.ItemBaseInfo.ItemDescription;
                EatFoodHealingAmountText.text = "+" + curHeldItem.ItemTypeInfo.GetType().GetField("GiveHealthAmount").GetValue(curHeldItem.ItemTypeInfo);
                EatFoodCaloriesAmountText.text = "+" + curHeldItem.ItemTypeInfo.GetType().GetField("GiveCaloriesAmount").GetValue(curHeldItem.ItemTypeInfo);
                EatFoodHydrationAmountText.text = "+" + curHeldItem.ItemTypeInfo.GetType().GetField("GiveHydrationAmount").GetValue(curHeldItem.ItemTypeInfo);

                EatFoodButton.onClick.AddListener(() => InventoryManager.Instance.UseItem(itemType, curHeldItem));
                DropFoodButton.onClick.AddListener(() => InventoryManager.Instance.DropItem());
                Debug.Log("Populating Consumable Type Info Panel");

                break;
            case ItemBaseInfo.ItemType.Weapon:

                Debug.Log("Populating Weapon Type Info Panel");
                break;
            case ItemBaseInfo.ItemType.Armor:

                Debug.Log("Populating Armor Type Info Panel");
                break;
            case ItemBaseInfo.ItemType.Placeable:

                Debug.Log("Populating Placeable Type Info Panel");
                break;
            case ItemBaseInfo.ItemType.Resource:

                Debug.Log("Populating Resource Type Info Panel");
                break;
            default:
                ResetAllPanels();

                break;
        }
    }

    public void ResetAllPanels()
    {
        Debug.Log("Reseting All Panels!");

        ActivePanel = null;

        BackInfoPanel.SetActive(false);
        EatFoodPanel.SetActive(false);
    }
}

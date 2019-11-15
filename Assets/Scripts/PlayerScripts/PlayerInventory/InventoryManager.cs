using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public InventorySlot CurrentlySelectedSlot;
    public InventorySlot HoveringOverSlot;
    [Header("Items Inside Inventory")]
    public List<Item> HoldingItems;

    [Header("Cache")]
    public InventorySlot[] InventorySlots;
    public InventorySlot[] HotBarSlots;
    public AudioSource audioSource;
    public ItemHolder TempHolder;
    public InfoPanelUIController InfoPanelUIController;

    public float uiVolume; //move
    public ItemHolder deductedHolder;
    public bool reset = false;
    public InventorySlot DropSlot;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        audioSource = GetComponent<AudioSource>();

        InvokeRepeating("GetCurrentlySelectSlot", 0.1f, 0.1f);
    }

    private void Update()
    {
        PickUpItem(PlayerSelection.Instance.ObjectInView);
    }

    private InventorySlot GetCurrentlySelectSlot()
    {
        if (!EventSystem.current.currentSelectedGameObject && reset || CurrentlySelectedSlot && !CurrentlySelectedSlot.Occupied && reset)
        {
            InfoPanelUIController.ResetAllPanels();
            CurrentlySelectedSlot = null;
            reset = false;
            return null;
        }
        else if (EventSystem.current.currentSelectedGameObject)
        {
            return CurrentlySelectedSlot = EventSystem.current.currentSelectedGameObject.GetComponent<InventorySlot>();
        }

        return null;
    }

    public void PickUpItem(Item itemInView)
    {
        if (!itemInView) return;

        if (Input.GetKeyDown(InputManager.Instance.InputKeyManager.InteractableKey))
        {
            if (HoldingItems.Contains(itemInView)) return;

            if (itemInView.GetComponent<IInteractable>() != null)
                itemInView.GetComponent<IInteractable>().OnPickedUp();
            audioSource.PlayOneShot(itemInView.ItemBaseInfo.PickUpItemSound, 0.1f);

            PlayerSelection.Instance.SelectableObjects.Remove(itemInView.gameObject);
            HoldingItems.Add(itemInView);

            itemInView.gameObject.SetActive(false);
            itemInView.transform.SetParent(transform);
            itemInView.transform.position = transform.position;
            itemInView.transform.rotation = Quaternion.identity;

            UpdatePlayerInventory(itemInView);
        }
    }

    private void UpdatePlayerInventory(Item itemInView)
    {
        for (int i = 0; i < InventorySlots.Length; i++)
        {
            var slot = InventorySlots[i];
            var itemHolder = slot.GetComponentInChildren<ItemHolder>();

            var leftOver = 0; //left over item after the stack is full in the slot

            if (!slot.Occupied) //adding first item based on if a slot is not occupied
            {
                itemHolder.CurrentlyHeldItem = itemInView;
                itemHolder.ItemAmount += itemInView.ItemAmount;

                slot.Occupied = true;
                Debug.Log("Adding Into a New Slot");
                break;
            }

            if (slot.Occupied && itemHolder.CurrentlyHeldItem.ItemTypeInfo.Equals(itemInView.ItemTypeInfo)
                && isAbleToStack(itemHolder) && !itemHolder.ItemAmount.Equals(itemHolder.CurrentlyHeldItem.ItemBaseInfo.StackLimit)) 
            {
                if (hasLeftOver(itemHolder, itemInView, out leftOver)) //check if stack has a left over, then move the left over to the next unoccupied slot
                {
                    var stackAmmount = (itemInView.ItemAmount + itemHolder.ItemAmount) - leftOver;

                    itemHolder.ItemAmount = stackAmmount;

                    var nextSlot = GetUnoccupiedSlot();
                    var nextItemHolder = nextSlot.GetComponentInChildren<ItemHolder>();

                    nextItemHolder.CurrentlyHeldItem = itemInView;
                    nextItemHolder.ItemAmount = leftOver;

                    nextSlot.Occupied = true;

                    Debug.Log("Combining Stack");
                    break;
                }

                itemHolder.ItemAmount += itemInView.ItemAmount; //stacking 
                Debug.Log("Stacking");
                break;
            }
        }
        UpdateInventoryItems();
    }

    private void UpdateInventoryItems()
    {
        for (int i = 0; i < InventorySlots.Length; i++)
        {
            var slot = InventorySlots[i];

            for (int z = 0; z < HoldingItems.Count; z++)
            {
                var item = HoldingItems[z];

                if (slot.Occupied && slot.ItemHolder.CurrentlyHeldItem.Equals(item)) //checks occupied slots if the item being picked up is the same as the holding item and sets its item count based on a new one
                {
                    item.ItemAmount = slot.ItemHolder.ItemAmount;
                }
            }
        }
    }

    public void DropItem()
    {
        for (int i = 0; i < HoldingItems.Count; i++)
        {
            var item = HoldingItems[i];

            if(item.Equals(DropSlot.ItemHolder.CurrentlyHeldItem))
            {
                var cameraForward = transform.root.GetComponentInChildren<Camera>().transform.forward;

                item.transform.position = transform.root.GetComponentInChildren<Camera>().ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, 1));
                item.gameObject.SetActive(true);
                item.GetComponent<Rigidbody>().AddForce(transform.parent.forward * 10, ForceMode.Impulse);
                item.transform.parent = null;

                HoldingItems.Remove(item);

                audioSource.PlayOneShot(DropSlot.ItemHolder.CurrentlyHeldItem.ItemBaseInfo.DropItemSound, 0.1f);
                break;
            }
        }

        DropSlot.Occupied = false;
        DropSlot.ItemHolder.ItemAmount = 0;
        DropSlot.ItemHolder.CurrentlyHeldItem = null;

        InfoPanelUIController.ResetAllPanels();
    }

    //FINISH LATER
    public void GrabSingleItemFromStack(InventorySlot onDropSlot) //right click
    {
        TempHolder.transform.position = Input.mousePosition;

        if (TempHolder.CurrentlyHeldItem || !HoveringOverSlot) return;

        if(HoveringOverSlot.ItemHolder.ItemAmount >= 1)
        {
            HoveringOverSlot.ItemHolder.ItemAmount -= 1;

            TempHolder.CurrentlyHeldItem = HoveringOverSlot.ItemHolder.CurrentlyHeldItem;
            TempHolder.ItemAmount = 1;

            deductedHolder = HoveringOverSlot.ItemHolder;

            if (HoveringOverSlot.ItemHolder.ItemAmount == 0)
            {
                HoveringOverSlot.ItemHolder.CurrentlyHeldItem = null;
                HoveringOverSlot.Occupied = false;
            }
        }
    }
    public void DropSingleItemOntoSlot(InventorySlot onDropSlot)
    {
        if(!onDropSlot.Occupied)
        {
            onDropSlot.ItemHolder.CurrentlyHeldItem = TempHolder.CurrentlyHeldItem;
            onDropSlot.ItemHolder.ItemAmount += TempHolder.ItemAmount;
            onDropSlot.Occupied = true;

            ResetItemHolder(TempHolder);
            deductedHolder = null;
        }
        else if(onDropSlot.Occupied && sameItemType(onDropSlot.ItemHolder, TempHolder) && onDropSlot.ItemHolder.ItemAmount != onDropSlot.ItemHolder.CurrentlyHeldItem.ItemBaseInfo.StackLimit)
        {
            onDropSlot.ItemHolder.ItemAmount += 1;

            ResetItemHolder(TempHolder);
        }
        else if (onDropSlot.Occupied && !sameItemType(onDropSlot.ItemHolder, TempHolder)) //fix 
        {
            deductedHolder.ItemAmount += 1;

            ResetItemHolder(TempHolder);
        }

        //if (sameItemType(holder, onDropSlot.ItemHolder) && onDropSlot.ItemHolder.ItemAmount < onDropSlot.ItemHolder.CurrentlyHeldItem.ItemBaseInfo.StackLimit)
        //{
        //    Debug.Log("SameType&Stackable");
        //}
    }
    private void ResetItemHolder(ItemHolder itemHolder)
    {
        itemHolder.ItemAmount = 0;
        itemHolder.CurrentlyHeldItem = null;
    }
    public void GrabHalfStack(ItemHolder _itemHolder, InventorySlot onDropSlot) //middle click
    {
        
    }

    public void MoveItem(ItemHolder _itemHolder, InventorySlot invSlot)
    {
        if (!CurrentlySelectedSlot.ItemHolder.CurrentlyHeldItem) return;

        _itemHolder.ItemAmount = CurrentlySelectedSlot.ItemHolder.ItemAmount;
        _itemHolder.CurrentlyHeldItem = CurrentlySelectedSlot.ItemHolder.CurrentlyHeldItem;

        if(_itemHolder.CurrentlyHeldItem)
            audioSource.PlayOneShot(_itemHolder.CurrentlyHeldItem.ItemBaseInfo.DropSound, uiVolume); //play sound

        CurrentlySelectedSlot.ItemHolder.ItemAmount = 0;
        CurrentlySelectedSlot.ItemHolder.CurrentlyHeldItem = null;
        CurrentlySelectedSlot.Occupied = false;

        invSlot.Occupied = true;
    }
    public void CombineItems(ItemHolder _itemHolder, InventorySlot onDropSlot)
    {
        if(!sameItemType(_itemHolder, CurrentlySelectedSlot.ItemHolder)) //check if same type of item & not already a full stack
        {
            SwapItems(_itemHolder, onDropSlot);

            return;
        }

        var tilFullStack = 0;

        if(fitsIntoStack(_itemHolder, CurrentlySelectedSlot.ItemHolder, out tilFullStack))
        {
            _itemHolder.ItemAmount += CurrentlySelectedSlot.ItemHolder.ItemAmount;
            CurrentlySelectedSlot.ItemHolder.ItemAmount = 0;
            CurrentlySelectedSlot.ItemHolder.CurrentlyHeldItem = null;

            CurrentlySelectedSlot.Occupied = false;
        }
        else
        {
            if(CurrentlySelectedSlot.ItemHolder.ItemAmount >= _itemHolder.ItemAmount || CurrentlySelectedSlot.ItemHolder.ItemAmount <= _itemHolder.ItemAmount)
            {
                var leftOver = _itemHolder.CurrentlyHeldItem.ItemBaseInfo.StackLimit - _itemHolder.ItemAmount;

                _itemHolder.ItemAmount += leftOver;
                CurrentlySelectedSlot.ItemHolder.ItemAmount -= leftOver;
            }
        }

        //play sound
    }
    private void SwapItems(ItemHolder _itemHolder, InventorySlot onDropSlot)
    {
        var tempItem = _itemHolder.CurrentlyHeldItem;
        var tempAmount = _itemHolder.ItemAmount;

        _itemHolder.CurrentlyHeldItem = CurrentlySelectedSlot.ItemHolder.CurrentlyHeldItem;
        _itemHolder.ItemAmount = CurrentlySelectedSlot.ItemHolder.ItemAmount;

        CurrentlySelectedSlot.ItemHolder.CurrentlyHeldItem = tempItem;
        CurrentlySelectedSlot.ItemHolder.ItemAmount = tempAmount;

        audioSource.PlayOneShot(_itemHolder.CurrentlyHeldItem.ItemBaseInfo.DropSound, 0.1f); //play sound

        tempItem = null;
        tempAmount = 0;
    }

    public void UseItem(ItemBaseInfo.ItemType itemType, Item itemToUse)
    {
        switch (itemType)
        {
            case ItemBaseInfo.ItemType.Consumable:

                break;
            case ItemBaseInfo.ItemType.Weapon:

                break;
            case ItemBaseInfo.ItemType.Armor:

                break;
            case ItemBaseInfo.ItemType.Placeable:

                break;
            case ItemBaseInfo.ItemType.Resource:

                break;
        }
    }

    public float GetUIVolumeNormalized(int value) //move
    {
        var uiVolMax = 0.5f;
        return uiVolume = (value * uiVolMax) / 100;
    }

    #region Helper Functions
    private InventorySlot GetUnoccupiedSlot()
    {
        for (int i = 0; i < InventorySlots.Length; i++)
        {
            var slot = InventorySlots[i];

            if (!slot.Occupied) return slot;
        }

        return null;
    }
    private bool isAbleToStack (ItemHolder itemHolder)
    {
        return itemHolder.CurrentlyHeldItem.ItemBaseInfo.StackLimit > 1;
    }
    private bool canFitStack(ItemHolder itemHolder, ItemHolder dropingHolder)
    {
        return itemHolder.ItemAmount + dropingHolder.ItemAmount <= dropingHolder.CurrentlyHeldItem.ItemBaseInfo.StackLimit;
    }
    private bool hasLeftOver(ItemHolder itemHolder, Item itemInView, out int leftOver)
    {
        leftOver = itemInView.ItemAmount + itemHolder.ItemAmount - itemInView.ItemBaseInfo.StackLimit;

        return itemInView.ItemAmount + itemHolder.ItemAmount > itemInView.ItemBaseInfo.StackLimit;
    }
    private bool fitsIntoStack(ItemHolder itemHolder, ItemHolder dropingHolder, out int amountTilFull)
    {
        amountTilFull = itemHolder.CurrentlyHeldItem.ItemBaseInfo.StackLimit - (itemHolder.ItemAmount + dropingHolder.ItemAmount);

        return itemHolder.ItemAmount + dropingHolder.ItemAmount <= itemHolder.CurrentlyHeldItem.ItemBaseInfo.StackLimit;
    }
    private bool sameItemType(ItemHolder itemHolder, ItemHolder dropingHolder)
    {
        return itemHolder.CurrentlyHeldItem.ItemTypeInfo == dropingHolder.CurrentlyHeldItem.ItemTypeInfo;
    }
    #endregion
}

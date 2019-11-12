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

    public float uiVolume; //move

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
        if (!EventSystem.current.currentSelectedGameObject)
        {
            CurrentlySelectedSlot = null;
            return null;
        }

        return CurrentlySelectedSlot = EventSystem.current.currentSelectedGameObject.GetComponent<InventorySlot>();
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
                && isStackable(itemHolder) && !itemHolder.ItemAmount.Equals(itemHolder.CurrentlyHeldItem.ItemBaseInfo.StackLimit)) 
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

    }

    public void MoveItem(ItemHolder _itemHolder, InventorySlot invSlot)
    {
        if (!CurrentlySelectedSlot.ItemHolder.CurrentlyHeldItem) return;

        _itemHolder.ItemAmount = CurrentlySelectedSlot.ItemHolder.ItemAmount;
        _itemHolder.CurrentlyHeldItem = CurrentlySelectedSlot.ItemHolder.CurrentlyHeldItem;

        if(_itemHolder.CurrentlyHeldItem)
            audioSource.PlayOneShot(_itemHolder.CurrentlyHeldItem.ItemBaseInfo.DropSound, uiVolume);

        CurrentlySelectedSlot.ItemHolder.ItemAmount = 0;
        CurrentlySelectedSlot.ItemHolder.CurrentlyHeldItem = null;
        CurrentlySelectedSlot.Occupied = false;

        invSlot.Occupied = true;
    }

    public void CombineItems(ItemHolder _itemHolder, InventorySlot onDropSlot)
    {
        if(_itemHolder.CurrentlyHeldItem.ItemBaseInfo.itemType != CurrentlySelectedSlot.ItemHolder.CurrentlyHeldItem.ItemBaseInfo.itemType
            && _itemHolder.ItemAmount != CurrentlySelectedSlot.ItemHolder.CurrentlyHeldItem.ItemBaseInfo.StackLimit) //check if same type of item & not already a full stack
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

            Debug.Log("FitsIntoStack, " + tilFullStack);
        }
        else
        {
            if(CurrentlySelectedSlot.ItemHolder.ItemAmount > _itemHolder.ItemAmount)
            {
                var leftOver = CurrentlySelectedSlot.ItemHolder.ItemAmount - _itemHolder.ItemAmount;

                _itemHolder.ItemAmount += leftOver;
                CurrentlySelectedSlot.ItemHolder.ItemAmount -= leftOver;


            }

            Debug.Log("DoesNotFitIntoStack");
        }
    }

    private void SwapItems(ItemHolder _itemHolder, InventorySlot onDropSlot)
    {
        var tempItem = _itemHolder.CurrentlyHeldItem;
        var tempAmount = _itemHolder.ItemAmount;

        _itemHolder.CurrentlyHeldItem = CurrentlySelectedSlot.ItemHolder.CurrentlyHeldItem;
        _itemHolder.ItemAmount = CurrentlySelectedSlot.ItemHolder.ItemAmount;

        CurrentlySelectedSlot.ItemHolder.CurrentlyHeldItem = tempItem;
        CurrentlySelectedSlot.ItemHolder.ItemAmount = tempAmount;
    }

    public void DropItem(Item itemToDrop)
    {
        audioSource.PlayOneShot(itemToDrop.ItemBaseInfo.DropItemSound, 0.1f);

    }

    public void UseItem(Item itemToUse)
    {

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
    private bool isStackable (ItemHolder itemHolder)
    {
        return itemHolder.CurrentlyHeldItem.ItemBaseInfo.StackLimit > 1;
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
    #endregion
}

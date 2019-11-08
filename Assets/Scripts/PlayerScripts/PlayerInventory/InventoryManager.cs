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

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

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

            PlayerSelection.Instance.SelectableObjects.Remove(itemInView.gameObject);
            HoldingItems.Add(itemInView);

            itemInView.gameObject.SetActive(false);
            itemInView.transform.SetParent(transform);

            if (itemInView.GetComponent<IInteractable>() != null)
                itemInView.GetComponent<IInteractable>().OnPickedUp();
        }
    }

    public void DropItem(Item itemToDrop)
    {

    }

    public void UseItem(Item itemToUse)
    {

    }

    private void DragItemsInInventory()
    {
        if (!CurrentlySelectedSlot) return;

        var originalPos = new Vector3(0, 0, 0);
        var itemHolder = CurrentlySelectedSlot.ItemHolder;

        if (Input.GetKey(KeyCode.Mouse0)) 
        {
            itemHolder.transform.position = Input.mousePosition;
            itemHolder.beingDragged = true;
        }
        else
        {
            itemHolder.transform.localPosition = originalPos;
            itemHolder.beingDragged = false;
        }
    }

}

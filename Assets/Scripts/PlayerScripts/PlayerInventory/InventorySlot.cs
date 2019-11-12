using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public ItemHolder ItemHolder;
    public bool Occupied;

    private ItemHolder tempHolder;

    public void OnPointerEnter(PointerEventData eventData) //slot animation & sound
    {
        iTween.PunchScale(gameObject, new Vector3(.1f, .1f, .1f), .5f);
        InventoryUIController.Instance.PlayOnHoverSlotSound(.02f);
        InventoryManager.Instance.HoveringOverSlot = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryManager.Instance.HoveringOverSlot = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Occupied && ItemHolder.CurrentlyHeldItem)
        {
            InventoryManager.Instance.audioSource.PlayOneShot(ItemHolder.CurrentlyHeldItem.ItemBaseInfo.SelectSound, InventoryManager.Instance.uiVolume);
        }

    }
    public bool playedOnce;

    public void OnDrag(PointerEventData eventData) //item drag handling
    {
        tempHolder = ItemHolder;

        if (!tempHolder.CurrentlyHeldItem) return;

        tempHolder.canvas.sortingOrder = 6;
        tempHolder.beingDragged = true;
        tempHolder.transform.position = Input.mousePosition;

        if (InventoryManager.Instance.audioSource.isPlaying || playedOnce) return;

        InventoryManager.Instance.audioSource.PlayOneShot(tempHolder.CurrentlyHeldItem.ItemBaseInfo.DragSound, InventoryManager.Instance.uiVolume);
        playedOnce = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        tempHolder.beingDragged = false;
        tempHolder.transform.localPosition = Vector3.zero;
        tempHolder.canvas.sortingOrder = 5;

        tempHolder = null;
        playedOnce = false;

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint((RectTransform)transform, Input.mousePosition))
        {
            if (!Occupied)
            {
                InventoryManager.Instance.MoveItem(ItemHolder, this);
                Debug.Log(gameObject);
            }
            else
                InventoryManager.Instance.CombineItems(ItemHolder, this); //includes the swaping of items method

        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IEndDragHandler
{
    public ItemHolder ItemHolder;

    private ItemHolder tempHolder;
    public RectTransform parentRect;

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

    public void OnDrag(PointerEventData eventData) //item drag handling
    {
        tempHolder = ItemHolder;

        tempHolder.beingDragged = true;
        tempHolder.transform.position = Input.mousePosition;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        tempHolder.beingDragged = false;
        tempHolder.transform.localPosition = Vector3.zero;

        tempHolder = null;
    }

}

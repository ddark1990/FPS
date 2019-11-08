using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ItemHolder ItemHolder;

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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler
{


    public void OnPointerEnter(PointerEventData eventData) //slot animation & sound
    {
        iTween.PunchScale(gameObject, new Vector3(.1f, .1f, .1f), .5f);
        InventoryUIController.Instance.PlayOnHoverSlotSound(.02f);
    }
}

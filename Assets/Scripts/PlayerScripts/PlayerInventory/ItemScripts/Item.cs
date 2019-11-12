using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    public int ItemAmount = 1;

    public ItemBaseInfo ItemBaseInfo;
    public ScriptableObject ItemTypeInfo;

    private void Update()
    {
        ItemAmount = Mathf.Clamp(ItemAmount, 1, ItemBaseInfo.StackLimit);
    }

    #region Interface Info

    public void OnDroped()
    {
        Debug.Log("Dropped " + ItemBaseInfo.ItemName);
    }
    public void OnEat()
    {
        Debug.Log("Ate " + ItemBaseInfo.ItemName);
    }
    public void OnInteract()
    {
        Debug.Log("Interacted with " + ItemBaseInfo.ItemName);
    }
    public void OnPickedUp()
    {
        Debug.Log("Picked Up " + ItemBaseInfo.ItemName);
    }

    #endregion
}

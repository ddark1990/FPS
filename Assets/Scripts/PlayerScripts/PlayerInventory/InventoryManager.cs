using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<Item> HoldingItems;
    public InventorySlot[] InvetorySlots;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
}

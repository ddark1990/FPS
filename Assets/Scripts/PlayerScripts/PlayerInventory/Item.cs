using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "CreateItem", order = 1)]
public class Item : ScriptableObject
{
    public enum ItemType { Consumable, Weapon, Armor, Resource, Placeable }
    [Header("Item Type")]
    public ItemType itemType;

    [Header("Item Information")]
    public string ItemName;
    [TextArea] public string ItemDescription;
    public Sprite ItemSprite;
    public int StackLimit;

    /*
    [Header("Durability Options")]
    public bool HasDurability;
    public float DurabilityAmmount;
    */
}

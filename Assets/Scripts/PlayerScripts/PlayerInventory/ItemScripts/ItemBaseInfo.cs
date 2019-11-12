using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemBaseInfo", menuName = "Create Item/Base Info", order = 0)]
public class ItemBaseInfo : ScriptableObject
{
    public enum ItemType { Consumable, Weapon, Armor, Placeable, Resource }
    [Header("Item Type")]
    public ItemType itemType;

    [Header("Item Information")]
    public string ItemName;
    [TextArea] public string ItemDescription;
    public Sprite ItemSprite;
    public int StackLimit;
    [Header("Item Pickup/Drop Sounds")]
    public AudioClip PickUpItemSound;
    public AudioClip DropItemSound;

    [Header("Item UI Sounds")]
    public AudioClip SelectSound; 
    public AudioClip DragSound; 
    public AudioClip DropSound; 

    /*
    [Header("Durability Options")]
    public bool HasDurability;
    public float DurabilityAmmount;
    */
}

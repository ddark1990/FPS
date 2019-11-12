using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemHolder : MonoBehaviour
{
    public Item CurrentlyHeldItem;
    public int ItemAmount;

    [Header("Cache")]
    public Image ItemSprite;
    public Text StackAmmountText;
    public Canvas canvas;

    public bool beingDragged;

    private int maxStackSize;

    private void Update()
    {
        UpdateItemInfo();
    }

    private void UpdateItemInfo()
    {
        var tempSpriteColor = ItemSprite.color;
        var tempTextColor = StackAmmountText.color;

        if (!CurrentlyHeldItem)
        {
            tempSpriteColor.a = 0;
            ItemSprite.color = tempSpriteColor;

            tempTextColor.a = 0;
            StackAmmountText.color = tempTextColor;

            return;
        }

        maxStackSize = CurrentlyHeldItem.ItemBaseInfo.StackLimit;

        tempSpriteColor.a = 255;
        ItemSprite.color = tempSpriteColor;

        tempTextColor.a = 255;
        StackAmmountText.color = tempTextColor;

        ItemSprite.sprite = CurrentlyHeldItem.ItemBaseInfo.ItemSprite;

        if(ItemAmount == 1) //if weapon or something that is not stackable
        {
            StackAmmountText.text = string.Empty;
            return;
        }

        StackAmmountText.text = string.Format("x{0:n0}", ItemAmount);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverOverPickUpUI : MonoBehaviour
{
    public Text ItemNameText;
    public Image PickUpHandIconImg;
    public Image CenterDotImg;

    public bool HoveringOverItem;

    [Header("Cache")]
    public Animator animator;
    public PlayerSelection playerSelection;


    private void Update()
    {
        UpdateHoverOver();
    }

    private void UpdateHoverOver()
    {
        animator.SetBool("HoveringOver", HoveringOverItem);

        if (!PlayerSelection.Instance.ObjectInView)
        {
            HoveringOverItem = false;
            return;
        }

        ItemNameText.text = playerSelection.ObjectInView.ItemBaseInfo.ItemName.ToUpper();
        HoveringOverItem = true;
    }
}

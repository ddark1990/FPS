using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour
{
    public static InventoryUIController Instance;

    public Animator InventoryAnimator;
    public GoomerFPSController.FPSController FPSController;

    public AudioSource AudioSource;
    public AudioClip SlotHoverSound;

    public bool MenuOpen;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            MenuOpen = !MenuOpen;
            FPSController.cameraLocked = !FPSController.cameraLocked;
            FPSController.cursorIsLocked = !FPSController.cursorIsLocked;
            InventoryAnimator.SetBool("MenuOpen", MenuOpen);
        }
    }

    public void PlayOnHoverSlotSound(float volumeScale)
    {
        AudioSource.PlayOneShot(SlotHoverSound, volumeScale);
    }

}

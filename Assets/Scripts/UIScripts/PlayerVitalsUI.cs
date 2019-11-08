using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerVitalsUI : MonoBehaviour
{
    public Image HealthBar;
    public Image HungerBar;
    public Image ThirstBar;
    public TextMeshProUGUI HungerText;
    public TextMeshProUGUI HealthText;
    public TextMeshProUGUI ThirstText;

    public PlayerVitals PlayerVitals;

    private float startingHealth;
    private float startingHunger;
    private float startingThirst;

    private void Start()
    {
        InitStartingValues();
    }

    private void Update()
    {
        HealthBar.fillAmount = PlayerVitals.Health / startingHealth;
        HungerBar.fillAmount = PlayerVitals.Calories / startingHunger;
        ThirstBar.fillAmount = PlayerVitals.Hydration / startingThirst;

        HealthText.text = PlayerVitals.Health.ToString("#");
        HungerText.text = PlayerVitals.Calories.ToString("#");
        ThirstText.text = PlayerVitals.Hydration.ToString("#");
    }

    private void InitStartingValues()
    {
        startingHealth = PlayerVitals.Health;
        startingHunger = PlayerVitals.Calories;
        startingThirst = PlayerVitals.Hydration;
    }


}

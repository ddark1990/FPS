using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVitals : MonoBehaviour
{
    public float Health;
    public float Calories;
    public float Hydration;

    [Range(0, 25)] public float hungerRate;
    [Range(0, 25)] public float thirstRate;

    private float maxHealth = 100;
    private float maxCalories = 500;
    private float maxHydration = 250;


    private void Awake()
    {
        StartStats();
    }

    void Update()
    {
        HungerOutput();
        ThirstOutput();

        HealthClamp();
    }

    private void StartStats()
    {
        Health = maxHealth;
        Calories = maxCalories;
        Hydration = maxHydration;
    }

    private void HungerOutput()
    {
        Calories = Mathf.Clamp(Calories, 0, maxCalories);
        Calories -= Time.deltaTime * hungerRate;
    }
    private void ThirstOutput()
    {
        Hydration = Mathf.Clamp(Hydration, 0, maxHydration);
        Hydration -= Time.deltaTime * thirstRate;
    }

    private void HealthClamp()
    {
        Health = Mathf.Clamp(Health, 0, maxHealth);
    }
}

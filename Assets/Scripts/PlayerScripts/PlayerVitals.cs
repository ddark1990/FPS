using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVitals : MonoBehaviour
{
    public float Health;
    public float Hunger;
    public float Thirst;

    [Range(0, 25)] public float hungerRate;
    [Range(0, 25)] public float thirstRate;

    private float maxHealth = 100;
    private float maxHunger = 500;
    private float maxThirst = 250;


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
        Hunger = maxHunger;
        Thirst = maxThirst;
    }

    private void HungerOutput()
    {
        Hunger = Mathf.Clamp(Hunger, 0, maxHunger);
        Hunger -= Time.deltaTime * hungerRate;
    }
    private void ThirstOutput()
    {
        Thirst = Mathf.Clamp(Thirst, 0, maxThirst);
        Thirst -= Time.deltaTime * thirstRate;
    }

    private void HealthClamp()
    {
        Health = Mathf.Clamp(Health, 0, maxHealth);
    }
}

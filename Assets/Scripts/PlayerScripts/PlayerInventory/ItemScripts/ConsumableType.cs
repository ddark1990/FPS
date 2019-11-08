using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConsumableItem", menuName = "Create Item/Item Type/Consumable", order = 0)]
public class ConsumableType : ScriptableObject
{
    public bool Cookable, Raw;

    public float GiveHealthAmmount;
    public float GiveCaloriesAmmount;
    public float GiveHydrationAmmount;

    public void Eat(float playerHealth, float playerCalories, float playerHydration)
    {
        playerHealth += GiveHealthAmmount;
        playerCalories += GiveCaloriesAmmount;
        playerHydration += GiveHydrationAmmount;
    }
}

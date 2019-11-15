using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConsumableItem", menuName = "Create Item/Item Type/Consumable", order = 0)]
public class ConsumableType : ScriptableObject
{
    public bool Cookable, Raw;

    public float GiveHealthAmount;
    public float GiveCaloriesAmount;
    public float GiveHydrationAmount;

    public void EatFood(float playerHealth, float playerCalories, float playerHydration)
    {
        playerHealth += GiveHealthAmount;
        playerCalories += GiveCaloriesAmount;
        playerHydration += GiveHydrationAmount;
    }
}

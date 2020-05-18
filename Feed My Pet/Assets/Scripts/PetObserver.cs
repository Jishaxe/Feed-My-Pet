using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetObserver : MonoBehaviour
{
    FoodSpawner _spawner;

    public StockpileArea stockpileArea;

    void Start() {
        _spawner = GameObject.Find("FoodSpawner").GetComponent<FoodSpawner>();
        stockpileArea = GameObject.Find("StockpileArea").GetComponent<StockpileArea>();
    }

    /// <summary>
    /// Get closest food to the pet
    /// </summary>
    /// <param name="searchStockpiles">true if we should search in stockpiles too</param>
    /// <returns>closest food, or null if none</returns>
    public FoodObject GetClosestFood(bool searchStockpiles) {
        FoodObject result = null;
        float closestDistance = Mathf.Infinity;

        foreach (FoodObject food in _spawner.foodInScene) {
            if (food.stockpileArea != null && !searchStockpiles) continue;
            float dist = (food.transform.position - this.transform.position).magnitude;
            if (dist < closestDistance) {
                result = food;
                closestDistance = dist;
            }
        }

        return result;
    }
}

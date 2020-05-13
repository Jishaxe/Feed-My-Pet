using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockpileArea : MonoBehaviour
{
    void OnTriggerEnter(Collider collider) {
        if (collider.CompareTag("Food")) {
            collider.GetComponentInParent<FoodObject>().stockpileArea = this;
        }
    }

    void OnTriggerExit(Collider collider) {
        if (collider.CompareTag("Food")) {
            collider.GetComponentInParent<FoodObject>().stockpileArea = null;
        }
    }
}

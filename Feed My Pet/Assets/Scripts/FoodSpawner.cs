using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public FoodObject foodPrefab;

    public List<FoodObject> foodInScene;
    
    public Collider raycastCatcher;


    public void SpawnFoodRandomly() {
        Vector3 randomLocation = new Vector3(Random.Range(raycastCatcher.bounds.min.x, raycastCatcher.bounds.max.x), 0.5f, Random.Range(raycastCatcher.bounds.min.z, raycastCatcher.bounds.max.z));
        SpawnFood(randomLocation);
    }

    void SpawnFood(Vector3 position) {
        FoodObject newFood = Instantiate(foodPrefab.gameObject).GetComponent<FoodObject>();

        newFood.transform.position = position;
        newFood.spawner = this;

        foodInScene.Add(newFood);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0)) {
            RaycastHit hit; 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 

            if (Physics.Raycast (ray, out hit, 100f, LayerMask.GetMask("RaycastCatcher"))) {
                Vector3 position = hit.point + new Vector3(0, 0.5f, 0);
                SpawnFood(position);
            }
        }
    }
}

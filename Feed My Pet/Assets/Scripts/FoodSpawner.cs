using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public FoodObject foodPrefab;

    public List<FoodObject> foodInScene;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0)) {
            RaycastHit hit; 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 

            if (Physics.Raycast (ray, out hit, 100f, LayerMask.GetMask("RaycastCatcher"))) {
                Vector3 position = hit.point + new Vector3(0, 0.5f, 0);
                FoodObject newFood = Instantiate(foodPrefab.gameObject).GetComponent<FoodObject>();

                newFood.transform.position = position;
                newFood.spawner = this;

                foodInScene.Add(newFood);
            }
        }
    }
}

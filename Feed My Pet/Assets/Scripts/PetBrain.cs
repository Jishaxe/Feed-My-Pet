using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetBrain : MonoBehaviour
{
    // TODO: Should be in PetMovement
    /// <summary>
    /// Adjusts wander "bumpiness"
    /// </summary>
    /// <value></value>
    [SerializeField] float wanderFactor;

    /// <summary>
    /// How long between ticks
    /// </summary>
    [SerializeField] float _tickSeconds;


    /// <summary>
    /// List of available actions
    /// </summary>
    List<BasePetAction> _actions = new List<BasePetAction>();
    BasePetAction _currentAction = new DoNothingAction();

    public bool isPlayerControlled = false;

    PetMovement _petMovement;
    JellyMesh _jellyMesh;
    MeshRenderer _renderer;
    Animator _animator;
    FoodSpawner _foodSpawner;
    PetInteractor _interactor;
    StockpileArea _stockpileArea;
    PetSounds _sounds;
    PetStats _stats;

    List<FoodObject> _spottedFood = new List<FoodObject>();

    // Start is called before the first frame update
    void Start()
    {
        _petMovement = GetComponent<PetMovement>();
        _jellyMesh = GetComponent<JellyMesh>();
        _renderer = GetComponent<MeshRenderer>();
        _animator = GetComponent<Animator>();
        _foodSpawner = GameObject.Find("FoodSpawner").GetComponent<FoodSpawner>();
        _stockpileArea = GameObject.Find("StockpileArea").GetComponent<StockpileArea>();
        _interactor = GetComponent<PetInteractor>();
        _sounds = GetComponent<PetSounds>();
        _stats = GetComponent<PetStats>();
        
        string debugActionList = "";
        // Use reflection to find every type that inherits from BasePetAction, instansiate it, and stick it in _actions
        foreach (var type in System.AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(BasePetAction))) {
            var action = (BasePetAction)Activator.CreateInstance(type);
            _actions.Add(action);
            debugActionList += type.ToString() + " ";
        }

        Debug.Log("Loaded " + _actions.Count + " pet actions: " + debugActionList);

        StartCoroutine(Tick());
        StartCoroutine(LookForFood());
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newMovementDirection = Vector3.zero;

        if (isPlayerControlled) {
            newMovementDirection = PlayerControl();
        } else {
            newMovementDirection = GetMovement();
        }

        if (newMovementDirection.magnitude > 1) newMovementDirection.Normalize();
        newMovementDirection *= _stats.excitement;
        newMovementDirection.y = 0;
        _petMovement.movementDirection = newMovementDirection;    
    }

    /// <summary>
    /// Add up all the scorers and pick the next action
    /// </summary>
    IEnumerator Tick() {
        while (true) {

            yield return new WaitForSeconds(_tickSeconds);
        }
    }

    /// <summary>
    /// Looks for food and stores it in the seen food array, checks every few seconds according to awakeness
    /// </summary>
    /// <returns></returns>
    IEnumerator LookForFood() {
        while (true) {
            _spottedFood = new List<FoodObject>(_foodSpawner.foodInScene);
            yield return new WaitForSeconds(_stats.GetReactionTime());
        }
    }


    /// <summary>
    /// Get closest food to the pet
    /// </summary>
    /// <param name="searchStockpiles">true if we should search in stockpiles too</param>
    /// <returns>closest food, or null if none</returns>
    FoodObject GetClosestFood(bool searchStockpiles) {
        FoodObject result = null;
        float closestDistance = Mathf.Infinity;

        foreach (FoodObject food in _spottedFood) {
            if (food.stockpileArea != null && !searchStockpiles) continue;
            float dist = (food.transform.position - this.transform.position).magnitude;
            if (dist < closestDistance) {
                result = food;
                closestDistance = dist;
            }
        }

        return result;
    }

    void FoodBitten(FoodObject food) {
        _stats.hunger += food.hungerPerBite;
        _stats.excitement += _stats._excitementChangeCurve.Evaluate(_stats.excitement) / 1000;
    }

    void FoodEaten(FoodObject food) {
        food.Eaten();
        this._spottedFood.Remove(food);
        Destroy(food.gameObject);
    }

    Vector3 GetMovement() {
        if (_spottedFood.Count > 0) {
            // there is food on the map and we are hungry
            if (_interactor.shouldBeEating) {
                FoodObject closestFood = GetClosestFood(true);
                float distanceToFood = (closestFood.transform.position - this.transform.position).magnitude;

                if (distanceToFood > _interactor.interactionDistance) {
                    return (closestFood.transform.position - this.transform.position).normalized;
                } else {
                    _interactor.EatFood(closestFood);
                }
            } else {
                // there is food on the map but we're not hungry
                if (!_interactor.isHoldingObject) {
                    // if we're not holding food, see if there's any food outside of stockpiles
                    FoodObject closestFood = GetClosestFood(false);


                    // if there is food outside of stockpiles nearby, go to it and pick it up
                    if (closestFood != null) {
                        float distanceToFood = (closestFood.transform.position - this.transform.position).magnitude;

                        if (distanceToFood > _interactor.interactionDistance) {
                            return (closestFood.transform.position - this.transform.position).normalized;
                        } else {
                            _interactor.PickUpObject(closestFood);
                        }
                    }
                } else {
                    // check that the food hasn't entered a stockpile yet
                    if (_interactor.currentHeldObject.stockpileArea == null) {
                        // we are holding food and dont want to eat it, move it to stockpile
                        float distanceToStockpile = (_stockpileArea.transform.position - this.transform.position).magnitude;
                        return (_stockpileArea.transform.position - this.transform.position).normalized;
                    } else {
                        // food has entered stockpile area, drop it
                        _interactor.DropObject();
                    }
                }
     
            }
        }

        if (_interactor.isEating) return Vector3.zero;
        
        // wander?
        float factor = Time.time * wanderFactor;

        Vector3 wander = new Vector3((Mathf.PerlinNoise(factor, 0) * 2) - 1, 0, (Mathf.PerlinNoise(0, factor) * 2) - 1);
        return wander;
    }

    Vector3 PlayerControl() {
        Vector3 movementDirection = new Vector3(Input.GetAxis("Vertical"), 0, -Input.GetAxis("Horizontal"));
        movementDirection.Normalize();

        return movementDirection;
    }
}

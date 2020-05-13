using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetBrain : MonoBehaviour
{
    [SerializeField] [Range(0,1)] float _awakeness;
    
    /// <summary>
    /// How long it takes to react to things when fully alert
    /// </summary>
    [SerializeField] float _alertReactionTime;

    /// <summary>
    /// How long it takes to react to things when sleeping
    /// </summary>
    [SerializeField] float _sleepingReactionTime;

    [Space(30)]
    [SerializeField] [Range(0,1)] float _excitement;

    /// <summary>
    /// Where the excitement level will naturally end up at
    /// </summary>
    /// <returns></returns>
    [SerializeField] [Range(0, 1)] float _excitementTendsTo;
    /// <summary>
    /// This curve depicts how fast the excitement level changes back to the tend to amount.
    /// </summary>
    [SerializeField] AnimationCurve _excitementChangeCurve;

    public float hunger {
        get {
            return _hunger;
        }

        set {
            _hunger = Mathf.Clamp01(value);

            if (value > 0.9) _interactor.shouldBeEating = false;
            else _interactor.shouldBeEating = true;
        }
    }
    [SerializeField] [Range(0,1)] float _hunger;
    [SerializeField] float _hungerLoss;
    [SerializeField] Color _hungryColour;
    [SerializeField] Color _fullColour;
    [SerializeField] float _hungrySpringiness;
    [SerializeField] float _fullSpringiness;

    /// <summary>
    /// How long between ticks
    /// </summary>
    [SerializeField] float _tickSeconds;

    
    public bool isPlayerControlled = false;

    PetMovement _petMovement;
    JellyMesh _jellyMesh;
    MeshRenderer _renderer;
    Animator _animator;
    FoodSpawner _foodSpawner;
    PetInteractor _interactor;
    StockpileArea _stockpileArea;
    PetSounds _sounds;

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
        newMovementDirection *= _excitement;
        newMovementDirection.y = 0;
        _petMovement.movementDirection = newMovementDirection;    

        ApplyHungerFx();
        ApplyAwakenessFx();
    }

    /// <summary>
    /// This is only called at certain intevals for non-urgent calculations (for example, adjusting excitement)
    /// </summary>
    IEnumerator Tick() {
        while (true) {
            UpdateExcitement();
            UpdateHunger();

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
            yield return new WaitForSeconds(Mathf.Lerp(_sleepingReactionTime, _alertReactionTime, _awakeness));
        }
    }

    void UpdateHunger() {
        hunger -= _hungerLoss;
    }

    void UpdateExcitement() {
        // Tend excitement towards the _excitementTendsTo according to the curve.
        // Right now, the excitement will come back slower at 0 than at 1

        if (_excitement > _excitementTendsTo + 0.01f) {
            _excitement -= _excitementChangeCurve.Evaluate(_excitement) / 1000;
        } else if (_excitement < _excitementTendsTo - 0.01f) {
            _excitement += _excitementChangeCurve.Evaluate(_excitement) / 1000;
        }
    }

    void ApplyAwakenessFx() {
        _animator.speed = Mathf.Max(0.2f, _awakeness);
    }

    void ApplyHungerFx() {
        float newStiffness = Mathf.Lerp(_hungrySpringiness, _fullSpringiness, _hunger);
        
        if (newStiffness != _jellyMesh.m_Stiffness) {
            _jellyMesh.m_Stiffness = newStiffness;
            _jellyMesh.UpdateJoints();
		    _jellyMesh.WakeUp();
        }

        _renderer.sharedMaterial.color = Color.Lerp(_hungryColour, _fullColour, _hunger);
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
        this.hunger += food.hungerPerBite;
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

        // wander?
        Vector3 wander = Vector3.one * ((Mathf.PerlinNoise(Time.time, Time.time) * 2) - 1);
        return wander;
    }

    Vector3 PlayerControl() {
        Vector3 movementDirection = new Vector3(Input.GetAxis("Vertical"), 0, -Input.GetAxis("Horizontal"));
        movementDirection.Normalize();

        return movementDirection;
    }
}

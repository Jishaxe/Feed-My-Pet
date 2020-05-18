using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetStats : MonoBehaviour
{   
    [SerializeField] [Range(0,1)] public float awakeness;
    
    /// <summary>
    /// How long it takes to react to things when fully alert
    /// </summary>
    [SerializeField] float _alertReactionTime;

    /// <summary>
    /// How long it takes to react to things when sleeping
    /// </summary>
    [SerializeField] float _sleepingReactionTime;

    [Space(30)]
    [SerializeField] [Range(0,1)] public float excitement;

    /// <summary>
    /// Where the excitement level will naturally end up at
    /// </summary>
    /// <returns></returns>
    [SerializeField] [Range(0, 1)] float _excitementTendsTo;
    /// <summary>
    /// This curve depicts how fast the excitement level changes back to the tend to amount.
    /// </summary>
    [SerializeField] public AnimationCurve _excitementChangeCurve;

    [Space(30)]
    [SerializeField] [Range(0,1)] public float fun;
    [SerializeField] float _funLoss;




    public float hunger {
        get {
            return _hunger;
        }

        set {
            _hunger = Mathf.Clamp01(value);
        }
    }
    [Space(30)]
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

    

    PetMovement _petMovement;
    JellyMesh _jellyMesh;
    MeshRenderer _renderer;
    Animator _animator;
    FoodSpawner _foodSpawner;
    PetInteractor _interactor;
    StockpileArea _stockpileArea;
    PetSounds _sounds;

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
    }


    /// <summary>
    /// This is only called at certain intevals for non-urgent calculations (for example, adjusting excitement)
    /// </summary>
    IEnumerator Tick() {
        while (true) {
            UpdateExcitement();
            UpdateHunger();
            UpdateFun();
            ApplyHungerFx();
            ApplyAwakenessFx();

            yield return new WaitForSeconds(_tickSeconds);
        }
    }

    void UpdateHunger() {
        hunger -= _hungerLoss;
        if (hunger < 0.3f) {
            excitement -= 0.05f; // excitement penalty when starving

            
            // get excited when hungry and we see food
            if (_foodSpawner.foodInScene.Count > 0) {
                excitement += 0.5f * (1 - hunger);
            }

            excitement = Mathf.Clamp01(excitement);
        }
    }

    void UpdateExcitement() {
        // Tend excitement towards the _excitementTendsTo according to the curve.
        // Right now, the excitement will come back slower at 0 than at 1

        if (excitement > _excitementTendsTo + 0.01f) {
            excitement -= _excitementChangeCurve.Evaluate(excitement) / 1000;
        } else if (excitement < _excitementTendsTo - 0.01f) {
            excitement += _excitementChangeCurve.Evaluate(excitement) / 1000;
        }

        excitement = Mathf.Clamp01(excitement);
    }

    void UpdateFun() {
        fun -= _funLoss;
        fun = Mathf.Clamp01(fun);
    }

    /// <summary>
    /// Modulate animation speed depending on how awake we are
    /// </summary>
    void ApplyAwakenessFx() {
        _animator.speed = Mathf.Max(0.2f, awakeness);
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
    /// Gets the time it should take to react to new things (tick length?) based on awakeness
    /// </summary>
    /// <returns></returns>
    public float GetReactionTime() {
        return Mathf.Lerp(_sleepingReactionTime, _alertReactionTime, awakeness);
    }
}

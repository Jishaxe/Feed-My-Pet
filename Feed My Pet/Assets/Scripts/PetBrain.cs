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
    BasePetAction _currentAction = new WanderAimlesslyAction();
    Coroutine _currentActionCoroutine = null;

    public bool isPlayerControlled = false;

    PetMovement _petMovement;
    JellyMesh _jellyMesh;
    MeshRenderer _renderer;
    Animator _animator;
    FoodSpawner _foodSpawner;
    PetInteractor _interactor;
    PetSounds _sounds;
    PetStats _stats;

    // Start is called before the first frame update
    void Start()
    {
        _petMovement = GetComponent<PetMovement>();
        _jellyMesh = GetComponent<JellyMesh>();
        _renderer = GetComponent<MeshRenderer>();
        _animator = GetComponent<Animator>();
        _foodSpawner = GameObject.Find("FoodSpawner").GetComponent<FoodSpawner>();
        _interactor = GetComponent<PetInteractor>();
        _sounds = GetComponent<PetSounds>();
        _stats = GetComponent<PetStats>();
        
        string debugActionList = "";
        // Use reflection to find every type that inherits from BasePetAction, instansiate it, and stick it in _actions
        foreach (var type in System.AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(BasePetAction))) {
            var action = (BasePetAction)Activator.CreateInstance(type);
            action.Init(gameObject);
            _actions.Add(action);
            debugActionList += type.ToString() + " ";
        }

        Debug.Log("Loaded " + _actions.Count + " pet actions: " + debugActionList);

        StartCoroutine(Tick());
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newMovementDirection = Vector3.zero;

        if (isPlayerControlled) {
            newMovementDirection = PlayerControl();
        } else {
            newMovementDirection = _currentAction.GetMovement();
        }

        if (newMovementDirection.magnitude > 1) newMovementDirection.Normalize();
        newMovementDirection.y = 0;
        _petMovement.movementDirection = newMovementDirection;  

        _currentAction.Update();  
    }

    void FixedUpdate() {
        _currentAction.FixedUpdate();
    }

    /// <summary>
    /// Add up all the scorers and pick the next action
    /// </summary>
    IEnumerator Tick() {
        while (true) {
            // Go through every action, calculate the score, and find the top scorer
            int highestScore = int.MinValue;
            BasePetAction highestAction = null;

            foreach (BasePetAction action in _actions) {
                // skip this action if it has a debounce and not enough time has passed since it was last executed
                if (Time.time - action.actionLastStartedAt < action.debounce) continue; 

                int score = action.GetScore();
                if (score > highestScore) {
                    highestScore = score;
                    highestAction = action;
                }
            }

            // If the new action is different from the current one
            if (highestAction != _currentAction) {
                // If the current is interruptable, or it's not interruptable but has stopped running
                if (_currentAction.isInterruptable || !_currentAction.isRunning) {
                    // Stop the current action, and start the next action
                    _currentAction.StopAction();
                    _currentAction.isRunning = false;
                    _currentAction.actionLastStartedAt = Time.time;
                    if (_currentActionCoroutine != null) StopCoroutine(_currentActionCoroutine);
                    _currentAction = highestAction;
                    _currentActionCoroutine = StartCoroutine(highestAction.StartAction());
                    _currentAction.isRunning = true;
                    Debug.Log("Switched to action " + _currentAction.GetType().ToString() + " with a score of " + highestScore);
                }
            } else {
                if (!_currentAction.isRunning) {
                    Debug.Log("Replaying action " + _currentAction.GetType().ToString() + " with a score of " + highestScore);
                    _currentAction.actionLastStartedAt = Time.time;
                    _currentAction.StopAction();
                    if (_currentActionCoroutine != null) StopCoroutine(_currentActionCoroutine);
                    _currentActionCoroutine = StartCoroutine(highestAction.StartAction());
                    _currentAction.isRunning = true;
                }
            }

            yield return new WaitForSeconds(_tickSeconds);
        }
    }



    void FoodBitten(FoodObject food) {
        _stats.hunger += food.hungerPerBite;
        _stats.excitement += _stats._excitementChangeCurve.Evaluate(_stats.excitement) / 1000;
    }

    void FoodEaten(FoodObject food) {
        food.Eaten();
        Destroy(food.gameObject);
    }

    Vector3 PlayerControl() {
        Vector3 movementDirection = new Vector3(Input.GetAxis("Vertical"), 0, -Input.GetAxis("Horizontal"));
        movementDirection.Normalize();

        return movementDirection;
    }
}

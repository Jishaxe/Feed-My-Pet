using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePetAction
{
    protected GameObject _pet;
    protected PetMovement _petMovement;
    protected FoodSpawner _foodSpawner;
    protected PetInteractor _interactor;
    protected StockpileArea _stockpileArea;
    protected PetSounds _sounds;
    protected PetStats _stats;
    protected PetObserver _observer;

    /// <summary>
    /// If this is true, then this behaviour is still running and the tick will not make another decision
    /// The action may be able to be interrupted by setting this to false
    /// </summary>
    public bool isRunning = false;


    /// <summary>
    /// Can this action be interrupted by another action if that scores more?
    /// If false, the brain will wait until isRunning is false before replacing with another action
    /// </summary>
    public bool isInterruptable = true;


    /// <summary>
    /// Pull the relevant scripts from the Pet gameobject, called once at start of game
    /// </summary>
    /// <param name="pet"></param>
    public void Init(GameObject pet) {
        _petMovement = pet.GetComponent<PetMovement>();
        _foodSpawner = GameObject.Find("FoodSpawner").GetComponent<FoodSpawner>();
        _stockpileArea = GameObject.Find("StockpileArea").GetComponent<StockpileArea>();
        _interactor = pet.GetComponent<PetInteractor>();
        _sounds = pet.GetComponent<PetSounds>();
        _stats = pet.GetComponent<PetStats>();
        _observer = pet.GetComponent<PetObserver>();

        this._pet = pet;
    }

    /// <summary>
    /// Given the Pet GO, calculate a score for this behaviour. The top scoring behaviour will be picked for that tick
    /// </summary>
    /// <param name="pet">Pet GO</param>
    /// <returns></returns>
    public virtual int GetScore() { throw new NotImplementedException(); }

    /// <summary>
    /// Starts the main routine for this action, make sure to set isRunning to false when you're done
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator StartAction() { throw new NotImplementedException(); }

    /// <summary>
    /// Stops this action, either at its natural end or interrupt it
    /// </summary>
    public virtual void StopAction() {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Where should the pet move in this update?
    /// </summary>
    /// <returns></returns>
    public virtual Vector3 GetMovement() { throw new NotImplementedException(); }

    /// <summary>
    /// Called as part of normal Unity update loop
    /// </summary>
    public virtual void Update() {

    }  

    /// <summary>
    /// Called as part of normal Unity fixedupdate loop
    /// </summary>
    public virtual void FixedUpdate() {

    }
}

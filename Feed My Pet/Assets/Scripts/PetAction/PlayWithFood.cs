using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayWithFood: BasePetAction
{
    FoodObject _target;
    bool _isNearby = false;

    /// <summary>
    /// Given the Pet GO, calculate a score for this behaviour. The top scoring behaviour will be picked for that tick
    /// </summary>
    /// <param name="pet">Pet GO</param>
    /// <returns></returns>
    public override int GetScore() { 
        // we can't move food if there is none
        if (_foodSpawner.foodInScene.Count == 0) return -100;

        FoodObject closestFood = _observer.GetClosestFood(true);

        // if all our food is in stockpile, don't move anything
        if (closestFood == null) return -100;

        // more likely to play with food when bored
        int score = (int)Mathf.Lerp(30, 0, _stats.fun);

        float distanceToFood = (closestFood.transform.position - _pet.transform.position).magnitude;
        if (distanceToFood < 1f) score += 10; // if we're near food, more likely to play with it

        return score;
     }

    /// <summary>
    /// Starts the main routine for this action, make sure to set isRunning to false when you're done
    /// </summary>
    /// <returns></returns>
    public override IEnumerator StartAction() { 
        debounce = 10f;
        _isNearby = false;
        isInterruptable = true;

        // move towards the food to stockpile
        yield return new WaitUntil(() => _isNearby);

        _interactor.PickUpObject(_target);

        // wander around for a bit with the food
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 5f));
        
        // throw food
        _interactor.ThrowObject();

        _stats.fun += 0.1f; // playing with food not very fun :(

        yield return new WaitForSeconds(1f);

        isRunning = false;
    }

    /// <summary>
    /// Stops this action, either at its natural end or interrupt it
    /// </summary>
    public override void StopAction() {
        Debug.Log("stopped!");
        _interactor.DropObject();
    }

    /// <summary>
    /// Where should the pet move in this update?
    /// </summary>
    /// <returns></returns>
    public override Vector3 GetMovement() { 
        if (!_interactor.isHoldingObject) {
            _target = _observer.GetClosestFood(true);
            if (_target == null) return Vector3.zero;
            
            float distanceToFood = (_target.transform.position - _pet.transform.position).magnitude;

            if (distanceToFood > _interactor.interactionDistance) {
                return (_target.transform.position - _pet.transform.position).normalized;
            } else {
                _isNearby = true;
            }
        } else {
            return _petMovement.Wander(); // wander around for  abit when holding food
        }

        return Vector3.zero;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatFoodAction: BasePetAction
{
    FoodObject _target;
    bool _isEating = false;
    bool _isNearby = false;

    /// <summary>
    /// Given the Pet GO, calculate a score for this behaviour. The top scoring behaviour will be picked for that tick
    /// </summary>
    /// <param name="pet">Pet GO</param>
    /// <returns></returns>
    public override int GetScore() { 
        // we can't eat food if there is none
        if (_foodSpawner.foodInScene.Count == 0) return -100;

        // if there is food, return score based on hunger
        return (int)Mathf.Lerp(100, 0, _stats.hunger);
    }

    /// <summary>
    /// Starts the main routine for this action, make sure to set isRunning to false when you're done
    /// </summary>
    /// <returns></returns>
    public override IEnumerator StartAction() { 
        // we can be interrupted while moving to the food
        isInterruptable = true;

        _isNearby = false;
        _isEating = false;
        _target = _observer.GetClosestFood(true);

        yield return new WaitUntil(() => _isNearby);

        // we can't be interrupted while eating the food
        _interactor.EatFood(_target);
        _isEating = true;
        isInterruptable = false;

        // wait until food is eaten, then we're done
        yield return new WaitUntil(() => _target.bitesLeft == 0);

        yield return new WaitForSeconds(0.5f);
        
        // done eating
        isRunning = false;
    }

    /// <summary>
    /// Stops this action, either at its natural end or interrupt it
    /// </summary>
    public override void StopAction() {
    }

    /// <summary>
    /// Where should the pet move in this update?
    /// </summary>
    /// <returns></returns>
    public override Vector3 GetMovement() { 
        if (!_isEating) {
            _target = _observer.GetClosestFood(true);
            float distanceToFood = (_target.transform.position - _pet.transform.position).magnitude;

            if (distanceToFood > _interactor.interactionDistance) {
                return (_target.transform.position - _pet.transform.position).normalized;
            } else {
                _isNearby = true;
            }
        }

        // stop moving if we're eating or nearby the food
        return Vector3.zero;
    }
}

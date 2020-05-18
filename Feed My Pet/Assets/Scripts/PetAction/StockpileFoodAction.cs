using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockpileFoodAction: BasePetAction
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

        FoodObject closestFoodNotInStockpile = _observer.GetClosestFood(false);

        // if all our food is in stockpile, don't move anything
        if (closestFoodNotInStockpile == null) return -100;

        int score = 25;

        float distanceToFood = (closestFoodNotInStockpile.transform.position - _pet.transform.position).magnitude;
        if (distanceToFood < 1f) score += 25; // if we're near food, more likely to start stockpiling

        return score;
     }

    /// <summary>
    /// Starts the main routine for this action, make sure to set isRunning to false when you're done
    /// </summary>
    /// <returns></returns>
    public override IEnumerator StartAction() { 
        _isNearby = false;
        isInterruptable = true;

        // move towards the food to stockpile
        yield return new WaitUntil(() => _isNearby);

        _interactor.PickUpObject(_target);

        // move towards the stockpile area until the food has been assigned a stockpile
        // aka it moves through the stockpile trigger
        yield return new WaitUntil(() => _target.stockpileArea != null);
        
        _interactor.DropObject();

        isRunning = false;
    }

    /// <summary>
    /// Stops this action, either at its natural end or interrupt it
    /// </summary>
    public override void StopAction() {
        _interactor.DropObject();
    }

    /// <summary>
    /// Where should the pet move in this update?
    /// </summary>
    /// <returns></returns>
    public override Vector3 GetMovement() { 
        if (!_interactor.isHoldingObject) {
            _target = _observer.GetClosestFood(false);
            if (_target == null) return Vector3.zero;
            
            float distanceToFood = (_target.transform.position - _pet.transform.position).magnitude;

            if (distanceToFood > _interactor.interactionDistance) {
                return (_target.transform.position - _pet.transform.position).normalized;
            } else {
                _isNearby = true;
            }
        } else {
            return (_observer.stockpileArea.transform.position - _pet.transform.position).normalized;
        }

        return Vector3.zero;
    }
}

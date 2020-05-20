using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LookOutWindow: BasePetAction
{
    bool isAtWindow = false;

    /// <summary>
    /// Given the Pet GO, calculate a score for this behaviour. The top scoring behaviour will be picked for that tick
    /// </summary>
    /// <param name="pet">Pet GO</param>
    /// <returns></returns>
    public override int GetScore() { 
        int score = Random.Range(0, 10);
        float distanceToWindow = (_observer.windowLocation.position - _pet.transform.position).magnitude;

        if (distanceToWindow < 1f) score += 10;

        return score; 
    }

    /// <summary>
    /// Starts the main routine for this action, make sure to set isRunning to false when you're done
    /// </summary>
    /// <returns></returns>
    public override IEnumerator StartAction() { 
        debounce = 0;
        isAtWindow = false;
        isInterruptable = true;

        yield return new WaitUntil(() => isAtWindow && _petMovement.IsSettled());
        isInterruptable = false;
        debounce = 200;
        yield return new WaitForSeconds(Random.Range(5f, 10f));

        isRunning = false;
        yield return null;
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
        float distanceToWindow = (_observer.windowLocation.position - _pet.transform.position).magnitude;

        if (distanceToWindow > 0.5f) {
            return (_observer.windowLocation.position - _pet.transform.position).normalized;
        } else {
            isAtWindow = true;
            return Vector3.zero;
        }
    }
}

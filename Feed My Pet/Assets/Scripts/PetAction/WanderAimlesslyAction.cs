using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WanderAimlesslyAction: BasePetAction
{
    /// <summary>
    /// Given the Pet GO, calculate a score for this behaviour. The top scoring behaviour will be picked for that tick
    /// </summary>
    /// <param name="pet">Pet GO</param>
    /// <returns></returns>
    public override int GetScore() { return Random.Range(5, 20); }

    /// <summary>
    /// Starts the main routine for this action, make sure to set isRunning to false when you're done
    /// </summary>
    /// <returns></returns>
    public override IEnumerator StartAction() { 
        isInterruptable = true;
        isRunning = true;
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
    public override Vector3 GetMovement() { return _petMovement.Wander(); }
}

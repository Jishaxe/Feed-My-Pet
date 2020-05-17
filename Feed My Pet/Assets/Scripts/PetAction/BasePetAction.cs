using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePetAction
{
    /// <summary>
    /// If this is true, then this behaviour is still running and the tick will not make another decision
    /// The action may be able to be interrupted by setting this to false
    /// </summary>
    public bool isRunning = false;

    /// <summary>
    /// Given the Pet GO, calculate a score for this behaviour. The top scoring behaviour will be picked for that tick
    /// </summary>
    /// <param name="pet">Pet GO</param>
    /// <returns></returns>
    public int GetScore(GameObject pet) { throw new NotImplementedException(); }

    /// <summary>
    /// Starts the main routine for this action, make sure to set isRunning to false when you're done
    /// </summary>
    /// <returns></returns>
    public IEnumerator StartAction(GameObject pet) { throw new NotImplementedException(); }

    /// <summary>
    /// Where should the pet move in this update?
    /// </summary>
    /// <returns></returns>
    public Vector3 GetMovement() { throw new NotImplementedException(); }
}

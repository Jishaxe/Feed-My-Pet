using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanceHappilyAction: BasePetAction
{
    /// <summary>
    /// Given the Pet GO, calculate a score for this behaviour. The top scoring behaviour will be picked for that tick
    /// </summary>
    /// <param name="pet">Pet GO</param>
    /// <returns></returns>
    public override int GetScore() { 
        if (_stats.hunger < 0.5f) return -100;
        return (int)Mathf.Lerp(-30, 30, _stats.excitement);
    }

    /// <summary>
    /// Starts the main routine for this action, make sure to set isRunning to false when you're done
    /// </summary>
    /// <returns></returns>
    public override IEnumerator StartAction() { 
        debounce = 20f;
        isInterruptable = false;
        
        // dance for at least 3 seconds
        yield return new WaitForSeconds(UnityEngine.Random.Range(3f, 10f));

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
        float factor = Time.time * 10f;
        return new Vector3((Mathf.PerlinNoise(factor, 0) * 2) - 1, 0, (Mathf.PerlinNoise(0, factor) * 2) - 1);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RandomAudioClip {
    public AudioClip clip;
    [Range(0,1)] public float chance = 0.5f;
}


/// <summary>
/// Helper class for playing sounds with a given chance and avoiding repeating sounds
/// </summary>
public class SoundClipContainer : MonoBehaviour
{
    [SerializeField] RandomAudioClip[] sounds;

    /// <summary>
    /// A list of indexes to the sound array, such as 1,1,1,2,2,3,3,3 according to their chance
    /// then one of the indexes is picked out of a hat
    /// </summary>
    int[] _chanceArray;

    /// <summary>
    /// The last index that was picked to play, we compare new picks to this to make sure the same sound is never picked twice in a row
    /// </summary>
    int lastIndexPicked = -1;

    void BuildChanceArray() {
        List<int> newChanceArray = new List<int>();

        for (int i = 0; i < sounds.Length; i++) {

            RandomAudioClip clip = sounds[i];


            if (clip.chance == 0) continue; // skip if zero chance

            // Multiply the amount of available clips by the chance, so a clip with 0.5 chance in a selection of 10 clips 
            // leads to 5 of that index in the chance array, and 0.1 chance leads to 1 of that index
            // we also make sure it doesn't go below 1 so there will always be one clip in the array
            for (int x = 0; x < Mathf.Max(sounds.Length * clip.chance, 1); x++) {
                newChanceArray.Add(i);
            }

        }

        
        _chanceArray = newChanceArray.ToArray();
    }

    void Start() {
        BuildChanceArray();
    }

    public AudioClip GetRandomClip() {
        // if we only have one option, return that sound
        if (sounds.Length == 1) return sounds[0].clip;

        int pickedIndex = lastIndexPicked;
        // Keep drawing indexes from the chanceArray until it's one we've not picked before
        while (pickedIndex == lastIndexPicked) {
            pickedIndex = _chanceArray[UnityEngine.Random.Range(0, _chanceArray.Length)];
        }

        lastIndexPicked = pickedIndex;
        return sounds[pickedIndex].clip;
    }
}

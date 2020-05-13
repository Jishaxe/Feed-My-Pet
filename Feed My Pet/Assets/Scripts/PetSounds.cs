using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetSounds : MonoBehaviour
{
    [Space(30)]
    [SerializeField] AudioSource _squelchAudioSource;

    [SerializeField] AudioSource _genericAudioSource;

    [SerializeField] SoundClipContainer _squelchingClips;
    [SerializeField] SoundClipContainer _openMouthClips;
    [SerializeField] SoundClipContainer _biteClips;
    [SerializeField] SoundClipContainer _munchClips;
    
    /// <summary>
    /// X us minimum relativeVelocity magnitute to play squelch sound, Y is maximum speed before volume is not increased any further
    /// </summary>
    [SerializeField] Vector2 _playSquelchThreshold;

    /// <summary>
    /// X is lowest volume, Y is highest volume
    /// </summary>
    [SerializeField] Vector2 _squelchVolumes;

    /// <summary>
    /// X is lowest pitch, Y is highest pitch
    /// </summary>
    [SerializeField] Vector2 _squelchPitches;

    /// <summary>
    /// How long to wait between squelch sounds
    /// </summary>
    [SerializeField] float timeBetweenSquelches;

    float timeLastPlayedSquelch = 0;

    void PlaySquelchSound(float force) {
        _squelchAudioSource.volume = Mathf.Lerp(_squelchVolumes.x, _squelchVolumes.y, force);
        _squelchAudioSource.pitch = Mathf.Lerp(_squelchPitches.x, _squelchPitches.y, force);
        _squelchAudioSource.PlayOneShot(_squelchingClips.GetRandomClip());
    }   

    public void PlayOpenMouthSound() {
        _genericAudioSource.PlayOneShot(_openMouthClips.GetRandomClip());
    }

    public void PlayCrunchSound() {
        _genericAudioSource.PlayOneShot(_biteClips.GetRandomClip());
    }

    public void PlayMunchSounds()
    {
        _genericAudioSource.PlayOneShot(_munchClips.GetRandomClip());
    }



    void OnJellyCollisionEnter(JellyMesh.JellyCollision collision) {
        // if its not time to play another squelch yet, quit
        if (Time.time - timeLastPlayedSquelch < timeBetweenSquelches) return;

        // if above threshold to play squelch sound
        if (collision.Collision.relativeVelocity.magnitude > _playSquelchThreshold.x) {
            //Debug.Log("collision with velocity " + collision.Collision.relativeVelocity.magnitude);
            // calculate a force value from 0 - 1, 0 being the lower threshold and 1 being the upper threshold
            float soundForce = (collision.Collision.relativeVelocity.magnitude / _playSquelchThreshold.y) + (_playSquelchThreshold.x / _playSquelchThreshold.y);

            soundForce = Mathf.Clamp01(soundForce);
            PlaySquelchSound(soundForce);

            timeLastPlayedSquelch = Time.time;
        }
    }


}

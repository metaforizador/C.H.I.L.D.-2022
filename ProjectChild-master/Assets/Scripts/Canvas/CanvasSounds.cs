using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used for playing UI canvas sounds using a single audio source.
/// </summary>
public class CanvasSounds : MonoBehaviour {

    private AudioSource source;

    public AudioClip BUTTON_ENTER;
    public AudioClip BUTTON_SELECT, BUTTON_BACK;
    public AudioClip ITEM_BREAK, ITEM_USAGE_NOTIFICATION;
    public AudioClip LEVEL_UP;

    /// <summary>
    /// Gets audio source component.
    /// </summary>
    void Awake() => source = GetComponent<AudioSource>();

    /// <summary>
    /// Plays provided sound once.
    /// 
    /// Plays the sound from start to finish even if another
    /// sounds starts playing.
    /// </summary>
    /// <param name="clip">sound to play</param>
    public void PlaySound(AudioClip clip) => source.PlayOneShot(clip);
}

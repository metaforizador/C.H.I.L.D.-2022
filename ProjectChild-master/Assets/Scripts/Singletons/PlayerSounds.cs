using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Plays the player's speech sounds.
/// </summary>
public class PlayerSounds : MonoBehaviour {
    // Make class static and destroy if script already exists
    private static PlayerSounds _instance; // **<- reference link to the class
    public static PlayerSounds Instance { get { return _instance; } }

    private void Awake() {
        // If instance not yet created
        if (_instance == null) {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    // Priority whether to play some sound over the other
    private const int LOW = 0, MEDIUM = 1, HIGH = 2;
    private int curSpeechPriority;

    public static readonly int[] HIT_SPEECH_PERCENTAGES = new int[] { 75 };

    // Audios to play dependant of the percentage
    [SerializeField]
    private AudioClip[] hitSpeechAudios;

    [SerializeField]
    private AudioSource source;

    [SerializeField]
    private AudioClip[] takeHitGrunts;

    [SerializeField]
    private AudioClip weaponPickup;

    /// <summary>
    /// Plays a random grunt when taking hit to the hp.
    /// </summary>
    public void PlayRandomTakeHitGrunt() {
        if (NotAbleToPlay(LOW))
            return;

        AudioClip sound = takeHitGrunts[Random.Range(0, takeHitGrunts.Length)];
        PlayAudio(sound);
    }

    /// <summary>
    /// Plays a health low speech dependant of the percentage of hp.
    /// </summary>
    /// <param name="index"></param>
    public void PlayHealthLowAudio(int index) {
        if (NotAbleToPlay(MEDIUM))
            return;

        PlayAudio(hitSpeechAudios[index]);
    }

    /// <summary>
    /// Plays a weapon pickup sound when player changes her weapon.
    /// </summary>
    public void PlayWeaponPickup() {
        if (NotAbleToPlay(MEDIUM))
            return;

        PlayAudio(weaponPickup);
    }

    /// <summary>
    /// Checks if the audio is able to play or not.
    /// </summary>
    /// <param name="priority">priority of the playable sound</param>
    /// <returns>return true if it's not able to be played</returns>
    private bool NotAbleToPlay(int priority) {
        bool notAble = source.isPlaying && curSpeechPriority >= priority;

        if (!notAble)
            curSpeechPriority = priority;

        return notAble;
    }

    /// <summary>
    /// Plays an audio.
    /// </summary>
    /// <param name="clip">audio to play</param>
    private void PlayAudio(AudioClip clip) {
        source.clip = clip;
        source.Play();
    }
}

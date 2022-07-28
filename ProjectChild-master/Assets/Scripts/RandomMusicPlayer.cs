using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMusicPlayer : MonoBehaviour {

    [SerializeField]
    private AudioSource source;

    [SerializeField]
    private AudioClip[] musics;

    void Start() {
        AudioClip randomMusic = musics[Random.Range(0, musics.Length)];
        source.clip = randomMusic;
        source.Play();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance {  get; private set; }

    [Header("Audio Source")]
    [SerializeField] AudioSource SFXSource;
    [SerializeField] AudioSource musicSource;

    [Header("Audio Clips")]
    public AudioClip backgroundMusic;
    public AudioClip playerAttack;
    public AudioClip playerJump;
    public AudioClip playerFlash;
    public AudioClip saveStation;

    private void Awake()
    {
        instance = this;
        musicSource.clip = backgroundMusic;
    }
    public void PlaySound(AudioClip _sound)
    {
        SFXSource.PlayOneShot(_sound);
    }
}

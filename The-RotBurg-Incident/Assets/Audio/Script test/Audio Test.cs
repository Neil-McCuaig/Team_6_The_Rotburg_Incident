using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTest : MonoBehaviour
{
    public AudioClip otherClip;
    AudioSource audio;
    SoundManager soundManager;

    void Start()
    {
        audio = GetComponent<AudioSource>();
        audio.Play();
    }
   public void AudioPlay()
    {
        audio.Play();
    }
    public void AudioStop()
    {
        audio.Stop();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }

    [Header("Audio Source")]
    [SerializeField] AudioSource SFXSource;
    [SerializeField] AudioSource musicSource;
    [SerializeField] private AudioSource heartbeatSource;

    [Header("Audio Clips")]
    public AudioClip backgroundMusic;
    [Header("Player Sounds")]
    public AudioClip playerAttack;
    public AudioClip playerMiss;
    public AudioClip playerJump;
    public AudioClip playerFlash;
    public AudioClip playerHurt;
    public AudioClip playerHurtSquish;
    public AudioClip playerHurtSplash;
    public AudioClip playerMove;
    public AudioClip uiHover;
    public AudioClip uiOk;
    public AudioClip uiBack;
    public AudioClip doorOpen;
    public AudioClip doorClose;
    public AudioClip doorBoth;

    [Header("Enemy Sounds")]
    public AudioClip pouncerGeneral;
    public AudioClip pouncerKill;
    public AudioClip angelGeneral;
    public AudioClip angelKill;
    public AudioClip flyingGeneral;
    public AudioClip flyingKill;
    public AudioClip explodingGeneral;
    public AudioClip explodingBoom;
    public AudioClip explodingKill;
    public AudioClip redlightGeneral;
    public AudioClip redlightKill;  


    [Header("Enviroment Sounds")]
    public AudioClip saveStation;
    public AudioClip lockerOpen;
    public AudioClip lockerClose;

    [Header("Heartbeat Clips")]
    public AudioClip slowHeartbeat;
    public AudioClip mediumHeartbeat;
    public AudioClip fastHeartbeat;
    public AudioClip criticalHeartbeat;
    public AudioClip flatLine;
    private AudioClip currentHeartbeatClip;

    private void Awake()
    {
        instance = this;
        musicSource.clip = backgroundMusic;
    }
    public void PlaySound(AudioClip _sound)
    {
        SFXSource.PlayOneShot(_sound);
    }

    public void StopSound(AudioClip _sound)
    {
        SFXSource.Stop();
    }


    public void SwitchHeartbeat(int level)
    {
        AudioClip newClip = null;

        switch (level)
        {
            case 0:
                newClip = slowHeartbeat;
                break;
            case 1:
                newClip = mediumHeartbeat;
                break;
            case 2:
                newClip = fastHeartbeat;
                break;
            case 3:
                newClip = criticalHeartbeat;
                break;
            case 4:
                newClip = flatLine;
                break;
        }

        if (newClip != currentHeartbeatClip)
        {
            currentHeartbeatClip = newClip;
            heartbeatSource.clip = currentHeartbeatClip;
            heartbeatSource.loop = true;
            heartbeatSource.Play();
        }
    }
}

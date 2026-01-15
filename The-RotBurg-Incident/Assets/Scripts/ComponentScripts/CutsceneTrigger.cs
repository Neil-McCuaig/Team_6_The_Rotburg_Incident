using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneTrigger : MonoBehaviour
{
    [Header("Cutscene Camera")]
    [Tooltip("Index from CameraManager cutsceneCameras array")]
    [SerializeField] private int cutsceneCamIndex;

    [Header("Timeline")]
    [SerializeField] private PlayableDirector playableDirector;

    [Header("Trigger Settings")]
    [SerializeField] private bool triggerOnlyOnce = true;

    private bool hasTriggered;
    PlayerController playerController;

    private void OnEnable()
    {
        if (playableDirector != null)
        {
            playableDirector.stopped += OnTimelineStopped;
        }
    }

    private void OnDisable()
    {
        if (playableDirector != null)
        {
            playableDirector.stopped -= OnTimelineStopped;
        }
    }

    private void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggerOnlyOnce && hasTriggered)
        {
            return;
        }
        if (!other.CompareTag("Player"))
        {
            return;
        }

        hasTriggered = true;
        PlayCutscene();
    }

    private void PlayCutscene()
    {
        if (playerController != null)
        {
            playerController.canMove = false;
            playerController.anim.SetInteger("WalkX", 0);
        }
        CameraManager.instance.SwitchToCutsceneCamera(cutsceneCamIndex);

        playableDirector.time = 0;
        playableDirector.Play();
    }

    private void OnTimelineStopped(PlayableDirector director)
    {
        if (playerController != null)
        {
            playerController.canMove = true;
        }

        CameraManager.instance.ReturnToPlayerCamera();
    }
}

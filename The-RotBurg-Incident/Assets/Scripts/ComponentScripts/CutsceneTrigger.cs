using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneTrigger : MonoBehaviour
{
    [Header("Cutscene Camera")]
    [SerializeField] private int cutsceneCamIndex;

    [Header("Timeline")]
    [SerializeField] private PlayableDirector playableDirector;

    [Header("Trigger Settings")]
    [SerializeField] private bool triggerOnlyOnce = true;
    [SerializeField] private bool hideUIDuringCutscene = false;

    private bool hasTriggered;
    PlayerController playerController;
    GameManager gameManager;

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
        gameManager = FindAnyObjectByType<GameManager>();
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
            playerController.velocity.y = -2f;
            playerController.anim.SetInteger("WalkX", 0);
        }

        if (hideUIDuringCutscene && gameManager != null)
        {
            gameManager.HideUI();
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

        if (hideUIDuringCutscene && gameManager != null)
        {
            gameManager.ShowUI();
        }
    }
}

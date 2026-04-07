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
    [SerializeField] public Transform playerPosDuringCutscene;
    [SerializeField] public Transform playerPosAfterCutscene;

    [Header("Trigger Settings")]
    [SerializeField] private bool triggerOnlyOnce = true;
    [SerializeField] private bool hideUIDuringCutscene = false;

    public Transform player;
    private bool hasTriggered;
    public PlayerController playerController;
    GameManager gameManager;
    private Rigidbody2D rb;

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
        player = GameObject.FindWithTag("Player").transform;
        gameManager = FindAnyObjectByType<GameManager>();

        if (player != null)
        {
            rb = player.GetComponent<Rigidbody2D>();
        }
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
            playerController.canControl = false;
            playerController.velocity = Vector2.zero;
            playerController.anim.SetInteger("WalkX", 0);
        }

        if (rb != null)
        {
            rb.isKinematic = true;
        }

        CameraManager.instance.SwitchToCutsceneCamera(cutsceneCamIndex);

        if (playerPosDuringCutscene != null)
        {
            player.position = playerPosDuringCutscene.position;
        }
        else
        {
            Debug.LogWarning("Missing playerPosDuringCutscene");
        }

        CameraManager.instance.SwitchToCutsceneCamera(cutsceneCamIndex);

        if (hideUIDuringCutscene && gameManager != null)
        {
            gameManager.HideUI();
        }

        playableDirector.time = 0;
        playableDirector.Play();
    }

    private void OnTimelineStopped(PlayableDirector director)
    {
        if (playerPosAfterCutscene != null)
        {
            player.position = playerPosAfterCutscene.position;
        }
        else
        {
            Debug.LogWarning("Missing playerPosAfterCutscene");
        }

        if (rb != null)
        {
            rb.isKinematic = false;
        }

        if (playerController != null)
        {
            playerController.canControl = true;
        }

        CameraManager.instance.ReturnToPlayerCamera();

        if (hideUIDuringCutscene && gameManager != null)
        {
            gameManager.ShowUI();
        }
    }
}

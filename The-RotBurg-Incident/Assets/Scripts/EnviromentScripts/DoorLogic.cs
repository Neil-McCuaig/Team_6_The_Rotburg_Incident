using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorLogic : MonoBehaviour
{
    [Header("Teleport Settings")]
    public Transform teleportDestination;
    public float fadeDuration;
    public KeyCode interactKey = KeyCode.E;
    private bool playerInRange = false;

    [Header("References")]
    Transform player;
    private Animator anim;
    FadeToBlack fader;
    public GameObject playerObject;
    PlayerController playerController;
    GameObject playerArm;
    SpriteRenderer playerRenderer;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        playerArm = GameObject.FindWithTag("PlayerArm");
        playerRenderer = GameObject.FindWithTag("Player").GetComponent<SpriteRenderer>();

        fader = FindAnyObjectByType<FadeToBlack>();
        playerController = FindAnyObjectByType<PlayerController>();
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            if (teleportDestination == null || player == null)
            {
                return;
            }
            StartCoroutine(TeleportPlayer());
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public IEnumerator TeleportPlayer()
    {
        playerController.inLocker = true;
        anim.SetBool("IsOpen", false);
        fader.FadeOut();
        playerRenderer.enabled = false;
        playerArm.SetActive(false);

        yield return new WaitForSeconds(fadeDuration);

        player.position = teleportDestination.position;
        playerRenderer.enabled = true;
        playerArm.SetActive(true);
        anim.SetBool("IsOpen", true);
        fader.FadeIn();
        playerController.inLocker = false;
    }
}

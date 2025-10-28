using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockerInteraction : MonoBehaviour
{
    private bool playerNearby = false;
    private bool playerInside = false;
    PlayerController playerController;
    private GameObject player;
    PlayerHealth health;
    private GameObject arm;
    private SpriteRenderer playerSprite;
    private Rigidbody2D playerRb;
    private Animator anim;

    private void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();
        player = GameObject.Find("Player");
        health = player.GetComponent<PlayerHealth>();
        arm = GameObject.Find("Arm");
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (playerNearby && playerController.attackAction.WasPressedThisFrame())
        {
            if (!playerInside)
            {
                EnterLocker();
            }
            else
            {
                ExitLocker();
            }
        }
    }

    private void EnterLocker()
    {
        if (player == null)
        {
            return;
        }

        anim.SetBool("IsOpen", false);
        playerInside = true;
        playerController.inLocker = true;
        arm.SetActive(false);

        if (playerSprite == null)
        {
            playerSprite = player.GetComponent<SpriteRenderer>();
        }
        if (playerRb == null)
        {
            playerRb = player.GetComponent<Rigidbody2D>();
        }

        playerRb.simulated = false;
        playerSprite.enabled = false;
        player.transform.position = transform.position;

        if (health.currentHealth <= 0)
        {
            ExitLocker();
        }
    }

    private void ExitLocker()
    {
        if (player == null)
        {
            return;
        }

        anim.SetBool("IsOpen", true);
        playerInside = false;
        playerController.inLocker = false;
        arm.SetActive(true);
        playerRb.simulated = true;
        playerSprite.enabled = true;

        player.transform.position = transform.position + Vector3.up * 1f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            player = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!playerInside)
            {
                playerNearby = false;
                player = null;
            }
        }
    }
}

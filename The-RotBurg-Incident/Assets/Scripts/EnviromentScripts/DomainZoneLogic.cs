using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DomainZoneLogic : MonoBehaviour
{
    public bool playerInDomain = false;
    PlayerController playerController;

    private void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();
    }

    private void Update()
    {
        if(playerController.isDead)
        {
            playerInDomain = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInDomain = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerController != null && playerController.inLocker == false)
            {
                playerInDomain = false;
            }
            else
            {
                return;
            }
        }
    }
}

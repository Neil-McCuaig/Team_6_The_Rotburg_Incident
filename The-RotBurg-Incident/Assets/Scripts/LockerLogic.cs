using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LockerLogic : MonoBehaviour
{
    public GameObject hoverEffect;

    private bool playerInRange = false;
    private bool inLocker = false;

    private PlayerController playerController;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        hoverEffect.SetActive(false);
        playerController = FindAnyObjectByType<PlayerController>();
        gameManager = FindAnyObjectByType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && !inLocker && playerController.attackAction.WasPressedThisFrame())
        {
            StartHiding();
        }
        if (inLocker)
        {
            Debug.Log("Player has hidden inside the locker.");
            playerController.isHiding = true;

            if (playerController.moveInput.x != 0)
            {
                StopHiding();
            }
        }
    }

    private void StartHiding()
    {
        inLocker = true;
        hoverEffect.SetActive(false);
        if (playerController != null)
        {
            playerController.isSitting = true;
            Debug.Log("Player is hiding inside the locker.");
        }
    }
    private void StopHiding()
    {
        inLocker = false;
        hoverEffect.SetActive(true);
        if (playerController != null)
        {
            playerController.isSitting = false;
            playerController.isHiding = false;
        }
        Debug.Log("Player has left the locker.");
    }
}

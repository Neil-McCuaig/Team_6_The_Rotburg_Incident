using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class NotePickupLogic : MonoBehaviour
{
    public GameObject noteGraphic;

    private bool playerInRange = false;

    private PlayerController playerController;
    private GameManager gameManager;


    // Start is called before the first frame update
    void Start()
    {
        noteGraphic.SetActive(false);
        playerController = FindAnyObjectByType<PlayerController>();
        gameManager = FindAnyObjectByType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && playerController.attackAction.WasPressedThisFrame())
        {

        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            noteGraphic.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            noteGraphic.SetActive(false);
        }
    }

}

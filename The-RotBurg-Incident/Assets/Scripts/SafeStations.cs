using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SafeStations : MonoBehaviour
{
    public GameObject hoverEffect; 
    public float chargeRate; 

    private bool playerInRange = false;
    private bool isCharging = false;
    private PlayerController playerController;
    private GameManager gameManager;
    private PlayerHealth health;

    private TextMeshProUGUI rechargeText;

    private void Start()
    {
        hoverEffect.SetActive(false);
        playerController = FindAnyObjectByType<PlayerController>();
        gameManager = FindAnyObjectByType<GameManager>();
        health = FindAnyObjectByType<PlayerHealth>();
        GameObject textObject = GameObject.Find("PercentageText");
        rechargeText = textObject.GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (playerInRange && !isCharging && playerController.attackAction.WasPressedThisFrame())
        {
            StartCharging();
            health.ResetHealthFull();
        }

        if (isCharging)
        {
            ChargeBattery();

            if (playerController.moveInput.x != 0)
            {
                StopCharging();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            hoverEffect.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            hoverEffect.SetActive(false);
            if (isCharging)
            {
                StopCharging();
            }
        }
    }

    private void StartCharging()
    {
        isCharging = true;
        hoverEffect.SetActive(false);
        rechargeText.color = Color.yellow;
        if (playerController != null)
        {
            playerController.isSitting = true;
        }
        Debug.Log("Player is sitting at the save station.");
    }

    private void StopCharging()
    {
        isCharging = false;
        rechargeText.color = Color.white;
        if (playerController != null)
        {
            playerController.isSitting = false;
        }
        Debug.Log("Player left the save station.");
    }

    private void ChargeBattery()
    {
        if (gameManager != null)
        {
            gameManager.batteryPercentage += chargeRate * Time.deltaTime;
            gameManager.batteryPercentage = Mathf.Clamp(gameManager.batteryPercentage, 0f, 100f);
        }
    }
}

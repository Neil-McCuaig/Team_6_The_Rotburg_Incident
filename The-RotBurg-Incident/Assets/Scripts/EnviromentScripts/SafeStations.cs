using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SafeStations : MonoBehaviour
{
    public GameObject hoverEffect; 
    public float chargeRate; 

    private bool playerInRange = false;
    public bool isCharging = false;
    public Transform spawnPoint;

    private EnemySpawnerManager enemySpawnerManager;
    private PlayerController playerController;
    private GameManager gameManager;
    private PlayerHealth health;

    [SerializeField] private AudioClip rechargeSound;

    private void Awake()
    {
        enemySpawnerManager = FindAnyObjectByType<EnemySpawnerManager>();
        if (enemySpawnerManager != null)
        {
            enemySpawnerManager.SpawnEnemies();
        }
    }
    private void Start()
    {
        hoverEffect.SetActive(false);
        playerController = FindAnyObjectByType<PlayerController>();
        playerController.SetRespawnPoint(playerController.transform.position);
        gameManager = FindAnyObjectByType<GameManager>();
        health = FindAnyObjectByType<PlayerHealth>();
    }

    private void Update()
    {
        if (playerInRange && !isCharging && playerController.attackAction.WasPressedThisFrame())
        {
            SoundManager.instance.PlaySound(rechargeSound);
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
        if (playerController != null)
        {
            playerController.isSitting = true;
            playerController.SetRespawnPoint(spawnPoint.position);
        }
        if (enemySpawnerManager != null)
        {
            enemySpawnerManager.SpawnEnemies();
        }
    }

    private void StopCharging()
    {
        isCharging = false;
        hoverEffect.SetActive(true);
        if (playerController != null)
        {
            playerController.isSitting = false;
        }
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

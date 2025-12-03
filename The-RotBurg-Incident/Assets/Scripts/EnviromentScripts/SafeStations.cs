using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SafeStations : MonoBehaviour
{
    public GameObject hoverEffect; 
    public float chargeRate; 

    private bool playerInRange = false;
    public bool isCharging = false;
    public Transform spawnPoint;

    private Animator anim;
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
        anim = playerController.GetComponent<Animator>();
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
                anim.SetBool("IsCharging", false);
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
        anim.SetBool("IsCharging", true);
        anim.SetTrigger("StartCharging");
        playerController.DisableArmRender();
        playerController.flashLight.gameObject.SetActive(false);

        isCharging = true;
        hoverEffect.SetActive(false);
        if (playerController != null)
        {
            playerController.canMove = false;
            playerController.SetRespawnPoint(spawnPoint.position);
        }
        if (enemySpawnerManager != null)
        {
            enemySpawnerManager.SpawnEnemies();
        }
    }

    public void StopCharging()
    {
        playerController.EnableArmRender();
        playerController.flashLight.gameObject.SetActive(true);

        isCharging = false;
        hoverEffect.SetActive(true);
        if (playerController != null)
        {
            playerController.canMove = true;
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

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

    public bool openUpgradesOnInteract;

    private Animator anim;
    private EnemySpawnerManager enemySpawnerManager;
    private CameraManager camManager;
    private PlayerController playerController;
    private GameManager gameManager;
    private PlayerHealth health;
    private StreamChat chat;
    private ViewerStats viewers;

    [SerializeField] private AudioClip rechargeSound;

    public Texture2D cursorFingerTexture;
    public Texture2D cursorCamTexture;
    public Vector2 hotspot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;

    void OnDisable()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }

    private void Awake()
    {
        enemySpawnerManager = FindAnyObjectByType<EnemySpawnerManager>();
        camManager = FindAnyObjectByType<CameraManager>();
    }

    private void Start()
    {
        hoverEffect.SetActive(false);
        enemySpawnerManager.SpawnEnemies();

        playerController = FindAnyObjectByType<PlayerController>();
        gameManager = FindAnyObjectByType<GameManager>();
        health = FindAnyObjectByType<PlayerHealth>();
        anim = playerController.GetComponent<Animator>();
        chat = FindAnyObjectByType<StreamChat>();
        viewers = FindAnyObjectByType<ViewerStats>();
    }

    private void Update()
    {
        if (playerInRange && !isCharging && playerController.interactAction.WasPressedThisFrame())
        {
            TriggerCharge();
            playerController.SetRespawnPoint();
        }

        if (isCharging)
        {
            ChargeBattery();

            if (playerController.moveInput.x != 0)
            {
                anim.SetBool("IsCharging", false);

                if (openUpgradesOnInteract)
                {
                    Cursor.SetCursor(cursorCamTexture, hotspot, cursorMode);
                    camManager.ReturnToPlayerCamera();
                    viewers.ToggleUpgradeMenu(false);
                }
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

        if (enemySpawnerManager != null)
        {
            enemySpawnerManager.SpawnEnemies();
        }

        playerController.canControl = false;
        chat.ToggleChat(false);
    }

    public void StopCharging()
    {
        playerController.EnableArmRender();
        playerController.flashLight.gameObject.SetActive(true);

        isCharging = false;
        hoverEffect.SetActive(true);
        playerController.canControl = true;
        chat.ToggleChat(true);
    }

    private void ChargeBattery()
    {
        if (gameManager != null)
        {
            gameManager.batteryPercentage += chargeRate * Time.deltaTime;
            gameManager.batteryPercentage = Mathf.Clamp(gameManager.batteryPercentage, 0f, 100f);
        }
    }

    public void TriggerCharge()
    {
        SoundManager.instance.PlaySound(rechargeSound);
        StartCharging();
        health.ResetHealthFull();

        if (openUpgradesOnInteract)
        {
            Cursor.SetCursor(cursorFingerTexture, hotspot, cursorMode);
            camManager.SwitchToUpgradeCamera();
            viewers.ToggleUpgradeMenu(true);
        }
    }
}

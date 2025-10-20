using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("PowerUp String Names")]
    public string powerUp1 = "DoubleJump";

    PlayerController player;

    [Header("Battery Settings")]
    [Range(0f, 100f)]
    public float batteryPercentage = 100f;
    public float drainRatePerSecond = 5f; 
    public TextMeshProUGUI batteryText; 
    public BatterySlider batterySlider;
    public Light2D playerLight;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerController>();
        batterySlider = FindAnyObjectByType<BatterySlider>();
        batterySlider.SetMaxBattery(batteryPercentage);
    }

    public void Update()
    {
        if (batteryPercentage > 0f)
        {
            playerLight.enabled = true;
            batteryPercentage -= drainRatePerSecond * Time.deltaTime;
            batteryPercentage = Mathf.Clamp(batteryPercentage, 0f, 100f);
            batterySlider.SetBattery(batteryPercentage);
        }
        else
        {
            playerLight.enabled = false;
        }

        if (batteryText != null)
        {
            batteryText.text = "Battery: " + Mathf.RoundToInt(batteryPercentage) + "%";
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(1);
        }
    }

    public void ReduceBattery(float amount)
    {
        if (batteryPercentage > 0f)
        {
            batteryPercentage -= amount;
            batteryPercentage = Mathf.Clamp(batteryPercentage, 0f, 100f);
            batterySlider.SetBattery(batteryPercentage);
        }
    }


    void OnEnable()
    {
        Inventory.OnItemAdded += CheckForItem; 
    }

    void OnDisable()
    {
        Inventory.OnItemAdded -= CheckForItem; 
    }

    private void CheckForItem(string itemName)
    {
        if (itemName == powerUp1)
        {
            player.hasDoubleJump = true;
        }
    }
}

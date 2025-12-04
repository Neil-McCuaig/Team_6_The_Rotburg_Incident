using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("PowerUp String Names")]
    public string powerUp1 = "DoubleJump";
    public string powerUp2 = "MetalPipe";

    PlayerController player;
    SafeStations safeStations;
    public GameObject batterySliderFill;
    private Image sliderFill;

    [Header("Battery Settings")]
    [Range(0f, 100f)]
    public float batteryPercentage = 100f;
    public float drainRatePerSecond = 5f; 
    public BatterySlider batterySlider;
    public Light2D playerLight;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerController>();
        safeStations = FindAnyObjectByType<SafeStations>();
        batterySlider = FindAnyObjectByType<BatterySlider>();
        batterySlider.SetMaxBattery(batteryPercentage);

        sliderFill = batterySliderFill.GetComponent<Image>();
    }

    public void Update()
    {
        if (batteryPercentage > 0f)
        {
            playerLight.enabled = true;
            batteryPercentage -= drainRatePerSecond * Time.deltaTime;
            batteryPercentage = Mathf.Clamp(batteryPercentage, 0f, 100f);
            batterySlider.SetBattery(batteryPercentage);

            batterySliderFill.SetActive(true);
        }
        else
        {
            playerLight.enabled = false;

            batterySliderFill.SetActive(false);
        }

        if (batteryPercentage > 99f)
        {
            sliderFill.color = Color.green;
        }
        else if (batteryPercentage > 50f)
        {
            sliderFill.color = Color.white;
        }
        else if (batteryPercentage > 20f)
        {
            sliderFill.color = Color.yellow;
        }
        else if (batteryPercentage < 20f)
        {
            sliderFill.color = Color.red;
        }

        if (Input.GetKeyDown(KeyCode.R))
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
        else if (itemName == powerUp2)
        {
            player.hasMetalPipe = true;
        }
    }
}

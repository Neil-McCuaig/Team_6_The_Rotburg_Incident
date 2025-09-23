using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Item String Names")]
    public string powerUp1 = "DoubleJump";

    PlayerController player;

    [Header("Battery Settings")]
    [Range(0f, 100f)]
    public float batteryPercentage = 100f;
    public float drainRatePerSecond = 5f; 
    public TextMeshProUGUI batteryText; 

    private void Start()
    {
        player = FindAnyObjectByType<PlayerController>();
    }

    public void Update()
    {
        if (batteryPercentage > 0f)
        {
            batteryPercentage -= drainRatePerSecond * Time.deltaTime;
            batteryPercentage = Mathf.Clamp(batteryPercentage, 0f, 100f);
        }
        else
        {
            Debug.Log("Battery Empty");
        }

        if (batteryText != null)
        {
            batteryText.text = "Battery: " + Mathf.RoundToInt(batteryPercentage) + "%";
        }
    }

    public void ReduceBattery(float amount)
    {
        if (batteryPercentage > 0f)
        {
            batteryPercentage -= amount;
            batteryPercentage = Mathf.Clamp(batteryPercentage, 0f, 100f);
        }

        if (batteryPercentage <= 0f)
        {
            Debug.Log("Battery Empty");
        }
    }


    void OnEnable()
    {
        Inventory.OnItemAdded += CheckForItem; // Subscribe to the event
    }

    void OnDisable()
    {
        Inventory.OnItemAdded -= CheckForItem; // Unsubscribe to avoid memory leaks
    }

    // Check if the required item was added
    private void CheckForItem(string itemName)
    {
        if (itemName == powerUp1)
        {
            player.hasDoubleJump = true;
        }
    }
}

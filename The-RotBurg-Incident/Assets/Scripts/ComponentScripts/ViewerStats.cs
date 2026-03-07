using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ViewerStats : MonoBehaviour
{
    public static ViewerStats Instance;

    [Header("Viewer Currency")]
    public int viewers;
    public int displayedViewers;
    public float countSpeed = 5;

    [Header("Player Stats")]
    public float batteryDrainRate = 2;

    [Header("UI Elements")]
    public TextMeshProUGUI viewerText;
    public TextMeshProUGUI menuViewerText;
    public RectTransform upgradeMenu;
    public TextMeshProUGUI batteryDrainLevelText;
    public TextMeshProUGUI batteryDrainCostText;

    [Header("Upgrade Settings")]
    public int batteryDrainLevel = 0;
    public int batteryDrainMaxLevel = 5;
    public int batteryDrainCost = 50;
    public int batteryDrainCostIncrase = 50;
    public float batteryDrainDecrease = 0.3f;

    void Start()
    {
        batteryDrainLevelText.text = batteryDrainLevel + " / " + batteryDrainMaxLevel;
        batteryDrainCostText.text = "Cost: " + batteryDrainCost;
        displayedViewers = viewers;
        UpdateViewerUI();
    }

    private void Update()
    {
        displayedViewers = (int)Mathf.MoveTowards(displayedViewers, viewers, countSpeed * Time.deltaTime);

        UpdateViewerUI();
    }

    public void AddViewers(int amount)
    {
        viewers += amount;
        Debug.Log("Viewers gained: " + amount + " | Total: " + viewers);
    }

    bool SpendViewers(int amount)
    {
        if (viewers >= amount)
        {
            viewers -= amount;
            return true;
        }

        Debug.Log("Not enough viewers");
        return false;
    }

    void UpdateViewerUI()
    {
        if (viewerText != null && menuViewerText != null)
        {
            viewerText.text = "LIVE • " + displayedViewers.ToString("N0") + " viewers";
            menuViewerText.text = displayedViewers.ToString("N0") + " viewers";
        }
    }

    public void ToggleUpgradeMenu(bool menuToggle)
    {
        if (upgradeMenu != null)
        {
            upgradeMenu.gameObject.SetActive(menuToggle);
        }
    }

    public void BuyBatteryDrainUpgrade()
    {
        if (batteryDrainLevel >= batteryDrainMaxLevel)
        {
            Debug.Log("Battery upgrade maxed");
            return;
        }

        if (SpendViewers(batteryDrainCost))
        {
            batteryDrainRate -= batteryDrainDecrease;
            batteryDrainLevel++;
            batteryDrainCost += batteryDrainCostIncrase;
            batteryDrainLevelText.text = batteryDrainLevel + "/" + batteryDrainMaxLevel;
            batteryDrainCostText.text = "Cost: " + batteryDrainCost;

            Debug.Log("Battery upgraded to level " + batteryDrainLevel);
        }
    }
}

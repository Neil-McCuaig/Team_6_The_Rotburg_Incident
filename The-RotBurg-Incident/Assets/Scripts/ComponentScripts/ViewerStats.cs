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
    public float maxAttackAmount = 1;
    public float knockbackAmount = 50;

    [Header("UI Elements")]
    public TextMeshProUGUI viewerText;
    public TextMeshProUGUI menuViewerText;
    public RectTransform upgradeMenu;

    [Header("UI Drain Rate")]
    public TextMeshProUGUI batteryDrainLevelText;
    public TextMeshProUGUI batteryDrainCostText;

    [Header("UI Num of Attacks")]
    public TextMeshProUGUI comboAmountLevelText;
    public TextMeshProUGUI comboAmountCostText;

    [Header("UI Attack Knockback")]
    public TextMeshProUGUI knockbackLevelText;
    public TextMeshProUGUI knockbackCostText;

    [Header("Drain Rate Upgrades")]
    public int batteryDrainLevel;
    public int batteryDrainMaxLevel;
    public int batteryDrainCost;
    public int batteryDrainCostIncrease;
    public float batteryDrainDecrease;

    [Header("Combo Amount Upgrades")]
    public int comboAmountLevel;
    public int comboAmountMaxLevel;
    public int comboAmountCost;
    public int comboAmountCostIncrease;
    public int comboAmountIncrease;

    [Header("Attack Knockback Upgrades")]
    public int knockbackLevel;
    public int knockbackMaxLevel;
    public int knockbackCost;
    public int knockbackCostIncrease;
    public int knockbackIncrease;

    void Start()
    {
        batteryDrainLevelText.text = batteryDrainLevel + "/" + batteryDrainMaxLevel;
        batteryDrainCostText.text = "Cost: " + batteryDrainCost;
        comboAmountLevelText.text = comboAmountLevel + "/" + comboAmountMaxLevel;
        comboAmountCostText.text = "Cost: " + comboAmountCost;
        knockbackLevelText.text = knockbackLevel + "/" + knockbackMaxLevel;
        knockbackCostText.text = "Cost: " + knockbackCost;

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
        if (batteryDrainLevel >= batteryDrainMaxLevel - 1)
        {
            batteryDrainLevelText.text = "Maxed ";
            batteryDrainCostText.text = "";
            return;
        }

        if (SpendViewers(batteryDrainCost))
        {
            batteryDrainRate -= batteryDrainDecrease;
            batteryDrainLevel++;
            batteryDrainCost += batteryDrainCostIncrease;
            batteryDrainLevelText.text = batteryDrainLevel + "/" + batteryDrainMaxLevel;
            batteryDrainCostText.text = "Cost: " + batteryDrainCost;

            Debug.Log("Battery upgraded to level " + batteryDrainLevel);
        }
    }

    public void BuyComboAmountUpgrade()
    {
        if (comboAmountLevel >= comboAmountMaxLevel - 1)
        {
            comboAmountLevelText.text = "Maxed ";
            comboAmountCostText.text = "";
            return;
        }

        if (SpendViewers(comboAmountCost))
        {
            maxAttackAmount += comboAmountIncrease;
            comboAmountLevel++;
            comboAmountCost += comboAmountCostIncrease;
            comboAmountLevelText.text = comboAmountLevel + "/" + comboAmountMaxLevel;
            comboAmountCostText.text = "Cost: " + comboAmountCost;

            Debug.Log("Combo amount upgraded to level " + comboAmountLevel);
        }
    }

    public void BuyKnockbackUpgrade()
    {
        if (knockbackLevel >= knockbackMaxLevel - 1)
        {
            knockbackLevelText.text = "Maxed ";
            knockbackCostText.text = "";
            return;
        }

        if (SpendViewers(knockbackCost))
        {
            knockbackAmount += knockbackIncrease;
            knockbackLevel++;
            knockbackCost += knockbackCostIncrease;
            knockbackLevelText.text = knockbackLevel + "/" + knockbackMaxLevel;
            knockbackCostText.text = "Cost: " + knockbackCost;

            Debug.Log("Combo amount upgraded to level " + knockbackLevel);
        }
    }
}

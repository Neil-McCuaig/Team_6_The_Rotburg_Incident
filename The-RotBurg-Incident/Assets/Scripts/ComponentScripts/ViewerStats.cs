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
    public float flashDrainRate = 10;
    public float personalLightRadius = 28;
    public float maxPlayerHealth = 25;

    [Header("UI Elements")]
    public TextMeshProUGUI viewerText;
    public TextMeshProUGUI menuViewerText;
    public RectTransform upgradeMenu;

    [Header("Drain Rate Upgrades")]
    public TextMeshProUGUI batteryDrainLevelText;
    public TextMeshProUGUI batteryDrainCostText;
    public int batteryDrainLevel;
    public int batteryDrainMaxLevel;
    public int batteryDrainCost;
    public int batteryDrainCostIncrease;
    public float batteryDrainDecrease;

    [Header("Flash Drain Upgrades")]
    public TextMeshProUGUI flashDrainLevelText;
    public TextMeshProUGUI flashDrainCostText;
    public int flashDrainLevel;
    public int flashDrainMaxLevel;
    public int flashDrainCost;
    public int flashDrainCostIncrease;
    public float flashDrainDecrease;

    [Header("Combo Amount Upgrades")]
    public TextMeshProUGUI comboAmountLevelText;
    public TextMeshProUGUI comboAmountCostText;
    public int comboAmountLevel;
    public int comboAmountMaxLevel;
    public int comboAmountCost;
    public int comboAmountCostIncrease;
    public int comboAmountIncrease;

    [Header("Attack Knockback Upgrades")]
    public TextMeshProUGUI knockbackLevelText;
    public TextMeshProUGUI knockbackCostText;
    public int knockbackLevel;
    public int knockbackMaxLevel;
    public int knockbackCost;
    public int knockbackCostIncrease;
    public int knockbackIncrease;

    [Header("Personal Light Radius Upgrades")]
    public TextMeshProUGUI lightRadiusLevelText;
    public TextMeshProUGUI lightRadiusCostText;
    public int lightRadiusLevel;
    public int lightRadiusMaxLevel;
    public int lightRadiusCost;
    public int lightRadiusCostIncrease;
    public int lightRadiusIncrease;

    [Header("Max Player Health Upgrades")]
    public TextMeshProUGUI maxHealthLevelText;
    public TextMeshProUGUI maxHealthCostText;
    public int maxHealthLevel;
    public int maxHealthMaxLevel;
    public int maxHealthCost;
    public int maxHealthCostIncrease;
    public int maxHealthIncrease;

    void Start()
    {
        batteryDrainLevelText.text = batteryDrainLevel + "/" + batteryDrainMaxLevel;
        batteryDrainCostText.text = "Cost: " + batteryDrainCost;

        comboAmountLevelText.text = comboAmountLevel + "/" + comboAmountMaxLevel;
        comboAmountCostText.text = "Cost: " + comboAmountCost;

        knockbackLevelText.text = knockbackLevel + "/" + knockbackMaxLevel;
        knockbackCostText.text = "Cost: " + knockbackCost;

        flashDrainLevelText.text = flashDrainLevel + "/" + flashDrainMaxLevel;
        flashDrainCostText.text = "Cost: " + flashDrainCost;

        lightRadiusLevelText.text = lightRadiusLevel + "/" + lightRadiusMaxLevel;
        lightRadiusCostText.text = "Cost: " + lightRadiusCost;

        maxHealthLevelText.text = maxHealthLevel + "/" + maxHealthMaxLevel;
        maxHealthCostText.text = "Cost: " + maxHealthCost;

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
        if (comboAmountLevel >= comboAmountMaxLevel)
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
        if (knockbackLevel >= knockbackMaxLevel)
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

    public void BuyFlashDrainRateUpgrade()
    {
        if (flashDrainLevel >= flashDrainMaxLevel)
        {
            flashDrainLevelText.text = "Maxed ";
            flashDrainCostText.text = "";
            return;
        }

        if (SpendViewers(flashDrainCost))
        {
            flashDrainRate -= flashDrainDecrease;
            flashDrainLevel++;
            flashDrainCost += flashDrainCostIncrease;
            flashDrainLevelText.text = flashDrainLevel + "/" + flashDrainMaxLevel;
            flashDrainCostText.text = "Cost: " + flashDrainCost;

            Debug.Log("Combo amount upgraded to level " + flashDrainLevel);
        }
    }

    public void BuyLightRadiusUpgrade()
    {
        if (lightRadiusLevel >= lightRadiusMaxLevel)
        {
            lightRadiusLevelText.text = "Maxed ";
            lightRadiusCostText.text = "";
            return;
        }

        if (SpendViewers(lightRadiusCost))
        {
            personalLightRadius += lightRadiusIncrease;
            lightRadiusLevel++;
            lightRadiusCost += lightRadiusCostIncrease;
            lightRadiusLevelText.text = lightRadiusLevel + "/" + lightRadiusMaxLevel;
            lightRadiusCostText.text = "Cost: " + lightRadiusCost;

            Debug.Log("Combo amount upgraded to level " + lightRadiusLevel);
        }
    }

    public void BuyMaxHealthUpgrade()
    {
        if (maxHealthLevel >= maxHealthMaxLevel)
        {
            maxHealthLevelText.text = "Maxed ";
            maxHealthCostText.text = "";
            return;
        }

        if (SpendViewers(maxHealthCost))
        {
            maxPlayerHealth += maxHealthIncrease;
            maxHealthLevel++;
            maxHealthCost += maxHealthCostIncrease;
            maxHealthLevelText.text = maxHealthLevel + "/" + maxHealthMaxLevel;
            maxHealthCostText.text = "Cost: " + maxHealthCost;

            Debug.Log("Combo amount upgraded to level " + maxHealthLevel);
        }
    }
}

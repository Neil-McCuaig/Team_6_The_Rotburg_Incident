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
    public TextMeshProUGUI batteryDrainCostText;
    public Animator batteryDrainAnim;
    public int batteryDrainLevel;
    public int batteryDrainMaxLevel;
    public int batteryDrainCost;
    public int batteryDrainCostIncrease;
    public float batteryDrainDecrease;

    [Header("Flash Drain Upgrades")]
    public TextMeshProUGUI flashDrainCostText;
    public Animator flashDrainAnim;
    public int flashDrainLevel;
    public int flashDrainMaxLevel;
    public int flashDrainCost;
    public int flashDrainCostIncrease;
    public float flashDrainDecrease;

    [Header("Combo Amount Upgrades")]
    public TextMeshProUGUI comboAmountCostText;
    public Animator comboAmountAnim;
    public int comboAmountLevel;
    public int comboAmountMaxLevel;
    public int comboAmountCost;
    public int comboAmountCostIncrease;
    public int comboAmountIncrease;

    [Header("Attack Knockback Upgrades")]
    public TextMeshProUGUI knockbackCostText;
    public Animator knockbackAnim;
    public int knockbackLevel;
    public int knockbackMaxLevel;
    public int knockbackCost;
    public int knockbackCostIncrease;
    public int knockbackIncrease;

    [Header("Personal Light Radius Upgrades")]
    public TextMeshProUGUI lightRadiusCostText;
    public Animator lightRadiusAnim;
    public int lightRadiusLevel;
    public int lightRadiusMaxLevel;
    public int lightRadiusCost;
    public int lightRadiusCostIncrease;
    public int lightRadiusIncrease;

    [Header("Max Player Health Upgrades")]
    public TextMeshProUGUI maxHealthCostText;
    public Animator maxHealthAnim;
    public int maxHealthLevel;
    public int maxHealthMaxLevel;
    public int maxHealthCost;
    public int maxHealthCostIncrease;
    public int maxHealthIncrease;

    void Start()
    {
        batteryDrainCostText.text = "Cost: " + batteryDrainCost;
        comboAmountCostText.text = "Cost: " + comboAmountCost;
        knockbackCostText.text = "Cost: " + knockbackCost;
        flashDrainCostText.text = "Cost: " + flashDrainCost;
        lightRadiusCostText.text = "Cost: " + lightRadiusCost;
        maxHealthCostText.text = "Cost: " + maxHealthCost;

        displayedViewers = viewers;
    }

    private void Update()
    {
        displayedViewers = (int)Mathf.MoveTowards(displayedViewers, viewers, countSpeed * Time.deltaTime);

        UpdateViewerUI();

        if(batteryDrainAnim && flashDrainAnim && lightRadiusAnim && maxHealthAnim && knockbackAnim && comboAmountAnim != null)
        {
            UpdateUIAnimations();
        }
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

        if (batteryDrainCostText != null && batteryDrainLevel >= batteryDrainMaxLevel)
        {
            batteryDrainCostText.text = "";
        }
        if (flashDrainCostText != null && flashDrainLevel >= flashDrainMaxLevel)
        {
            flashDrainCostText.text = "";
        }
        if (lightRadiusCostText != null && lightRadiusLevel >= lightRadiusMaxLevel)
        {
            lightRadiusCostText.text = "";
        }
        if (maxHealthCostText != null && maxHealthLevel >= maxHealthMaxLevel)
        {
            maxHealthCostText.text = "";
        }
        if (knockbackCostText != null && knockbackLevel >= knockbackMaxLevel)
        {
            knockbackCostText.text = "";
        }
        if (comboAmountCostText != null && comboAmountLevel >= comboAmountMaxLevel)
        {
            comboAmountCostText.text = "";
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
            return;
        }

        if (SpendViewers(batteryDrainCost))
        {
            batteryDrainRate -= batteryDrainDecrease;
            batteryDrainLevel++;
            batteryDrainCost += batteryDrainCostIncrease;
            batteryDrainCostText.text = "Cost: " + batteryDrainCost;

            Debug.Log("Battery upgraded to level " + batteryDrainLevel);
        }
    }

    public void BuyComboAmountUpgrade()
    {
        if (comboAmountLevel >= comboAmountMaxLevel)
        {
            return;
        }

        if (SpendViewers(comboAmountCost))
        {
            maxAttackAmount += comboAmountIncrease;
            comboAmountLevel++;
            comboAmountCost += comboAmountCostIncrease;
            comboAmountCostText.text = "Cost: " + comboAmountCost;

            Debug.Log("Combo amount upgraded to level " + comboAmountLevel);
        }
    }

    public void BuyKnockbackUpgrade()
    {
        if (knockbackLevel >= knockbackMaxLevel)
        {
            return;
        }

        if (SpendViewers(knockbackCost))
        {
            knockbackAmount += knockbackIncrease;
            knockbackLevel++;
            knockbackCost += knockbackCostIncrease;
            knockbackCostText.text = "Cost: " + knockbackCost;

            Debug.Log("Combo amount upgraded to level " + knockbackLevel);
        }
    }

    public void BuyFlashDrainRateUpgrade()
    {
        if (flashDrainLevel >= flashDrainMaxLevel)
        {
            return;
        }

        if (SpendViewers(flashDrainCost))
        {
            flashDrainRate -= flashDrainDecrease;
            flashDrainLevel++;
            flashDrainCost += flashDrainCostIncrease;
            flashDrainCostText.text = "Cost: " + flashDrainCost;

            Debug.Log("Combo amount upgraded to level " + flashDrainLevel);
        }
    }

    public void BuyLightRadiusUpgrade()
    {
        if (lightRadiusLevel >= lightRadiusMaxLevel)
        {
            return;
        }

        if (SpendViewers(lightRadiusCost))
        {
            personalLightRadius += lightRadiusIncrease;
            lightRadiusLevel++;
            lightRadiusCost += lightRadiusCostIncrease;
            lightRadiusCostText.text = "Cost: " + lightRadiusCost;

            Debug.Log("Combo amount upgraded to level " + lightRadiusLevel);
        }
    }

    public void BuyMaxHealthUpgrade()
    {
        if (maxHealthLevel >= maxHealthMaxLevel)
        {
            return;
        }

        if (SpendViewers(maxHealthCost))
        {
            maxPlayerHealth += maxHealthIncrease;
            maxHealthLevel++;
            maxHealthCost += maxHealthCostIncrease;
            maxHealthCostText.text = "Cost: " + maxHealthCost;

            Debug.Log("Combo amount upgraded to level " + maxHealthLevel);
        }
    }

    public void UpdateUIAnimations()
    {
        batteryDrainAnim.SetInteger("UpgradeLevel", batteryDrainLevel);
        flashDrainAnim.SetInteger("UpgradeLevel", flashDrainLevel);
        lightRadiusAnim.SetInteger("UpgradeLevel", lightRadiusLevel);
        maxHealthAnim.SetInteger("UpgradeLevel", maxHealthLevel);
        knockbackAnim.SetInteger("UpgradeLevel", knockbackLevel);
        comboAmountAnim.SetInteger("UpgradeLevel", comboAmountLevel);
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ViewerStats : MonoBehaviour
{
    public static ViewerStats Instance;

    [Header("Viewer Currency")]
    public int viewers;
    public int displayedViewers;
    public float countSpeed = 5;

    [Header("Player Stats Base Values")]
    public float baseBatteryDrainRate;
    public float baseMaxAttackAmount;
    public float baseKnockbackAmount;
    public float baseFlashDrainRate;
    public float basePersonalLightRadius;
    public float baseMaxPlayerHealth;

    [Header("Player Stats Current Values")]
    public float batteryDrainRate;
    public float maxAttackAmount;
    public float knockbackAmount;
    public float flashDrainRate;
    public float personalLightRadius;
    public float maxPlayerHealth;

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

    [Header("Event System GameObjects")]
    public GameObject firstUpgradeMenuButton;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ApplyUpgradeValues();
        UpdateAllUI();
        displayedViewers = viewers;
    }

    void Update()
    {
        displayedViewers = (int)Mathf.MoveTowards(displayedViewers, viewers, countSpeed * Time.deltaTime);
        UpdateViewerUI();

        if (batteryDrainAnim && flashDrainAnim && lightRadiusAnim && maxHealthAnim && knockbackAnim && comboAmountAnim != null)
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

        batteryDrainCostText.text = batteryDrainLevel >= batteryDrainMaxLevel ? "" : "Cost: " + batteryDrainCost;
        flashDrainCostText.text = flashDrainLevel >= flashDrainMaxLevel ? "" : "Cost: " + flashDrainCost;
        lightRadiusCostText.text = lightRadiusLevel >= lightRadiusMaxLevel ? "" : "Cost: " + lightRadiusCost;
        maxHealthCostText.text = maxHealthLevel >= maxHealthMaxLevel ? "" : "Cost: " + maxHealthCost;
        knockbackCostText.text = knockbackLevel >= knockbackMaxLevel ? "" : "Cost: " + knockbackCost;
        comboAmountCostText.text = comboAmountLevel >= comboAmountMaxLevel ? "" : "Cost: " + comboAmountCost;
    }

    public void ToggleUpgradeMenu(bool menuToggle)
    {
        if (upgradeMenu != null)
        {
            upgradeMenu.gameObject.SetActive(menuToggle);

            if(menuToggle)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(firstUpgradeMenuButton);
            }
        }
    }

    public void BuyBatteryDrainUpgrade() => BuyUpgrade(ref batteryDrainLevel, batteryDrainMaxLevel, ref batteryDrainCost, batteryDrainCostIncrease, batteryDrainDecrease, true, "Battery upgraded to level ");
    public void BuyFlashDrainRateUpgrade() => BuyUpgrade(ref flashDrainLevel, flashDrainMaxLevel, ref flashDrainCost, flashDrainCostIncrease, flashDrainDecrease, true, "Flash drain upgraded to level ");
    public void BuyComboAmountUpgrade() => BuyUpgrade(ref comboAmountLevel, comboAmountMaxLevel, ref comboAmountCost, comboAmountCostIncrease, comboAmountIncrease, false, "Combo amount upgraded to level ");
    public void BuyKnockbackUpgrade() => BuyUpgrade(ref knockbackLevel, knockbackMaxLevel, ref knockbackCost, knockbackCostIncrease, knockbackIncrease, false, "Knockback upgraded to level ");
    public void BuyLightRadiusUpgrade() => BuyUpgrade(ref lightRadiusLevel, lightRadiusMaxLevel, ref lightRadiusCost, lightRadiusCostIncrease, lightRadiusIncrease, false, "Light radius upgraded to level ");
    public void BuyMaxHealthUpgrade() => BuyUpgrade(ref maxHealthLevel, maxHealthMaxLevel, ref maxHealthCost, maxHealthCostIncrease, maxHealthIncrease, false, "Max health upgraded to level ");

    void BuyUpgrade(ref int level, int maxLevel, ref int cost, int costIncrease, float statIncrease, bool isDecrease, string debugMessage)
    {
        if (level >= maxLevel)
        {
            return;
        }

        if (SpendViewers(cost))
        {
            level++;
            cost += costIncrease;
            ApplyUpgradeValues();
            Debug.Log(debugMessage + level);
        }
    }

    void ApplyUpgradeValues()
    {
        batteryDrainRate = baseBatteryDrainRate - (batteryDrainDecrease * batteryDrainLevel);
        flashDrainRate = baseFlashDrainRate - (flashDrainDecrease * flashDrainLevel);
        maxAttackAmount = baseMaxAttackAmount + (comboAmountIncrease * comboAmountLevel);
        knockbackAmount = baseKnockbackAmount + (knockbackIncrease * knockbackLevel);
        personalLightRadius = basePersonalLightRadius + (lightRadiusIncrease * lightRadiusLevel);
        maxPlayerHealth = baseMaxPlayerHealth + (maxHealthIncrease * maxHealthLevel);
    }

    void UpdateUIAnimations()
    {
        batteryDrainAnim.SetInteger("UpgradeLevel", batteryDrainLevel);
        flashDrainAnim.SetInteger("UpgradeLevel", flashDrainLevel);
        lightRadiusAnim.SetInteger("UpgradeLevel", lightRadiusLevel);
        maxHealthAnim.SetInteger("UpgradeLevel", maxHealthLevel);
        knockbackAnim.SetInteger("UpgradeLevel", knockbackLevel);
        comboAmountAnim.SetInteger("UpgradeLevel", comboAmountLevel);
    }

    void UpdateAllUI()
    {
        UpdateViewerUI();
        UpdateUIAnimations();
    }
}

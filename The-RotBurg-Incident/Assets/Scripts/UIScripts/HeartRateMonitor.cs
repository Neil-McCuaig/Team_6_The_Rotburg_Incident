using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeartRateMonitor : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI BPMText;
    public Image heartRate;
    PlayerHealth health;

    [Header("Float Control")]
    public float displayFloat = 0f;    
    public float incrementSpeed = 50f; 
    private float targetValue = 0f;

    private void Start()
    {
        health = FindAnyObjectByType<PlayerHealth>();
    }
    void Update()
    {
        UpdateTargetValue();
        displayFloat = Mathf.MoveTowards(displayFloat, targetValue, incrementSpeed * Time.deltaTime);
        if (BPMText != null)
        {
            BPMText.text = displayFloat.ToString("F0");
        }
    }

    void UpdateTargetValue()
    {
        float hpPercent = (health.currentHealth / health.maxHealth) * 100f;
        if (heartRate != null)
        {
            if (hpPercent <= 0)
            {
                displayFloat = 0f;
                heartRate.color = Color.red;
            }
            else if (hpPercent <= 10)
            {
                targetValue = 220f;
                heartRate.color = Color.red;
            }
            else if (hpPercent <= 30)
            {
                targetValue = 190f;
                heartRate.color = Color.magenta;
            }
            else if (hpPercent <= 50)
            {
                targetValue = 160f;
                heartRate.color = Color.yellow;
            }
            else if (hpPercent <= 70)
            {
                targetValue = 110f;
            }
            else
            {
                targetValue = 70f;
                heartRate.color = Color.green;
            }
        }
    }
}

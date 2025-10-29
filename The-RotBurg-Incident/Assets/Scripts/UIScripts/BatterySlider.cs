using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatterySlider : MonoBehaviour
{
    public Slider slider;

    public void SetMaxBattery(float percentage)
    {
        slider.maxValue = percentage;
        slider.value = percentage;
    }

    public void SetBattery(float percentage)
    {
        slider.value = percentage;
    }
}

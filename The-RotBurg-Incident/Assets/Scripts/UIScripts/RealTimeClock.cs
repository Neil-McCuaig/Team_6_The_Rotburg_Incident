using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RealTimeClock : MonoBehaviour
{
    public float timeOfDay;
    public TextMeshProUGUI batteryText;
    public string formattedTime;

    void Update()
    {
        DateTime now = DateTime.Now;
        formattedTime = now.ToString("h:mm");
        batteryText.text = formattedTime;
    }
}

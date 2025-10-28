using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RealTimeClock : MonoBehaviour
{
    public float timeOfDay;
    public TextMeshProUGUI batteryText;

    void Update()
    {
        DateTime now = DateTime.Now;
        string formattedTime = now.ToString("h:mm");
        batteryText.text = formattedTime;
    }
}

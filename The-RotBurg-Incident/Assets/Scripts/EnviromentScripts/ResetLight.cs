using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ResetLight : MonoBehaviour
{
    private Light2D light2D;
    private Color defaultColor;

    void Awake()
    {
        light2D = GetComponent<Light2D>();
        defaultColor = light2D.color;
    }

    public void ResetLightColor()
    {
        light2D.color = defaultColor;
    }
}

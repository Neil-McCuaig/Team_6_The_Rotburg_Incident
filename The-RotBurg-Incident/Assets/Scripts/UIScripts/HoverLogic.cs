using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverLogic : MonoBehaviour
{
    public bool currentlyHovering = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMouseEnter()
    {
        currentlyHovering = true;
    }

    public void OnMouseExit()
    {
        currentlyHovering = false;
    }
}

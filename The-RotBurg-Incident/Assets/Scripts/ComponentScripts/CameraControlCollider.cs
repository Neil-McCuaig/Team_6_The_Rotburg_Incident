using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlCollider : MonoBehaviour
{
    private Collider2D coll;
    public PanDirection panDirection;
    public float panDistance = 3f;
    public float panTime = 0.35f;
    void Start()
    {
        coll = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(CameraManager.instance != null)
            {
                CameraManager.instance.PanCameraOnContact(panDistance, panTime, panDirection, false);
            }            
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (CameraManager.instance != null)
            {
                CameraManager.instance.PanCameraOnContact(panDistance, panTime, panDirection, true);
            }
        }
    }
}

public enum PanDirection
{
    Up, Down, Left, Right
}

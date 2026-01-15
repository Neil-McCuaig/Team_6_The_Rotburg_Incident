using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightHitBox : MonoBehaviour
{
    WeepingAngelBehavior weepingAngel;

    private void Start()
    {
        weepingAngel = FindAnyObjectByType<WeepingAngelBehavior>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Weeping"))
        {
            if (weepingAngel != null)
            {
                weepingAngel.isInLight = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Weeping"))
        {
            weepingAngel.isInLight = false;
        }
    }
}

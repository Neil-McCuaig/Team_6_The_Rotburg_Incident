using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DomainZoneLogic : MonoBehaviour
{
    [Header("Objects To Activate")]
    [SerializeField] private GameObject[] targets;

    PlayerController playerController;

    private void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }
        foreach (GameObject obj in targets)
        {
            MonnsterActivation activatable = obj.GetComponent<MonnsterActivation>();
            if (activatable != null)
            {
                activatable.SetActiveState(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerController != null && playerController.inLocker == false)
            {
                foreach (GameObject obj in targets)
                {
                    MonnsterActivation activatable = obj.GetComponent<MonnsterActivation>();
                    if (activatable != null)
                    {
                        activatable.SetActiveState(false);
                    }
                }
            }
            else
            {
                return;
            }
        }
    }
}

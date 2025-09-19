using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Item String Names")]
    public string powerUp1 = "DoubleJump";
    PlayerController player;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerController>();
    }

    void OnEnable()
    {
        Inventory.OnItemAdded += CheckForItem; // Subscribe to the event
    }

    void OnDisable()
    {
        Inventory.OnItemAdded -= CheckForItem; // Unsubscribe to avoid memory leaks
    }

    // Check if the required item was added
    private void CheckForItem(string itemName)
    {
        if (itemName == powerUp1)
        {
            player.hasDoubleJump = true;
        }
    }
}

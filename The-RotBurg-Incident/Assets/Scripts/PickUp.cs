using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    private Inventory inventory;  // Reference to the Inventory script

    void Start()
    {
        // Get the Inventory component from the GameObject where it's attached
        inventory = FindAnyObjectByType<Inventory>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Check if the collider belongs to the player
        {
            // Convert name to string
            string itemName = gameObject.name;
            // Add the item to the inventory 
            inventory.AddItem(itemName);
            // Destroy the key object when collected
            Destroy(gameObject);
        }
    }
}

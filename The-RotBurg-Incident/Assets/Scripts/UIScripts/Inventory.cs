using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static event Action<string> OnItemAdded; // Event when an item is added
    private List<string> items = new List<string>();

    // Method to add an item to the inventory
    public void AddItem(string itemName)
    {
        items.Add(itemName);
        Debug.Log($"Item Added: {itemName}");

        // Trigger the event after an item is added
        OnItemAdded?.Invoke(itemName);
    }
}

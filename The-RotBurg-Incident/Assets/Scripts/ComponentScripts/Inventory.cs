using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static event Action<string> OnItemAdded; 
    private List<string> items = new List<string>();

    public void AddItem(string itemName)
    {
        items.Add(itemName);
        Debug.Log($"Item Added: {itemName}");
        OnItemAdded?.Invoke(itemName);
    }
}

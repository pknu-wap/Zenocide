using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public GameObject inventoryPanel;
    bool activeInventory = false;
    private void Start()
    {
        inventoryPanel.SetActive(activeInventory);
    }
    public void ToggleInventory()
    {
        activeInventory = !activeInventory;
        inventoryPanel.SetActive(activeInventory);
    }
}
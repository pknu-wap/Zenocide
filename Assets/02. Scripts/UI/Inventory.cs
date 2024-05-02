using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameObject invnetoryPanel;
    bool toggleInvenyory = false;
    void Start()
    {
        invnetoryPanel.SetActive(toggleInvenyory);
    }

    void Update()
    {
        
    }
    public void ToggleInventory()
    {
        toggleInvenyory = !toggleInvenyory;
        invnetoryPanel.SetActive(toggleInvenyory);
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using UnityEngine.UIElements;

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

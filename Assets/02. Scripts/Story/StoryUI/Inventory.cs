using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameObject inventoryPanel;
    bool activeInventory = false;
    public List<TMP_Text> slots = new List<TMP_Text>();
    private void Start()
    {
        slots = transform.GetChild(0).GetChild(0).GetChild(0).GetComponentsInChildren<TMP_Text>().ToList();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Option : MonoBehaviour
{
    public GameObject OptionPanel;
    bool activeInventory = false;
    void Start()
    {
        OptionPanel.SetActive(activeInventory);
    }

    public void ToggleInventory()
    {
        activeInventory = !activeInventory;
        OptionPanel.SetActive(activeInventory);
    }
}

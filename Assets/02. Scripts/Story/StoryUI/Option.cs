using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Option : MonoBehaviour
{
    public GameObject OptionPanel;
    bool activeOption = false;
    void Start()
    {
        OptionPanel.SetActive(activeOption);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            activeOption = !activeOption;
            OptionPanel.SetActive(activeOption);
        }
    }
    public void ToggleOption()
    {
        activeOption = !activeOption;
        OptionPanel.SetActive(activeOption);
    }
}

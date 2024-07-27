using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionTabMenu : MonoBehaviour
{
    public GameObject[] Tabs;
    public Image[] TabButtons;
    public Sprite DeselectedTabBG, SelectedTabBG;
    public Vector2 DeselectedTabButtonSize, SelectedTabButtonSize;
    private void Start()
    {
        SwitchToTab(0);
    }

    public void SwitchToTab(int TabID)
    {
        for (int i = 0; i < Tabs.Length; i++)
        {
            Tabs[i].SetActive(false);
        }

        Tabs[TabID].SetActive(true);

        for (int i = 0; i < TabButtons.Length; i++)
        {
            TabButtons[i].sprite = DeselectedTabBG;
            TabButtons[i].rectTransform.sizeDelta = DeselectedTabButtonSize;
        }

        TabButtons[TabID].sprite = SelectedTabBG;
        TabButtons[TabID].rectTransform.sizeDelta = SelectedTabButtonSize;
    }
}

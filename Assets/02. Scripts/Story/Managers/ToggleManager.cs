using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleManager : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject deckPanel;
    public GameObject optionPanel;

    private bool activeInventory = false;
    private bool activeDeck = false;
    private bool activeOption = false;

    public ResolutionManager resolutionManager;
    public SoundManager soundManager;

    public AudioClip deckButton;
    public AudioClip backpackButton;
    public AudioClip optionButton;

    void Start()
    {
        inventoryPanel.SetActive(activeInventory);
        deckPanel.SetActive(activeDeck);
        optionPanel.SetActive(activeOption);
    }

    void Update()
    {
        // Panel이 켜진 상태에서 ESC 누르면 꺼짐
        if (activeInventory == true && Input.GetKeyDown(KeyCode.Escape))
        {
            activeInventory = !activeInventory;
            inventoryPanel.SetActive(activeInventory);
        }
        if (activeDeck == true && Input.GetKeyDown(KeyCode.Escape))
        {
            activeDeck = !activeDeck;
            deckPanel.SetActive(activeDeck);
        }
        if (activeOption == true && Input.GetKeyDown(KeyCode.Escape))
        {
            activeOption = !activeOption;
            optionPanel.SetActive(activeOption);
            resolutionManager.CancelResolutionSettings();
            soundManager.CancelVolumeSettings();
        }
    }
    // aPanel이 켜진 상태에서 bPanel을 키면 aPanel 꺼지고 bPanel 켜짐
    public void ToggleInventory()
    {
        SoundManager.Instance.Play(backpackButton);

        if (!activeInventory)
        {
            if (activeDeck)
            {
                ToggleDeck(false);
            }
            if (activeOption)
            {
                ToggleOption(false);
            }
        }
        activeInventory = !activeInventory;
        inventoryPanel.SetActive(activeInventory);
    }
    public void ToggleDeck()
    {
        SoundManager.Instance.Play(deckButton);

        if (!activeDeck)
        {
            if (activeInventory)
            {
                ToggleInventory(false);
            }
            if (activeOption)
            {
                ToggleOption(false); 
            }
        }
        activeDeck = !activeDeck;
        deckPanel.SetActive(activeDeck);
    }
    public void ToggleOption()
    {
        SoundManager.Instance.Play(optionButton);

        if (!activeOption)
        {
            if (activeInventory)
            {
                ToggleInventory(false); 
            }
            if (activeDeck)
            {
                ToggleDeck(false);
            }
        }
        activeOption = !activeOption;
        optionPanel.SetActive(activeOption);
    }
    // Panel이 true면 활성화, false면 비활성화
    public void ToggleInventory(bool state)
    {
        activeInventory = state;
        inventoryPanel.SetActive(activeInventory);
    }

    public void ToggleDeck(bool state)
    {
        activeDeck = state;
        deckPanel.SetActive(activeDeck);
    }

    public void ToggleOption(bool state)
    {
        activeOption = state;
        optionPanel.SetActive(activeOption);
    }
}

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
        // 게임 시작 시 인벤토리 안 보이게
        inventoryPanel.SetActive(activeInventory);
        slots = transform.GetChild(0).GetChild(0).GetChild(0).GetComponentsInChildren<TMP_Text>().ToList();
    }
    public void ToggleInventory()
    {
        // Pointer Enter, Exit시 false -> true, true -> false로 바꾼다
        activeInventory = !activeInventory;
        // activeInventory가 false인지 true인지에 따라 Panel이 보일지 말지 결정한다.
        inventoryPanel.SetActive(activeInventory); 
    }
}
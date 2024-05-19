using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class Inventory : MonoBehaviour
{
    public GameObject inventoryPanel;
    bool activeInventory = false;
    private void Start()
    {
        inventoryPanel.SetActive(activeInventory); // 초기 설정값을 false로 한다.
        slots = transform.GetChild(1).GetChild(0).GetChild(0).GetComponentsInChildren<TMP_Text>().ToList();
    }
    public void ToggleInventory()
    {
        activeInventory = !activeInventory; // Pointer Enter, Exit시 false -> true, true -> false로 바꾼다
        inventoryPanel.SetActive(activeInventory); // activeInventory가 false인지 true인지에 따라 Panel이 보일지 말지 결정한다.
    }
    public static List<string> items = new List<string>();
    public List<TMP_Text> slots = new List<TMP_Text>();
}
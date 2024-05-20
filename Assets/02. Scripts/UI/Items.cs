using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Items : MonoBehaviour
{
    public static Items instance { get; private set; }
    public Transform slotsParent;
    public List<TMP_Text> slots = new List<TMP_Text>();
    public static List<string> items = new List<string>();
    public string newItemName;

    private void Awake() => instance = this;
    private void Start()
    {
        foreach (TMP_Text slot in slotsParent.GetComponentsInChildren<TMP_Text>())
        {
            slots.Add(slot);
            slot.text = "";
        }
        ItemList();
    }
    public void ItemList()
    {
        items.Add("배고픔");
        items.Add("무기");
        items.Add("근력");
        items.Add("속도");
        items.Add("빵");
        items.Add("갈증");
        items.Add("토마토 통조림");
        items.Add("꽁치 통조림");
        items.Add("철학");
        items.Add("책");
        items.Add("붕대");
        items.Add("진통제");
        items.Add("출혈");
        items.Add("버섯");
        items.Add("최상의 식사");
        items.Add("냉동 삼겹살");
        items.Add("영어 실력");
        items.Add("마체테");
        items.Add("총");
        items.Add("총알");
        items.Add("우울증");
    }
    public void AddItem()
    {
        if (items.Count > 0)
        {
            // 첫 번째 비어 있는 슬롯을 찾음
            int slotIndex = slots.FindIndex(slot => string.IsNullOrEmpty(slot.text));
            if (slotIndex != -1)
            {
                // 첫 번째 비어 있는 슬롯부터 순서대로 아이템을 추가
                for (int i = slotIndex; i > 0; i--)
                {
                    slots[i].text = slots[i - 1].text;
                }
                // items 리스트에 있는 랜덤 아이템 슬롯에 추가
                int randomIndex = Random.Range(0, items.Count);
                slots[0].text = items[randomIndex];
                items.RemoveAt(randomIndex);
            }
        }
    }
    public void RemoveItem()
    {
        int slotIndex = slots.FindIndex(slot => !string.IsNullOrEmpty(slot.text));
        // 아이템이 표시된 슬롯을 찾았을 경우
        if (slotIndex != -1)
        {
            // 슬롯에 있는 아이템을 removedItem에 저장한다.
            string removedItem = slots[slotIndex].text;
            // removedItem을 다시 items 리스트에 추가
            items.Add(removedItem);
            // 슬롯에 있는 아이템들을 앞으로 한 칸씩 이동
            for (int i = slotIndex; i < slots.Count - 1; i++)
            {
                slots[i].text = slots[i + 1].text;
            }
        }
    }
}
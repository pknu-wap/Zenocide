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
        // 슬롯에 아이템을 추가할 인덱스 계산
        int slotIndex = slots.FindIndex(slot => string.IsNullOrEmpty(slot.text));

        // 아이템이 남아있는 경우 슬롯에 추가
        if (items.Count > 0)
        {
            // 랜덤으로 선택
            int randomIndex = Random.Range(0, items.Count);
            // 선택된 아이템을 슬롯에 추가하고 리스트에서 제거
            slots[slotIndex].text = items[randomIndex];
            items.RemoveAt(randomIndex);
        }
    }
    public void DeleteItem()
    {
        Debug.Log("없어짐");
    }
}

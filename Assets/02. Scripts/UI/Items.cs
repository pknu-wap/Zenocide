using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Items : MonoBehaviour
{
    public static Items Inst {get; private set;}
    private void Awake() => Inst = this;

    public static List<string> items = new List<string>();
    public List<TMP_Text> slots = new List<TMP_Text>();
    void Start()
    {
        slots = transform.GetChild(1).GetChild(0).GetChild(0).GetComponentsInChildren<TMP_Text>().ToList();
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
        foreach (TMP_Text slot in slots)
        {
            slot.text = "";
        }
    }
    public void AddItem()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (!string.IsNullOrEmpty(slots[i].text))
                continue;
            string randomItem = AddAndRemoveItem();
            if (randomItem == null)
                break;
            slots[i].text = randomItem;
            break;
        }
    }
    private string AddAndRemoveItem()
    {
        if (items.Count == 0)
            return null;
        int randomIndex = Random.Range(0, items.Count);
        string randomItem = items[randomIndex];
        items.RemoveAt(randomIndex);
        return randomItem;
    }
}

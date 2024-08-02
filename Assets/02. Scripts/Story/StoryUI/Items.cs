using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

// 아이템 타입 (능력, 물품)
public enum ItemType
{
    Status,
    Item,
}

// 아이템 구조체
public class Item
{
    // 타입
    public ItemType Type { get; set; }
    // 이름
    public string Name { get; set; }
    // 태그
    public string Tag { get; set; }
    // 개수
    public int Count { get; set; }

    public Item(ItemType type, string name, string tag, int count)
    {
        Type = type;
        Name = name;
        Tag = tag;
        Count = count;
    }
}

public class Items : MonoBehaviour
{
    public static Items Instance { get; private set; }
    public Transform slotsParent;
    // 인벤토리의 슬롯 리스트
    public List<TMP_Text> slots = new List<TMP_Text>();
    // 마지막 슬롯의 번호
    public int lastSlotIndex = -1;
    // 아이템 데이터 리스트
    public List<Item> items = new List<Item>();
    // 에러 아이템 (검색 실패 시 반환됨)
    public Item errorItem = new Item(ItemType.Item, "Error", "Error", 0);

    private void Awake() => Instance = this;

    private void Start()
    {
        // 슬롯을 받아온다.
        slots = slotsParent.GetComponentsInChildren<TMP_Text>().ToList();
        
        // 모든 슬롯을 비활성화한다.
        for(int i = 0; i < slots.Count; i++)
        {
            slots[i].gameObject.SetActive(false);
        }

        // 아이템 데이터를 로드한다.


    #if UNITY_EDITOR
        // 테스트 아이템 추가
        TestItem();
    #endif

        // 인벤토리를 갱신한다.
        UpdateAllSlots();
    }

    void TestItem()
    {
        // 기능 테스트도 함께 할 겸
        AddItems("관찰력#빵#민첩성");

        UpdateAllSlots();
    }

    /// <summary>
    /// 아이템을 검색한 후, 인덱스를 반환한다. 실패 시 -1을 반환한다.
    /// </summary>
    /// <param name="tag">검색할 아이템 태그</param>
    /// <returns>검색된 아이템 인덱스</returns>
    public int FindItemIndexWithTag(string tag)
    {
        // 정렬하지 않으니 순차 탐색
        for(int i = 0; i < items.Count; ++i)
        {
            if (items[i].Tag == tag)
            {
                // 발견 시 인덱스 리턴
                return i;
            }
        }

        // 실패 시 -1 리턴
        return -1;
    }

    public Item FindItemWithName(string name)
    {
        // 정렬하지 않으니 순차 탐색
        for (int i = 0; i < items.Count; ++i)
        {
            if (items[i].Name == name)
            {
                // 발견 시 인덱스 리턴
                return items[i];
            }
        }

        // 실패 시 -1 리턴
        return errorItem;
    }

    public Item FindItemWithTag(string tag)
    {
        // 정렬하지 않으니 순차 탐색
        for (int i = 0; i < items.Count; ++i)
        {
            if (items[i].Tag == tag)
            {
                // 발견 시 인덱스 리턴
                return items[i];
            }
        }

        // 실패 시 -1 리턴
        return errorItem;
    }

    /// <summary>
    /// 이름으로 여러 아이템을 추가한다.
    /// </summary>
    /// <param name="items">#으로 연결된 아이템 목록 문자열 (ex. 빵#권총#근력)</param>
    public void AddItems(string items)
    {
        string[] splited_items = items.Split('#');

        for(int i = 0; i < splited_items.Length; ++i)
        {
            AddItem(splited_items[i]);
        }
    }

    /// <summary>
    /// 이름으로 아이템을 추가한다.
    /// </summary>
    /// <param name="name">추가할 아이템 이름</param>
    public void AddItem(string name)
    {
        // ItemInfo에서 검색 후 추가
        Item item = ItemInfo.Instance.GetItem(name);

        Debug.Log(item.Name);

        AddItem(item);
    }

    /// <summary>
    /// 아이템을 추가한다.
    /// </summary>
    /// <param name="item">추가할 아이템</param>
    public void AddItem(Item item)
    {
        Item findedItem = FindItemWithName(item.Name);

        // 아이템이 이미 있다면
        if (findedItem.Name != "Error")
        {
            // 아이템 수량을 증가
            findedItem.Count += 1;

        }
        // 아이템이 없다면
        else
        {
            // 인벤토리에 해당 아이템을 추가
            items.Add(item);
        }

        UpdateAllSlots();
    }

    /// <summary>
    /// 이름으로 여러 아이템을 삭제한다.
    /// </summary>
    /// <param name="items">#으로 연결된 아이템 목록 문자열 (ex. 빵#권총#근력)</param>
    public void RemoveItems(string items)
    {
        string[] splited_items = items.Split('#');

        for (int i = 0; i < splited_items.Length; ++i)
        {
            RemoveItem(splited_items[i]);
        }
    }

    // 아이템을 삭제한다.
    public void RemoveItem(string name)
    {
        // 아이템이 있는지 검색
        int findedIndex = FindItemIndexWithTag(name);

        // 아이템이 있다면
        if (findedIndex != -1)
        {
            // 수량을 감소
            items[findedIndex].Count -= 1;

            // 아이템이 1개만 있다면
            if (items[findedIndex].Count <= 0)
            {
                // 인벤토리에서 아이템 삭제
                items.RemoveAt(findedIndex);
            }

            // 인벤토리 갱신
            UpdateAllSlots();
        }
    }

    /// <summary>
    /// 모든 슬롯을 갱신한다.
    /// </summary>
    private void UpdateAllSlots()
    {
        // items 딕셔너리에 있는 아이템과 수량을 인벤토리 슬롯에 추가
        for(int i = 0; i < items.Count; ++i)
        {
            // 최신 순 정렬을 위해 뒤에서부터 앞쪽 슬롯에 넣는다.
            UpdateSlot(i, items[items.Count - i - 1]);
            slots[i].gameObject.SetActive(true);
        }

        // 비워진 슬롯들은
        for(int i = items.Count; i < lastSlotIndex - 1; ++i)
        {
            // 비활성화한다. (내용은 바꾸지 않는다.)
            slots[i].gameObject.SetActive(false);
        }

        lastSlotIndex = items.Count - 1;
    }

    /// <summary>
    /// index번째 슬롯을 Item으로 갱신한다.
    /// </summary>
    /// <param name="index">갱신할 슬롯 번호</param>
    /// <param name="item">갱신할 아이템 데이터</param>
    private void UpdateSlot(int index, Item item)
    {
        Debug.Log(item.Name + ": " + item.Count);

        // StringBuilder를 통해 최적화할 수 있지만, 이해하기 쉽게 if문을 사용
        // 아이템이 한 개만 있는 경우
        if (item.Count <= 1)
        {
            // 이름만 적는다.
            slots[index].text = item.Name;
        }
        // 1개 이상인 경우
        else
        {
            // 능력이라면
            if (item.Type == ItemType.Status)
            {
                // 레벨을 함께 표시한다.
                slots[index].text = $"{item.Name} Lv. {item.Count}";
            }
            // 물품이라면
            else
            {
                // 개수를 함께 표시한다.
                slots[index].text = $"{item.Name} x {item.Count}";
            }
        }
    }
    
    // Legacy
    public void GainJobItem()
    {
        // 선택한 직업의 아이템을 추가한다.
        string[] classItems = Supplier.Instance.classItemList[Player.Instance.job];
        for (int j = 0; j < classItems.Length; ++j)
        {
            //Items.Instance.AddItem(classItems[j]);
        }
    }
}
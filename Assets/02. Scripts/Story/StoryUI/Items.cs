using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

// 아이템 타입 (능력, 물품)
public enum ItemType
{
    Status,
    Item,
}

// 아이템 구조체
[Serializable]
public class Item
{
    // 타입
    public ItemType type;
    // 이름
    public string name;
    // 태그
    public string tag;
    // 개수
    public int count;

    public Item(ItemType type, string name, string tag, int count)
    {
        this.type = type;
        this.name = name;
        this.tag = tag;
        this.count = count;
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
        AddItems("관찰력#식칼#빵#민첩성#해열제#마체테");

        UpdateAllSlots();
    }

    #region 이름으로 검색
    public Item FindItemWithName(string name)
    {
        // 정렬하지 않으니 순차 탐색
        for (int i = 0; i < items.Count; ++i)
        {
            if (items[i].name == name)
            {
                // 발견 시 인덱스 리턴
                return items[i];
            }
        }

        // 실패 시 null 리턴
        return null;
    }

    /// <summary>
    /// 아이템을 검색한 후, 인덱스를 반환한다. 실패 시 -1을 반환한다.
    /// </summary>
    /// <param name="name">검색할 아이템 이름</param>
    /// <param name="startIndex">검색을 시작할 위치 인덱스, 없을 시 처음부터</param>
    /// <returns>검색된 아이템 인덱스</returns>
    public int FindItemIndexWithName(string name, int startIndex = 0)
    {
        // 정렬하지 않으니 순차 탐색
        for (int i = startIndex; i < items.Count; ++i)
        {
            if (items[i].name == name)
            {
                // 발견 시 인덱스 리턴
                return i;
            }
        }

        // 실패 시 -1 리턴
        return -1;
    }
    #endregion 이름으로 검색

    #region 태그로 검색
    public Item FindItemWithTag(string tag)
    {
        // 정렬하지 않으니 순차 탐색
        for (int i = 0; i < items.Count; ++i)
        {
            if (items[i].tag == tag)
            {
                // 발견 시 인덱스 리턴
                return items[i];
            }
        }

        // 실패 시 -1 리턴
        return null;
    }

    /// <summary>
    /// 아이템을 검색한 후, 인덱스를 반환한다. 실패 시 -1을 반환한다.
    /// </summary>
    /// <param name="name">검색할 아이템 태그</param>
    /// <param name="startIndex">검색을 시작할 위치 인덱스, 없을 시 처음부터</param>
    /// <returns>검색된 아이템 인덱스</returns>
    public int FindItemIndexWithTag(string tag, int startIndex = 0)
    {
        // 정렬하지 않으니 순차 탐색
        for (int i = startIndex; i < items.Count; ++i)
        {
            if (items[i].tag == tag)
            {
                // 발견 시 인덱스 리턴
                return i;
            }
        }

        // 실패 시 -1 리턴
        return -1;
    }

    // 아이템을 요청한다.
    public Dictionary<string, int> RequestItems(Dictionary<string, int> requireItems)
    {
        // 실제 갖고 있는 아이템
        Dictionary<string, int> havingItems = new Dictionary<string, int>();

        // 최종적으로 작성될 문구 정보 (삽입 순서 보장)
        Dictionary<string, int> resultItems = new Dictionary<string, int>();

        // 인벤토리에 있는지 한 태그씩 검사한다.
        foreach (KeyValuePair<string, int> pair in requireItems)
        {
            // 태그로 검색해보고
            havingItems = FindItemsWithTag(pair);

            // 찾았다면
            if(havingItems != null)
            {
                // 결과에 추가한다.
                foreach(KeyValuePair<string, int> searchedPair in havingItems)
                {
                    resultItems[searchedPair.Key] = searchedPair.Value;
                }

                // 종료
                continue;
            }

            // 실패했을 경우 이름으로도 검색해본다.
            havingItems = FindItemsWithName(pair);

            // 찾았다면
            if (havingItems != null)
            {
                // 결과에 추가한다.
                foreach (KeyValuePair<string, int> searchedPair in havingItems)
                {
                    resultItems[searchedPair.Key] = searchedPair.Value;
                }

                // 종료
                continue;
            }

            // 그럼에도 정말 못 찾았다면 못 찾았음을 결과에 추가한다.
            resultItems[pair.Key] = -1;
        }

        return resultItems;
    }

    // 태그, 개수를 받아 찾은 결과를 리턴한다.
    public Dictionary<string, int> FindItemsWithTag(KeyValuePair<string, int> requireItems)
    {
        // 실제 갖고 있는 아이템
        Dictionary<string, int> havingItems = new Dictionary<string, int>();
        Item item;
        // 필요 개수 저장
        string tag = requireItems.Key;
        int requireCount = requireItems.Value;

        // 최종적으로 작성될 문구 정보 (삽입 순서 보장)
        Dictionary<string, int> resultItems = new Dictionary<string, int>();

        // 인벤토리에 있는지 검사한다.
        // 검색 시작 위치. 0부터 시작
        int index = -1;

        // 전부 찾는다.
        while (requireCount > 0)
        {
            // 먼저 해당 Tag의 아이템이 있는지 검색한다.
            index = FindItemIndexWithTag(tag, index + 1);

            // 더이상 없다면 종료한다.
            if (index == -1)
            {
                break;
            }

            item = items[index];

            // 만약 아이템이 있고, 필요 개수를 충족한다면
            if (item.count >= requireCount)
            {
                // 필요한 만큼만 havingItem에 추가한다.
                havingItems[item.name] = requireCount;
            }
            // 반대로 아이템은 있는데, 필요 개수보다 모자란다면
            else
            {
                // 아이템 개수를 전부 넣고
                havingItems[item.name] = item.count;
            }

            // 필요 count를 감소시킨다.
            requireCount -= item.count;
        }

        // 개수가 모자라다면
        if (requireCount > 0)
        {
            // 해당 태그는 처리에 실패했음을 알린다.
            resultItems[tag] = -1;

            // 이름으로 재시도한다.
            return null;

        }
        // 충분하다면
        else
        {
            // 찾은 아이템들을 전부 넣는다.
            foreach (var result in havingItems)
            {
                resultItems[result.Key] = result.Value;
            }
        }

        return resultItems;
    }

    // 이름, 개수를 받아 찾은 결과를 리턴한다.
    public Dictionary<string, int> FindItemsWithName(KeyValuePair<string, int> requireItems)
    {
        // 실제 갖고 있는 아이템
        Dictionary<string, int> havingItems = new Dictionary<string, int>();
        Item item;
        // 필요 개수 저장
        string name = requireItems.Key;
        int requireCount = requireItems.Value;

        // 최종적으로 작성될 문구 정보 (삽입 순서 보장)
        Dictionary<string, int> resultItems = new Dictionary<string, int>();

        // 인벤토리에 있는지 검사한다.
        // 검색 시작 위치. 0부터 시작
        int index = -1;

        // 전부 찾는다.
        while (requireCount > 0)
        {
            // 먼저 해당 Tag의 아이템이 있는지 검색한다.
            index = FindItemIndexWithName(name, index + 1);

            // 더이상 없다면 종료한다.
            if (index == -1)
            {
                break;
            }

            item = items[index];

            // 만약 아이템이 있고, 필요 개수를 충족한다면
            if (item.count >= requireCount)
            {
                // 필요한 만큼만 havingItem에 추가한다.
                havingItems[item.name] = requireCount;
            }
            // 반대로 아이템은 있는데, 필요 개수보다 모자란다면
            else
            {
                // 아이템 개수를 전부 넣고
                havingItems[item.name] = item.count;
            }

            // 필요 count를 감소시킨다.
            requireCount -= item.count;
        }

        // 개수가 모자라다면
        if (requireCount > 0)
        {
            // 해당 태그는 처리에 실패했음을 알린다.
            resultItems[tag] = -1;

            // 이름으로 재시도한다.
            return null;

        }
        // 충분하다면
        else
        {
            // 찾은 아이템들을 전부 넣는다.
            foreach (var result in havingItems)
            {
                resultItems[result.Key] = result.Value;
            }
        }

        return resultItems;
    }
    #endregion 태그로 검색

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

        AddItem(item);
    }

    /// <summary>
    /// 아이템을 추가한다.
    /// </summary>
    /// <param name="item">추가할 아이템</param>
    public void AddItem(Item item)
    {
        Item findedItem = FindItemWithName(item.name);

        // 아이템이 이미 있다면
        if (findedItem != null)
        {
            // 아이템 수량을 증가
            findedItem.count += 1;

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
    public void RemoveItems(string items, int count = 1)
    {
        // 문자열을 자르고
        string[] splited_items = items.Split('#');

        // 딕셔너리로 변환한다.
        Dictionary<string, int> removeItems = ItemStringToDictionary(splited_items);

        // 아이템이 모자란 경우는 선택되지 않으니, 따로 검사하지 않는다.
        foreach (var item in removeItems)
        {
            RemoveItem(item.Key, item.Value);
        }
    }

    // 아이템을 삭제한다.
    public void RemoveItem(string name, int count = 1)
    {
        // 아이템이 있는지 검색
        int findedIndex = FindItemIndexWithName(name);

        // 아이템이 있다면
        if (findedIndex != -1)
        {
            // 수량을 감소
            items[findedIndex].count -= count;

            // 아이템이 없다면
            if (items[findedIndex].count <= 0)
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
        for(int i = items.Count; i <= lastSlotIndex; ++i)
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
        // StringBuilder를 통해 최적화할 수 있지만, 이해하기 쉽게 if문을 사용
        // 아이템이 한 개만 있는 경우
        if (item.count <= 1)
        {
            // 이름만 적는다.
            slots[index].text = item.name;
        }
        // 1개 이상인 경우
        else
        {
            // 능력이라면
            if (item.type == ItemType.Status)
            {
                // 레벨을 함께 표시한다.
                slots[index].text = $"{item.name} Lv. {item.count}";
            }
            // 물품이라면
            else
            {
                // 개수를 함께 표시한다.
                slots[index].text = $"{item.name} x {item.count}";
            }
        }
    }

    #region 문자열 해석 및 아이템 검색
    // 필요한 아이템의 종류와 개수가 담긴 문자열을 Dictionary로 정리한다.
    public Dictionary<string, int> ItemStringToDictionary(string[] itemText)
    {
        Dictionary<string, int> requireItem = new Dictionary<string, int>();

        // Dictionary에 추가
        for (int i = 0; i < itemText.Length; ++i)
        {
            if (itemText[i] == "")
            {
                continue;
            }

            if (requireItem.ContainsKey(itemText[i]))
            {
                // 해당 아이템 개수 추가
                requireItem[itemText[i]] += 1;
            }
            else
            {
                // 없다면 새로 추가
                requireItem[itemText[i]] = 1;
            }
        }

        return requireItem;
    }
    #endregion 문자열 해석 및 아이템 검색

    public void SaveItems()
    {
        // 현재 아이템 데이터에 저장
        DataManager.Instance.data.Items = items.ToList();
    }

    public void LoadItems()
    {
        // 데이터에 저장된 아이템 리스트를 불러옴
        items = DataManager.Instance.data.Items.ToList();

        // 인벤토리에 적용 (UI 업데이트)
        UpdateAllSlots();
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
using System.Collections.Generic;
using UnityEngine;

public class ItemInfo : MonoBehaviour
{
    #region 싱글톤
    public static ItemInfo Instance { get; set; }
    #endregion 싱글톤

    public Dictionary<string, Item> itemDictionary = new Dictionary<string, Item>();
    private List<Item> items = new List<Item>
    {
        // 에러 아이템
        new Item(ItemType.Item, "Error", "Error", 0),
        // 물품
        new Item(ItemType.Item, "빵", "음식", 1),
        new Item(ItemType.Item, "송이버섯", "음식", 1),
        new Item(ItemType.Item, "고기", "음식", 1),
        new Item(ItemType.Item, "고기?", "음식", 1),
        new Item(ItemType.Item, "건빵", "음식", 1),
        new Item(ItemType.Item, "사탕", "간식", 1),
        new Item(ItemType.Item, "초콜릿", "간식", 1),
        new Item(ItemType.Item, "개껌", "개껌", 1),
        new Item(ItemType.Item, "코트", "외투", 1),
        new Item(ItemType.Item, "가운", "외투", 1),
        new Item(ItemType.Item, "군복", "군복", 1),
        new Item(ItemType.Item, "우산", "우산", 1),
        new Item(ItemType.Item, "해열제", "약", 1),
        new Item(ItemType.Item, "붕대", "약", 1),
        new Item(ItemType.Item, "돈", "재화", 1),
        new Item(ItemType.Item, "다이아몬드", "재화", 1),
        new Item(ItemType.Item, "식칼", "칼", 1),
        new Item(ItemType.Item, "마체테", "칼", 1),
        new Item(ItemType.Item, "권총", "총", 1),
        new Item(ItemType.Item, "빠루", "둔기", 1),
        new Item(ItemType.Item, "펜던트", "펜던트", 1),
        new Item(ItemType.Item, "크레용", "크레용", 1),
        new Item(ItemType.Item, "개", "반려동물", 1),
        // 능력
        new Item(ItemType.Status, "근력", "근력", 1),
        new Item(ItemType.Status, "민첩성", "민첩성", 1),
        new Item(ItemType.Status, "관찰력", "관찰력", 1),
        new Item(ItemType.Status, "추리력", "추리력", 1),
        new Item(ItemType.Status, "철학", "철학", 1),
        new Item(ItemType.Status, "감성", "감성", 1),
        new Item(ItemType.Status, "통쾌함", "통쾌함", 1),
        new Item(ItemType.Status, "요리 실력", "요리 실력", 1),
        new Item(ItemType.Status, "근성", "근성", 1),
        new Item(ItemType.Status, "후각", "후각", 1),
        new Item(ItemType.Status, "아리마와의 친분", "아리마와의 친분", 1),
        new Item(ItemType.Status, "보육원과의 친분", "보육원과의 친분", 1),
        new Item(ItemType.Status, "감기", "감기", 1),
        new Item(ItemType.Status, "출혈", "출혈", 1),
    };

    private void Awake()
    {
        #region 싱글톤
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        #endregion 싱글톤

        // 아이템 딕셔너리 생성
        for (int i = 0; i < items.Count; ++i)
        {
            itemDictionary[items[i].name] = items[i];
        }
    }

    public Item GetItem(string name)
    {
        // 리스트에 있다면 아이템 반환
        if (itemDictionary.ContainsKey(name))
        {
            return itemDictionary[name];
        }
        // 없다면 에러
        else
        {
            Debug.LogError("해당 name을 가진 아이템 정보가 없습니다.");
            // 원래 return 안 하고 막는 게 맞다. 임시로 에러만 안 뜨게 하려고 해뒀음.
            return itemDictionary["Error"];
        }
    }
}

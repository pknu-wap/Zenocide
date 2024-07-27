using System.Collections.Generic;
using UnityEngine;

public class Supplier : MonoBehaviour
{
    // 싱글톤
    public static Supplier Instance;
    private void Awake()
    {
        Instance = this;
        for (int i = 0; i < classCardArray.Length; ++i)
        {
            classCardDeck[className[i]] = classCardArray[i];
        }
    }

    // 직업 순서
    public string[] className = new string[4];

    [Header("딕셔너리")]
    // 직업 별 카드를 모아둔 덱
    public Dictionary<string, CardList> classCardDeck = new Dictionary<string, CardList>();
    public CardList[] classCardArray = new CardList[4];

    // 직업 별 아이템을 모아둔 리스트
    public Dictionary<string, string[]> classItemList = new Dictionary<string, string[]>
    {
        { "군인", new string[] { "군복", "총", "건빵" } },
        { "의사", new string[] { "의약품", "볼펜", "가운" } },
        { "경찰", new string[] { "총", "수갑", "삼단봉" } },
        { "건설", new string[] { "작업복", "삽", "안전모" } },
    };
}
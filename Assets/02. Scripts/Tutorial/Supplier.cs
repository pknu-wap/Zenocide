using System.Collections.Generic;
using UnityEngine;

public class Supplier : MonoBehaviour
{
    [Header("딕셔너리")]
    // 직업 별 카드를 모아둔 덱
    public Dictionary<string, string[]> classCardDeck = new Dictionary<string, string[]>
    {
        { "군인", new string[] { "체인 소우", "독침" } },
    };
    // 직업 별 아이템을 모아둔 리스트
    public Dictionary<string, string[]> classItemList = new Dictionary<string, string[]>
    {
        { "군인", new string[] { "군복", "총", "건빵" } },
    };
}
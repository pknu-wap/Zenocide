using UnityEngine;

[CreateAssetMenu(fileName = "Card List", menuName = "Scriptable Object/Card List")]
public class CardList : ScriptableObject
{
    // 카드 리스트 이름
    public string listName;
    // 카드 리스트
    public CardData[] items;
}

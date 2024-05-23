// 김동건
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card List", menuName = "Scriptable Object/Card List")]
public class CardList : ScriptableObject
{
    public CardData[] items;
}

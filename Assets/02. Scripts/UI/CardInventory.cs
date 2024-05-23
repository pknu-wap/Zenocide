using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardInventory : MonoBehaviour
{
    public static CardInventory instance { get; private set; }
    public Transform slotsParent;
    public List<CardSlot> slots = new List<CardSlot>();

    private void Awake() => instance = this;

    private void Start()
    {
        slots = slotsParent.GetComponentsInChildren<CardSlot>().ToList<CardSlot>();
    }

    public void UpdateAllCardSlot()
    {
        List<CardData> deck = CardManager.Inst.deck;
        for (int i = 0; i < deck.Count; ++i)
        {
            slots[i].Setup(deck[i]);
        }
    }
}
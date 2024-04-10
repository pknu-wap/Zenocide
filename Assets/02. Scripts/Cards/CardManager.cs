// 김동건
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class CardManager : MonoBehaviour
{
    public static CardManager Inst { get; private set; }
    void Awake() => Inst = this;

    [SerializeField] ItemSO itemSO;
    [SerializeField] GameObject cardPrefab;
    [SerializeField] List<CardData> hand; 

    List<CardData> deck;

    public CardData DrawCard()
    {
        if (deck.Count == 0)
            SetUpDeck();

        CardData card = deck[0];
        deck.RemoveAt(0);
        return card;
    }

    void SetUpDeck()
    {
        deck = new List<CardData>(30);

        // itemSO의 카드들을 deck에 추가
        for (int i = 0; i < itemSO.items.Length; i++) 
        {
            CardData card = itemSO.items[i];
            deck.Add(card);
        }

        // deck 셔플
        for (int i = 0; i < deck.Count; i++)
        {
            int rand = Random.Range(i, deck.Count);
            CardData temp = deck[i];
            deck[i] = deck[rand];
            deck[rand] = temp;
        }
    }

    void Start()
    {
        SetUpDeck();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
            Debug.Log(DrawCard());
    }

    void AddCardToHand()
    {
        var cardObject = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
        var card = cardObject.GetComponent<CardInfoUpdater>();
        card.UpdateCardInfo(DrawCard());
        hand.Add(card.cardData);

        //SetOriginOrder();
        //CardAlignment(isMine);
    }
}

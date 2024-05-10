// 김동건
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class CardManager : MonoBehaviour
{
    public static CardManager Inst { get; private set; }
    void Awake() => Inst = this;

    // 아이템 풀
    [SerializeField] ItemSO itemSO;

    // 카드 프리팹
    [SerializeField] GameObject cardPrefab;

    // 핸드
    public List<Card> hand;
    [SerializeField] Transform handLeft;
    [SerializeField] Transform handRight;

    // 카드 관련 트랜스폼
    [SerializeField] public Transform cardSpawnPoint;
    [SerializeField] public Transform cardDrawPoint;
    [SerializeField] public Transform cardDumpPoint;

    public List<CardData> deck;
    public List<CardData> dump;
    [SerializeField] TMP_Text deckCountTMP;
    [SerializeField] TMP_Text dumpCountTMP;

    Card selectCard;

    float delay03 = 0.3f;
    float delay05 = 0.5f;

    public CardData DrawCard()
    {
        if (deck.Count == 0)
        {
            ResetDeck();
        }

        // queue나 dequeue를 쓰는 게 더 나을 듯
        CardData card = deck[0];
        deck.RemoveAt(0);
        UpdateDeckCount();
        return card;
    }

    void SetUpDeck()
    {
        deck = new List<CardData>(100);

        // itemSO의 카드들을 deck에 추가
        for (int i = 0; i < itemSO.items.Length; i++) 
        {
            CardData card = itemSO.items[i];
            deck.Add(card);
        }
        UpdateDeckCount();

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
        dump = new List<CardData>(100);
        // 왜 싱글톤에서 호출하지 않고 Action으로 호출할까,,,,
        TurnManager.OnAddCard += AddCardToHand;
    }

    void OnDestroy()
    {
        TurnManager.OnAddCard -= AddCardToHand;
    }

    [SerializeField] int maxHand = 10;

    void AddCardToHand(bool isMine)
    {
        if (!isMine || hand.Count > maxHand)
        {
            return;
        }

        var cardObject = Instantiate(cardPrefab, cardSpawnPoint.position, Utils.QI);
        var card = cardObject.GetComponent<Card>();
        card.Setup(DrawCard());
        card.transform.localScale = Vector3.zero;
        hand.Add(card);

        SetOriginOrder();
        StartCoroutine(DrawAnimationCo(card));
    }

    IEnumerator DrawAnimationCo(Card card)
    {
        Sequence sequence = DOTween.Sequence()
                .Append(card.transform.DOMove(cardDrawPoint.position, delay05))
                .Join(card.transform.DORotateQuaternion(cardDrawPoint.rotation, delay05))
                .Join(card.transform.DOScale(Vector3.one * 12f, delay05))
                .SetEase(Ease.OutCubic);

        yield return new WaitForSeconds(delay05);

        CardAlignment();
    }

    void SetOriginOrder()
    {
        int count = hand.Count;
        for (int i = 0; i < count; i++)
        {
            var targetCard = hand[i];
            targetCard?.GetComponent<CardOrder>().SetOriginOrder(i);
        }
    }

    void CardAlignment()
    {
        List<PRS> originCardPRSs = new List<PRS>();
        float radius = handRight.position.x - handLeft.position.x;
        originCardPRSs = RoundAlignment(handLeft, handRight, hand.Count, radius, Vector3.one * 10f);

        for (int i = 0; i < hand.Count; i++)
        {
            var targetCard = hand[i];

            targetCard.originPRS = originCardPRSs[hand.Count - i - 1];
            targetCard.MoveTransform(targetCard.originPRS, true, delay05);
        }
    }

    // 이해하기를 포기함...
    List<PRS> RoundAlignment(Transform leftTr, Transform rightTr, int objCount, float radius, Vector3 scale)
    {
        float[] objLerps = new float[objCount];
        List<PRS> results = new List<PRS>(objCount);

        switch (objCount)
        {
            case 1: objLerps = new float[] { 0.5f }; break;
            case 2: objLerps = new float[] { 0.27f, 0.73f }; break;
            case 3: objLerps = new float[] { 0.1f, 0.5f, 0.9f }; break;
            default:
                float interval = 1f / (objCount - 1);
                for (int i = 0; i < objCount; i++)
                    objLerps[i] = interval * i;
                break;
        }

        Vector3 circleCenter = Vector3.Lerp(leftTr.position, rightTr.position, 0.5f) + Vector3.down * radius;

        for (int i = 0; i < objCount; i++)
        {
            // 타겟 위치는 leftTr과 rightTr 사이, i번째 카드의 위치
            Vector3 targetPos = Vector3.Lerp(leftTr.position, rightTr.position, objLerps[i]);
            // targetRot은 우선 기본값으로.
            Quaternion targetRot = Utils.QI;


            // 카드가 4개 이상일 때만 회전을 적용한다.
            if (objCount >= 4)
            {
                // 원의 방정식, (x-a)^2 + (y-b)^2 = r^2의 변형.
                // x = targetPos.x, a = circleCenter.x, y = curve, b = circleCenter.y, r = height                
                float curve = Mathf.Sqrt(Mathf.Pow(radius, 2) - Mathf.Pow(targetPos.x - circleCenter.x, 2));
                // 절댓값으로 변환
                curve = Mathf.Abs(curve);
                targetPos.y = targetPos.y - radius + curve;
                targetRot = Quaternion.Slerp(leftTr.rotation, rightTr.rotation, objLerps[i]);
            }
            results.Add(new PRS(targetPos, targetRot, scale));
        }
        return results;
    }

    public void DiscardCard(Card card)
    {
        hand.Remove(card);
        dump.Add(card.cardData);
        UpdateDumpCount();

        card.transform.DOKill();

        DestroyImmediate(card.gameObject);
        selectCard = null;

        CardAlignment();
    }

    public void ResetDeck()
    {
        deck.Clear();

        // dump의 카드들을 deck에 추가
        for (int i = 0; i < dump.Count; i++)
        {
            CardData card = dump[i];
            deck.Add(card);
        }
        UpdateDeckCount();

        // deck 셔플
        for (int i = 0; i < deck.Count; i++)
        {
            int rand = Random.Range(i, deck.Count);
            CardData temp = deck[i];
            deck[i] = deck[rand];
            deck[rand] = temp;
        }

        // dump 비우기
        dump.Clear();
        UpdateDumpCount();
    }

    public IEnumerator DiscardHandCo()
    {
        int handCount = hand.Count;
        for (int i = 0; i < handCount; i++)
        {
            Card card = hand[0];

            // 묘지로 카드 이동
            Sequence sequence = DOTween.Sequence()
                .Append(card.transform.DOMove(cardDumpPoint.position, delay03))
                .Join(card.transform.DORotateQuaternion(Utils.QI, delay03))
                .Join(card.transform.DOScale(Vector3.one, delay03))
                .SetEase(Ease.OutQuad);
            
            hand.RemoveAt(0);
            CardAlignment();

            // sequence 끝나기 전까지 기다리기
            yield return new WaitForSeconds(delay03);

            // sequence가 끝나면 오브젝트 파괴
            dump.Add(card.cardData);
            UpdateDumpCount();
            DestroyImmediate(card.gameObject);
        }

        hand.Clear();
        selectCard = null;
    }

    public void UpdateDeckCount()
    {
        deckCountTMP.text = deck.Count.ToString();
    }

    public void UpdateDumpCount()
    {
        dumpCountTMP.text = dump.Count.ToString();
    }

    #region MyCard

    public void CardMouseEnter(Card card)
    {
        selectCard = card;
        EnlargeCard(true, card);
    }

    public void CardMouseExit(Card card)
    {
        EnlargeCard(false, card);
    }

    public void CardMouseDown()
    {
        if(TurnManager.Inst.myTurn)
            DiscardCard(selectCard);
    }

    void EnlargeCard(bool isEnlarge, Card card)
    {
        if (isEnlarge)
        {
            Vector3 enlargePos = new Vector3(card.originPRS.pos.x, handLeft.position.y + delay05, -3f);
            card.MoveTransform(new PRS(enlargePos, Utils.QI, card.originPRS.scale * 1.2f), false);
        }
        else
        {
            card.MoveTransform(card.originPRS, false);
        }
        
        card.GetComponent<CardOrder>().SetMostFrontOrder(isEnlarge);
    }

    #endregion
}

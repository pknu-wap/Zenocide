// 김동건
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }
    void Awake() => Instance = this;

    // 카드 풀
    [SerializeField] CardList cardList;
    [SerializeField] CardList defaultDeck;
    Dictionary<string, CardData> cardDict;

    // 카드 프리팹
    [SerializeField] GameObject cardPrefab;
    [SerializeField] GameObject cardBackPrefab;

    // 핸드
    public List<Card> hand;
    [SerializeField] int maxHand = 10;
    [SerializeField] Transform cardObjcetParent;
    [SerializeField] Transform handLeft;
    [SerializeField] Transform handRight;

    // 카드 관련 트랜스폼
    [SerializeField] public Transform cardSpawnPoint;
    [SerializeField] public Transform cardDrawPoint;
    [SerializeField] public Transform cardResetPoint;
    [SerializeField] public Transform cardDumpPoint;

    // 덱, 묘지
    public List<CardData> deck;
    public List<CardData> dump;
    List<GameObject> cardBackObjectList;
    [SerializeField] TMP_Text deckCountTMP;
    [SerializeField] TMP_Text dumpCountTMP;
    [SerializeField] Transform cardBackObjectParent;

    Card selectCard;

    // 상수
    int listSize = 100;
    float delay01 = 0.1f;
    float delay03 = 0.3f;
    float delay05 = 0.5f;
    float focusOffset = 100f;

    public Vector3 focusPos;

    void Start()
    {
        // 동적 참조를 줄이기 위해 싱글톤 대신 Action으로 호출
        TurnManager.OnAddCard += AddCardToHand;

        focusPos = new Vector3(0f, handLeft.position.y + focusOffset, -3f);

        // 지금은 게임과 전투가 동시에 시작
        // InitDeck은 게임 시작 시, SetUpDeck과 InitDump는 전투 시작 시 호출해야 함
        #region CreateDict
        cardDict = new Dictionary<string, CardData>();
        foreach (CardData card in cardList.items)
        {
            cardDict.Add(card.name, card);
        }
        #endregion

        #region InitDeck
        deck = new List<CardData>(listSize);
        // 기본 카드들을 deck에 추가
        foreach (CardData card in defaultDeck.items)
        {
            AddCardToDeck(card.name);
        }

        UpdateDeckCount();
        #endregion
        MergeDumpToDeck();
        SetUpDeck();

        #region ResetDeckInitiation
        cardBackObjectList = new List<GameObject>(listSize);

        // 카드 뒷면 오브젝트 생성해서 리스트에 추가하고 enable 처리
        for (int i = 0; i < listSize; i++)
        {
            cardBackObjectList.Add(Instantiate(cardBackPrefab, cardDumpPoint.position, Utils.QI, cardBackObjectParent));
            cardBackObjectList[i].SetActive(false);
        }
        #endregion
    }

    private void Update()
    {
        string[] dummyCards =
        {
            "더미1",
            "더미2",
            "더미3"
        };

        if (Input.GetKeyDown(KeyCode.Q) && TurnManager.Instance.myTurn)
        {
            AddCardToDeck(dummyCards[Random.Range(0, 3)]);
            SetUpDeck();
        }
    }

    // 덱 카운트가 0인지 확인하고 사용해야 함
    CardData DrawCard()
    {
        CardData card = deck[0];
        deck.RemoveAt(0);
        UpdateDeckCount();
        return card;
    }

    public void AddCardToDeck(string cardName)
    {
        deck.Add(cardDict[cardName]);
        UpdateDeckCount();
    }

    public void RemoveCardFromDeck(string cardName)
    {
        CardData target = null;
        foreach (CardData card in deck)
        {
            // 일치하는 이름 중 첫번째 카드를 가져온다.
            // 중복 카드가 있어도 하나만 선택
            if (card.name == cardName)
            {
                target = card;
                break;
            }
        }

        // target이 null이면 Remove가 false를 반환하고 아무 일도 일어나지 않는다.
        deck.Remove(target);
    }

    void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int rand = Random.Range(i, deck.Count);
            CardData temp = deck[i];
            deck[i] = deck[rand];
            deck[rand] = temp;
        }
    }

    void OnDestroy()
    {
        TurnManager.OnAddCard -= AddCardToHand;
    }

    void AddCardToHand(bool isMine)
    {
        if (!isMine || hand.Count > maxHand)
        {
            return;
        }

        var cardObject = Instantiate(cardPrefab, cardSpawnPoint.position, Utils.QI, cardObjcetParent);
        var card = cardObject.GetComponent<Card>();

        // DrawCard() 호출 전에 덱이 비었는지 확인
        if (deck.Count == 0)
        {
            StartCoroutine(ResetDeckAnimationCo(dump.Count));
            ResetDeck();
            MergeDumpToDeck();
        }

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
                .Join(card.transform.DOScale(Vector3.one * 15f, delay05))
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

    // dump와 hand를 덱으로 모아서 셔플
    void SetUpDeck()
    {
        // hand의 카드들을 deck에 추가하고 오브젝트 파괴
        for (int i = 0; i < hand.Count; i++)
        {
            Card card = hand[i];
            deck.Add(card.cardData);
            DestroyImmediate(card.gameObject);
        }

        MergeDumpToDeck();
        hand.Clear();
        UpdateDeckCount();
        selectCard = null;

        ShuffleDeck();
    }

    // dump를 deck에 모아 셔플
    void ResetDeck()
    {
        MergeDumpToDeck();

        ShuffleDeck();
    }

    void MergeDumpToDeck()
    {
        // dump의 카드들을 deck에 추가
        for (int i = 0; i < dump.Count; i++)
        {
            CardData card = dump[i];
            deck.Add(card);
        }

        dump = new List<CardData>(listSize);
        UpdateDumpCount();
    }

    IEnumerator ResetDeckAnimationCo(int dumpCount)
    {
        // 카드 뒷면 오브젝트 활성화
        for (int i = 0; i < dumpCount; i++)
        {
            cardBackObjectList[i].SetActive(true);
        }

        for (int i = 0; i < dumpCount; i++)
        {
            // 포물선 이동
            Sequence sequence = DOTween.Sequence()
                .Append(cardBackObjectList[i].transform.DOMoveX(cardResetPoint.position.x, delay03))
                .Join(cardBackObjectList[i].transform.DOMoveY(cardResetPoint.position.y, delay03)).SetEase(Ease.OutCubic)
                .Append(cardBackObjectList[i].transform.DOMoveX(cardSpawnPoint.position.x, delay03))
                .Join(cardBackObjectList[i].transform.DOMoveY(cardSpawnPoint.position.y, delay03));

            // 각 카드에 딜레이 주기
            yield return new WaitForSeconds(delay01);
        }

        // 전체 애니메이션 종료까지 대기
        yield return new WaitForSeconds(delay03 * 2 + delay01 * dumpCount);

        for (int i = 0; i < dumpCount; i++)
        {
            // 카드 뒷면 오브젝트 다시 숨기기
            cardBackObjectList[i].SetActive(false);
            // 덱으로 옮겨놓은 오브젝트 다시 묘지로 원위치
            cardBackObjectList[i].transform.position = cardDumpPoint.position;
        }
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

    void UpdateDeckCount()
    {
        deckCountTMP.text = deck.Count.ToString();
    }

    void UpdateDumpCount()
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
        if (TurnManager.Instance.myTurn)
            DiscardCard(selectCard);
    }

    void EnlargeCard(bool isEnlarge, Card card)
    {
        if (isEnlarge)
        {
            Vector3 enlargePos = new Vector3(card.originPRS.pos.x, handLeft.position.y + focusOffset, -3f);
            card.MoveTransform(new PRS(enlargePos, Utils.QI, card.originPRS.scale * 1.5f), false);
        }
        else
        {
            card.MoveTransform(card.originPRS, false);
        }

        card.GetComponent<CardOrder>().SetMostFrontOrder(isEnlarge);
    }

    #endregion
}

// 김동건
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Linq;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }

    [Header("카드 풀")]
    [SerializeField] CardList cardList;
    [SerializeField] CardList defaultDeck;
    Dictionary<string, CardData> cardDict;

    [Header("드로우 버퍼")]
    // 덱에서 패로 이동하기 전, 드로우 될 카드들이 모여 있는 곳
    public List<Card> drawBuffer;

    [Header("핸드")]
    public List<Card> hand;
    private int maxHand = 10;
    private Transform handGroup;
    private Transform handLeft;
    private Transform handRight;

    [Header("카드 위치")]
    private Transform cardSpawnPoint;
    private Transform cardDrawPoint;
    private Transform cardResetPoint;
    public Transform cardDumpPoint;

    [Header("덱, 묘지")]
    public List<CardData> deck;
    public List<CardData> dump;
    private TMP_Text deckCountTMP;
    private TMP_Text dumpCountTMP;
    private Transform cardBackGroup;

    [Header("이펙트")]
    private Transform effectGroup;

    // 선택된 카드
    [SerializeField] Card selectCard;

    [Header("코스트 조정")]
    public int costModificationAmount = 0;

    [Header("상수")]
    int listSize = 100;
    float focusOffset = 100f;
    public Vector3 focusPos;

    [Header("딜레이")]
    private float drawDelay = 0.5f;
    private float discardDelay = 0.2f;
    public float resetDelay = 0.05f;
    public float resetMoveDelay = 0.1f;
    private float moveDelay = 0.5f;

    private void Awake()
    {
        Instance = this;
        
        // 컴포넌트를 할당한다.
        EnrollComponent();
    }
    void Start()
    {
        focusPos = new Vector3(0f, handLeft.position.y + focusOffset, -3f);
        UpdateDumpCount();

        #region CreateDict
        cardDict = new Dictionary<string, CardData>();
        for(int i = 0; i < cardList.items.Length; i++)
        {
            cardDict.Add(cardList.items[i].name, cardList.items[i]);
        }
        #endregion

        #region InitDeck
        // 새 게임이면
        if (!DataManager.Instance.isLoaded)
        {
            deck = new List<CardData>(listSize);
            // 기본 카드들을 deck에 추가
            for (int i = 0; i < defaultDeck.items.Length; i++)
            {
                AddCardToDeck(defaultDeck.items[i].name);
            }
        }
        #endregion
    }

    private void Update()
    {
        if(selectCard != null)
        {
            // 마우스 우클릭 시
            if (Input.GetMouseButtonDown(1))
            {
                // 선택한 카드를 취소한다.
                selectCard.CancelWithRightClick();
            }
        }
    }

    private void EnrollComponent()
    {
        handGroup = GameObject.Find("Hand Group").transform;
        handLeft = GameObject.Find("Hand Left").transform;
        handRight = GameObject.Find("Hand Right").transform;

        cardSpawnPoint = GameObject.Find("Card Spawn Point").transform;
        cardDrawPoint = GameObject.Find("Card Draw Point").transform;
        cardResetPoint = GameObject.Find("Card Reset Point").transform;
        cardDumpPoint = GameObject.Find("Card Dump Point").transform;
        
        deckCountTMP = GameObject.Find("Deck Count TMP").GetComponent<TMP_Text>();
        dumpCountTMP = GameObject.Find("Dump Count TMP").GetComponent<TMP_Text>();
        cardBackGroup = GameObject.Find("Card Back Group").transform;
        effectGroup = GameObject.Find("Effect Group").transform;
    }
    
    // 여러 카드를 덱에 추가한다. (string)
    public void AddCardsToDeck(string cardName)
    {
        string[] cards = cardName.Split('#');

        for (int i = 0; i < cards.Length; ++i)
        {
            if (cardDict.ContainsKey(cardName) == false)
            {
                Debug.LogError(cards[i] + " 카드가 없습니다. 이름을 확인해주세요.");
                continue;
            }

            AddCardToDeck(cards[i]);
        }
    }

    // 카드를 덱에 추가한다. (string)
    public void AddCardToDeck(string cardName)
    {
        if (cardDict.ContainsKey(cardName) == false)
        {
            Debug.LogError(cardName + " 카드가 없습니다. 이름을 확인해주세요.");
            return;
        }

        deck.Add(cardDict[cardName]);
        SortDeck();
        UpdateDeckCount();
    }

    // 카드를 덱에 추가한다. (CardData)
    public void AddCardToDeck(CardData card)
    {
        deck.Add(card);
        SortDeck();
        UpdateDeckCount();
    }

    // 여러 카드를 덱에서 삭제한다.
    public void RemoveCardsFromDeck(string cardName)
    {
        string[] cards = cardName.Split('#');
        
        for(int i = 0; i < cards.Length; ++i)
        {
            RemoveCardFromDeck(cards[i]);
        }
    }

    // 카드를 덱에서 삭제한다.
    public void RemoveCardFromDeck(string cardName)
    {
        CardData target = null;
        for(int i = 0;  i < deck.Count; i++)
        {
            // 일치하는 이름 중 첫번째 카드를 가져온다.
            // 중복 카드가 있어도 하나만 선택
            if (deck[i].name == cardName)
            {
                target = deck[i];
                break;
            }
        }

        // target이 null이면 Remove가 false를 반환하고 아무 일도 일어나지 않는다.
        deck.Remove(target);
    }

    public void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int rand = Random.Range(i, deck.Count);
            CardData temp = deck[i];
            deck[i] = deck[rand];
            deck[rand] = temp;
        }
    }

    public IEnumerator AddCardToHand(int count)
    {
        int drawCount = count;
        // Draw Count 조정
        if (hand.Count + drawCount > maxHand)            
        {
            drawCount = maxHand - hand.Count;
        }

        // 덱과 묘지 전부 카드가 없을 경우
        if (deck.Count + dump.Count == 0)
        {
            // 뽑지 않는다.
            drawCount = 0;
        }

        int resetCount = deck.Count;
        int dumpCount = 0;

        // drawBuffer에 개수만큼 등록한다. (데이터적으로, 한 번에 drawBuffer에 추가된다.)
        for (int i = 0; i < drawCount; ++i)
        {
            GameObject cardObject = ObjectPoolManager.Instance.GetGo("Card");
            cardObject.transform.position = cardSpawnPoint.position;
            Card card = cardObject.GetComponent<Card>();

            // DrawCard() 호출 전에 덱이 비었는지 확인
            if (deck.Count == 0)
            {
                dumpCount = dump.Count;
                MergeDumpToDeck();
            }

            card.effectGroup = effectGroup;
            card.transform.localScale = Vector3.zero;
            card.Setup(DrawCard());

            // 코스트 조정 중이라면 적용해서 드로우
            if(costModificationAmount != 0)
            {
                ModifyCost(card);
            }

            drawBuffer.Add(card);
        }

        // 애니메이션을 순차적으로 실행한다.
        for(int i = 0; i < drawCount; ++i)
        {
            // 덱 리셋 애니메이션 출력
            if(i == resetCount)
            {
                yield return StartCoroutine(ResetDeckAnimationCo(dumpCount));
            }

            // 드로우 버퍼의 첫 장을 골라
            Card card = drawBuffer[0];

            // 드로우 애니메이션을 실행하고, 끝나면 버퍼에서 삭제, hand에 추가한다.
            StartCoroutine(DrawAnimationCo(card));
            drawBuffer.RemoveAt(0);
            hand.Add(card);

            // 덱 텍스트를 변경해준다.
            UpdateDeckCount(-1);

            // Hand 카드 순서 정렬
            SetOriginOrder();

            // drawDelay만큼 딜레이를 준다.
            yield return new WaitForSeconds(drawDelay);
        }
    }

    // 덱 카운트가 0인지 확인하고 사용해야 함
    // AddCardToHand 호출 시 함께 사용된다.
    CardData DrawCard()
    {
        CardData card = deck[0];
        deck.RemoveAt(0);
        return card;
    }

    // 드로우 애니메이션을 실행한다.
    IEnumerator DrawAnimationCo(Card card)
    {
        Sequence sequence = DOTween.Sequence()
                .Append(card.transform.DOMove(cardDrawPoint.position, drawDelay))
                .Join(card.transform.DOScale(Vector3.one * 15f, drawDelay))
                .SetEase(Ease.OutCubic);

        // DOTween이 끝날 때까지 기다린다.
        yield return sequence.WaitForCompletion();

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

    // 0번 카드부터 last번 째 카드까지 정렬시킨다.
    public void CardAlignment()
    {
        List<PRS> originCardPRSs = new List<PRS>();
        float radius = handRight.position.x - handLeft.position.x;
        originCardPRSs = RoundAlignment(handLeft, handRight, hand.Count, radius, Vector3.one * 10f);

        for (int i = 0; i < hand.Count; i++)
        {
            var targetCard = hand[i];

            targetCard.originPRS = originCardPRSs[hand.Count - i - 1];
            targetCard.MoveTransform(targetCard.originPRS, true, moveDelay);
        }
    }

    // 카드를 원형으로 정렬시킨다. (위치 배열 반환)
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
        dump.Add(card.cardData);
        UpdateDumpCount();

        card.transform.DOKill();

        // 카드 자원을 풀에 반환
        card.ReleaseObject();
    }

    public void ClearSelectCard()
    {
        selectCard = null;
    }

    // dump와 hand를 덱으로 모아서 셔플
    public void ResetDeck()
    {
        // hand의 카드들을 deck에 추가하고 오브젝트 파괴
        for (int i = 0; i < hand.Count; i++)
        {
            Card card = hand[i];
            deck.Add(card.cardData);
            DestroyImmediate(card.gameObject);
        }

        MergeDumpToDeck();
        UpdateDumpCount();
        hand.Clear();
        UpdateDeckCount();
        selectCard = null;

        ShuffleDeck();
    }

    // dump의 카드들을 deck에 추가
    // dumpCount와 deckCount는 따로 갱신해줘야 한다.
    public void MergeDumpToDeck()
    {
        for (int i = 0; i < dump.Count; i++)
        {
            CardData card = dump[i];
            deck.Add(card);
        }

        dump = new List<CardData>(listSize);
    }

    IEnumerator ResetDeckAnimationCo(int dumpCount)
    {
        CardBack[] cardBack = new CardBack[dumpCount];

        // 카드 뒷면 자원 가져오기
        for (int i = 0; i < dumpCount; i++)
        {
            cardBack[i] = ObjectPoolManager.Instance.GetGo("CardBack").GetComponent<CardBack>();
        }

        for (int i = 0; i < dumpCount; i++)
        {
            // dumpCount 갱신
            UpdateDumpCount(-1);

            // 포물선 이동
            // 각 카드에 딜레이 주기
            yield return StartCoroutine(cardBack[i].Move(cardResetPoint, cardSpawnPoint, cardDumpPoint));

            // deckCount 갱신
            UpdateDeckCount(1);
        }

        // 전체 애니메이션 종료까지 대기
        yield return new WaitForSeconds(resetMoveDelay * 2 + resetDelay * dumpCount);

        // 카드 뒷면 자원 반환
        for (int i = 0; i < dumpCount; i++)
        {
            cardBack[i].ResetPosition(cardDumpPoint);
            cardBack[i].ReleaseObject();
        }
    }

    public IEnumerator DiscardHandCo()
    {
        // drawBuffer가 남아 있다면 빌 때까지 기다린다.
        while (drawBuffer.Any())
        {
            yield return null;
        }

        // 패 버리기가 시작되면 모든 카드의 클릭을 막는다.
        for(int i = 0; i < hand.Count; ++i)
        {
            hand[i].isDiscarded = true;
        }

        // hand가 남아 있다면 반복
        while (hand.Any())
        {
            // 맨 끝의 카드를 가져오고 캐싱
            Card card = hand[0];

            // 코스트가 조정되어 있을 때 원복한다.
            if (costModificationAmount != 0)
            {
                card.SetCost(cardDict[card.cardData.name].cost, 0);
            }

            // 패에서 바로 삭제한다.
            hand.RemoveAt(0);
            // 카드가 사라지는 순간 정렬한다.
            CardAlignment();

            // 카드의 실행 중인 애니메이션을 종료하고
            card.moveSequence.Kill();

            // 묘지로 카드 이동
            Sequence move = DOTween.Sequence()
                .Append(card.transform.DOMove(cardDumpPoint.position, discardDelay))
                .Join(card.transform.DORotateQuaternion(Utils.QI, discardDelay))
                .Join(card.transform.DOScale(Vector3.one, discardDelay))
                .SetEase(Ease.OutQuad);

            // sequence 끝나기 전까지 기다리기
            yield return move.WaitForCompletion();

            // sequence가 끝나면 오브젝트 파괴
            dump.Add(card.cardData);
            UpdateDumpCount();
            // 카드 자원을 풀에 반환
            card.ReleaseObject();
            // 카드의 클릭 허용
            //card.EnableCollider();
            //hand[i].isDiscarded = false;
        }

        // 패를 전부 비우고
        hand.Clear();
        // 선택된 카드도 비운다.
        selectCard = null;
    }

    void UpdateDeckCount()
    {
        deckCountTMP.text = (deck.Count + drawBuffer.Count).ToString();
    }

    void UpdateDeckCount(int amount)
    {
        deckCountTMP.text = (int.Parse(deckCountTMP.text) + amount).ToString();
    }

    void UpdateDumpCount()
    {
        dumpCountTMP.text = dump.Count.ToString();
    }

    void UpdateDumpCount(int amount)
    {
        dumpCountTMP.text = (int.Parse(dumpCountTMP.text) + amount).ToString();
    }

    // 선택한 직업의 카드를 추가한다.
    public void GainJobCard()
    {
        CardList classCards = Supplier.Instance.classCardDeck[Player.Instance.job];
        for (int j = 0; j < classCards.items.Length; ++j)
        {
            CardManager.Instance.AddCardToDeck(classCards.items[j]);
        }
    }

    // 덱을 코스트, 이름 순으로 정렬한다.
    // 묘지, 핸드 병합은 따로 호출해야한다.
    public void SortDeck()
    {
        deck.Sort((CardData c1, CardData c2) =>
        {
            if (c1.cost != c2.cost)
            {
                return c1.cost.CompareTo(c2.cost);
            }
            else
            {
                return c1.name.CompareTo(c2?.name);
            }
        });
    }

    // deck을 Data 객체에 저장한다.
    public void SaveDeck()
    {
        DataManager.Instance.data.Deck = deck.ToList();
    }

    public void LoadDeck()
    {
        deck = DataManager.Instance.data.Deck.ToList();
        CardInventory.instance.UpdateAllCardSlot();
    }

    #region MyCard
    public bool IsCardSelected()
    {
        return selectCard != null;
    }

    public void SelectCard(Card card)
    {
        selectCard = card;
    }

    public void CardMouseEnter(Card card)
    {
        EnlargeCard(true, card);
    }

    public void CardMouseExit(Card card)
    {
        EnlargeCard(false, card);
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

    public void SetModifyCost(int amount)
    {
        // 조정량을 설정하고
        costModificationAmount = amount;

        // 핸드의 코스트를 조정한다.
        for(int i=0;i<hand.Count;i++)
        {
            ModifyCost(hand[i]);
        }
    }

    public void ModifyCost(Card card)
    {
        card.SetCost(Mathf.Max(0, card.cost + costModificationAmount), costModificationAmount);
    }

    public void ResetModifyCost()
    {
        // 조정량을 초기화 하고
        costModificationAmount = 0;

        // 핸드의 코스트를 원복시킨다
        for (int i = 0; i < hand.Count; i++)
        {
            hand[i].SetCost(cardDict[hand[i].cardData.name].cost, 0);
        }
    }

    public void SetExtraDamage()
    {
        for(int i = 0; i < hand.Count; i++)
        {
            hand[i].SetDamageDiscription();
        }
    }
}

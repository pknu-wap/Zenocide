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
    void Awake() => Instance = this;

    [Header("카드 풀")]
    [SerializeField] CardList cardList;
    [SerializeField] CardList defaultDeck;
    Dictionary<string, CardData> cardDict;

    [Header("카드 프리팹")]
    [SerializeField] GameObject cardPrefab;
    [SerializeField] GameObject cardBackPrefab;

    [Header("드로우 버퍼")]
    // 덱에서 패로 이동하기 전, 드로우 될 카드들이 모여 있는 곳
    public List<Card> drawBuffer;

    [Header("핸드")]
    public List<Card> hand;
    [SerializeField] int maxHand = 10;
    [SerializeField] Transform handGroup;
    [SerializeField] Transform handLeft;
    [SerializeField] Transform handRight;

    [Header("카드 위치")]
    [SerializeField] public Transform cardSpawnPoint;
    [SerializeField] public Transform cardDrawPoint;
    [SerializeField] public Transform cardResetPoint;
    [SerializeField] public Transform cardDumpPoint;

    [Header("덱, 묘지")]
    public List<CardData> deck;
    public List<CardData> dump;
    List<GameObject> cardBackObjectList;
    [SerializeField] TMP_Text deckCountTMP;
    [SerializeField] TMP_Text dumpCountTMP;
    [SerializeField] Transform cardBackGroup;

    [Header("이펙트")]
    [SerializeField] public Transform effectGroup;

    // 선택된 카드
    [SerializeField] Card selectCard;

    [Header("상수")]
    int listSize = 100;
    float focusOffset = 100f;
    public Vector3 focusPos;

    [Header("딜레이")]
    [SerializeField] float drawDelay = 0.5f;
    [SerializeField] float discardDelay = 0.2f;
    [SerializeField] float resetDelay = 0.1f;
    [SerializeField] float resetMoveDelay = 0.1f;
    [SerializeField] float moveDelay = 0.5f;

    void Start()
    {
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
        #endregion

        #region ResetDeckInitiation
        cardBackObjectList = new List<GameObject>(listSize);

        // 카드 뒷면 오브젝트 생성해서 리스트에 추가하고 enable 처리
        for (int i = 0; i < listSize; i++)
        {
            cardBackObjectList.Add(Instantiate(cardBackPrefab, cardDumpPoint.position, Utils.QI, cardBackGroup));
            cardBackObjectList[i].SetActive(false);
        }
        #endregion

        GameManager.Instance.onStartBattle.AddListener(StartBattle);
    }

    private void Update()
    {
        // 마우스 우클릭 시
        if (Input.GetMouseButtonDown(1))
        {
            // 선택한 카드를 취소한다.
            selectCard.CancelWithRightClick();
        }
    }

    private void StartBattle()
    {
        UpdateDeckCount();
        MergeDumpToDeck();
        SetUpDeck();
    }

    // 카드를 덱에 추가한다. (string)
    public void AddCardToDeck(string cardName)
    {
        deck.Add(cardDict[cardName]);
        UpdateDeckCount();
    }

    // 카드를 덱에 추가한다. (CardData)
    public void AddCardToDeck(CardData card)
    {
        deck.Add(card);
        UpdateDeckCount();
    }

    // 카드를 덱에서 삭제한다.
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

        // drawBuffer에 개수만큼 등록한다. (데이터적으로, 한 번에 drawBuffer에 추가된다.)
        for (int i = 0; i < drawCount; ++i)
        {
            GameObject cardObject = Instantiate(cardPrefab, cardSpawnPoint.position, Utils.QI, handGroup);
            Card card = cardObject.GetComponent<Card>();

            // DrawCard() 호출 전에 덱이 비었는지 확인
            if (deck.Count == 0)
            {
                StartCoroutine(ResetDeckAnimationCo(dump.Count));
                ResetDeck();
            }

            card.effectGroup = effectGroup;
            card.Setup(DrawCard());
            card.transform.localScale = Vector3.zero;
            drawBuffer.Add(card);
        }

        // 애니메이션을 순차적으로 실행한다.
        for(int i = 0; i < drawCount; ++i)
        {

            // 드로우 버퍼의 첫 장을 골라
            Card card = drawBuffer[0];

            // 드로우 애니메이션을 실행하고, 끝나면 버퍼에서 삭제, hand에 추가한다.
            StartCoroutine(DrawAnimationCo(card));
            drawBuffer.RemoveAt(0);
            hand.Add(card);

            // Hand 카드 순서 정렬
            SetOriginOrder();
            // 덱 텍스트를 변경해준다.
            UpdateDeckCount();

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
        UpdateDeckCount();
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

        // 추후 풀링 예정
        DestroyImmediate(card.gameObject);
    }

    public void ClearSelectCard()
    {
        selectCard = null;
    }

    // dump와 hand를 덱으로 모아서 셔플
    public void SetUpDeck()
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

    public void MergeDumpToDeck()
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
                .Append(cardBackObjectList[i].transform.DOMoveX(cardResetPoint.position.x, resetMoveDelay).SetEase(Ease.Linear))
                .Join(cardBackObjectList[i].transform.DOMoveY(cardResetPoint.position.y, resetMoveDelay).SetEase(Ease.OutCubic))
                .Append(cardBackObjectList[i].transform.DOMoveX(cardSpawnPoint.position.x, resetMoveDelay).SetEase(Ease.Linear))
                .Join(cardBackObjectList[i].transform.DOMoveY(cardSpawnPoint.position.y, resetMoveDelay).SetEase(Ease.InCubic));

            // 각 카드에 딜레이 주기
            yield return new WaitForSeconds(resetDelay);
        }

        // 전체 애니메이션 종료까지 대기
        yield return new WaitForSeconds(resetMoveDelay * 2 + resetDelay * dumpCount);

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
            // 추후 오브젝트 풀링 예정
            DestroyImmediate(card.gameObject);
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

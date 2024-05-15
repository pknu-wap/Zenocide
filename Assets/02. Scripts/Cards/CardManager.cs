// �赿��
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class CardManager : MonoBehaviour
{
    public static CardManager Inst { get; private set; }
    void Awake() => Inst = this;

    // ������ Ǯ
    [SerializeField] ItemSO itemSO;

    // ī�� ������
    [SerializeField] GameObject cardPrefab;
    [SerializeField] GameObject cardBackPrefab;

    // �ڵ�
    public List<Card> hand;
    [SerializeField] int maxHand = 10;
    [SerializeField] Transform handObject;
    [SerializeField] Transform handLeft;
    [SerializeField] Transform handRight;

    // ī�� ���� Ʈ������
    [SerializeField] public Transform cardSpawnPoint;
    [SerializeField] public Transform cardDrawPoint;
    [SerializeField] public Transform cardResetPoint;
    [SerializeField] public Transform cardDumpPoint;

    // ��, ����
    public List<CardData> deck;
    public List<CardData> dump;
    List<GameObject> cardBack;
    [SerializeField] TMP_Text deckCountTMP;
    [SerializeField] TMP_Text dumpCountTMP;
    [SerializeField] Transform cardBackObject;

    Card selectCard;

    // ���
    int listSize = 100;
    float delay01 = 0.1f;
    float delay03 = 0.3f;
    float delay05 = 0.5f;
    float focusOffset = 100f;

    public Vector3 focusPos;

    void Start()
    {
        focusPos = new Vector3(0f, handLeft.position.y + focusOffset, -3f);

        SetUpDeck();

        dump = new List<CardData>(listSize);
        UpdateDumpCount();

        // ���� ������ ���̱� ���� �̱��� ��� Action���� ȣ��
        TurnManager.OnAddCard += AddCardToHand;

        #region ResetDeckInitiation
        cardBack = new List<GameObject>(listSize);

        // ī�� �޸� ������Ʈ �����ؼ� ����Ʈ�� �߰��ϰ� enable ó��
        for(int i = 0; i < listSize; i++)
        {
            cardBack.Add(Instantiate(cardBackPrefab, cardDumpPoint.position, Utils.QI, cardBackObject));
            cardBack[i].SetActive(false);
        }
        #endregion
    }

    // �� ī��Ʈ�� 0���� Ȯ���ϰ� ����ؾ� ��
    CardData DrawCard()
    {
        CardData card = deck[0];
        deck.RemoveAt(0);
        UpdateDeckCount();
        return card;
    }

    void SetUpDeck()
    {
        deck = new List<CardData>(listSize);

        // itemSO�� ī����� deck�� �߰�
        for (int i = 0; i < itemSO.items.Length; i++) 
        {
            CardData card = itemSO.items[i];
            deck.Add(card);
        }
        UpdateDeckCount();

        // deck ����
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

        var cardObject = Instantiate(cardPrefab, cardSpawnPoint.position, Utils.QI, handObject);
        var card = cardObject.GetComponent<Card>();

        // DrawCard() ȣ�� ���� ���� ������� Ȯ��
        if(deck.Count == 0)
        {
            StartCoroutine(ResetDeckAnimationCo(dump.Count));
            ResetDeck();
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

    // �����ϱ⸦ ������...
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
            // Ÿ�� ��ġ�� leftTr�� rightTr ����, i��° ī���� ��ġ
            Vector3 targetPos = Vector3.Lerp(leftTr.position, rightTr.position, objLerps[i]);
            // targetRot�� �켱 �⺻������.
            Quaternion targetRot = Utils.QI;


            // ī�尡 4�� �̻��� ���� ȸ���� �����Ѵ�.
            if (objCount >= 4)
            {
                // ���� ������, (x-a)^2 + (y-b)^2 = r^2�� ����.
                // x = targetPos.x, a = circleCenter.x, y = curve, b = circleCenter.y, r = height                
                float curve = Mathf.Sqrt(Mathf.Pow(radius, 2) - Mathf.Pow(targetPos.x - circleCenter.x, 2));
                // �������� ��ȯ
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

        // dump�� ī����� deck�� �߰�
        for (int i = 0; i < dump.Count; i++)
        {
            CardData card = dump[i];
            deck.Add(card);
        }
        UpdateDeckCount();

        // deck ����
        for (int i = 0; i < deck.Count; i++)
        {
            int rand = Random.Range(i, deck.Count);
            CardData temp = deck[i];
            deck[i] = deck[rand];
            deck[rand] = temp;
        }

        // dump ����
        dump.Clear();
        UpdateDumpCount();
    }

    IEnumerator ResetDeckAnimationCo(int dumpCount)
    {
        // ī�� �޸� ������Ʈ Ȱ��ȭ
        for (int i = 0; i < dumpCount; i++)
        {
            cardBack[i].SetActive(true);
        }

        for(int i = 0; i < dumpCount; i++)
        {
            // ������ �̵�
            Sequence sequence = DOTween.Sequence()
                .Append(cardBack[i].transform.DOMoveX(cardResetPoint.position.x, delay03))
                .Join(cardBack[i].transform.DOMoveY(cardResetPoint.position.y, delay03)).SetEase(Ease.OutCubic)
                .Append(cardBack[i].transform.DOMoveX(cardSpawnPoint.position.x, delay03))
                .Join(cardBack[i].transform.DOMoveY(cardSpawnPoint.position.y, delay03));

            // �� ī�忡 ������ �ֱ�
            yield return new WaitForSeconds(delay01);
        }

        // ��ü �ִϸ��̼� ������� ���
        yield return new WaitForSeconds(delay03 * 2 + delay01 * dumpCount);

        for (int i = 0; i < dumpCount; i++)
        {
            // ī�� �޸� ������Ʈ �ٽ� �����
            cardBack[i].SetActive(false);
            // ������ �Űܳ��� ������Ʈ �ٽ� ������ ����ġ
            cardBack[i].transform.position = cardDumpPoint.position;
        }
    }

    public IEnumerator DiscardHandCo()
    {
        int handCount = hand.Count;
        for (int i = 0; i < handCount; i++)
        {
            Card card = hand[0];

            // ������ ī�� �̵�
            Sequence sequence = DOTween.Sequence()
                .Append(card.transform.DOMove(cardDumpPoint.position, delay03))
                .Join(card.transform.DORotateQuaternion(Utils.QI, delay03))
                .Join(card.transform.DOScale(Vector3.one, delay03))
                .SetEase(Ease.OutQuad);
            
            hand.RemoveAt(0);
            CardAlignment();

            // sequence ������ ������ ��ٸ���
            yield return new WaitForSeconds(delay03);

            // sequence�� ������ ������Ʈ �ı�
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
        if(TurnManager.Inst.myTurn)
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

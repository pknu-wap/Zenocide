using UnityEngine;
using TMPro;
using DG.Tweening;

public class Card : MonoBehaviour
{
    #region 변수
    [Header("카드 정보")]
    public CardData cardData;

    [Header("컴포넌트")]
    [SerializeField] SpriteRenderer card;
    [SerializeField] SpriteRenderer illust;
    [SerializeField] TMP_Text nameTMP;
    [SerializeField] TMP_Text costTMP;
    [SerializeField] TMP_Text descriptionTMP;
    [SerializeField] CardOrder cardOrder;
    [SerializeField] Collider2D cardCollider;

    [Header("상태")]
    [SerializeField] bool isDragging = false;
    [SerializeField] bool isTargetingCard = false;
    public PRS originPRS;

    // DOTween 시퀀스
    Sequence moveSequence;
    Sequence disappearSequence;
    [SerializeField] float dotweenTime = 0.4f;
    [SerializeField] float focusTime = 0.4f;
    #endregion 변수

    public void Setup(CardData item)
    {
        cardData = item;

        illust.sprite = cardData.sprite;
        nameTMP.text = cardData.name;
        costTMP.text = cardData.cost.ToString();
        descriptionTMP.text = cardData.description;

        cardOrder = GetComponent<CardOrder>();
        cardCollider = GetComponent<Collider2D>();

        isTargetingCard = CardInfo.Instance.IsTargetingCard(cardData.type);
    }

    #region 마우스 상호작용
    // 마우스를 카드 위에 올릴 떄 실행된다.
    void OnMouseEnter()
    {
        if (BattleInfo.Inst.isGameOver)
        {
            return;
        }

        CardManager.Inst.CardMouseEnter(this);

        // 실행 중인 moveSequence가 있다면 종료한다.
        moveSequence.Kill();
    }

    // 마우스가 카드를 벗어날 떄 실행된다.
    void OnMouseExit()
    {
        if (BattleInfo.Inst.isGameOver)
        {
            return;
        }

        if(isDragging == true)
        {
            return;
        }

        CardManager.Inst.CardMouseExit(this);
    }

    // 드래그가 시작될 때 호출된다.
    public void OnMouseDown()
    {
        if (BattleInfo.Inst.isGameOver)
        {
            return;
        }

        // 실행 중인 moveSequence가 있다면 종료한다.
        moveSequence.Kill();

        // 다른 카드의 마우스 이벤트를 막는다.
        CardArrow.Instance.ShowBlocker();

        // 공격 카드일 경우
        if (isTargetingCard)
        {
            // 화살표를 표시한다.
            CardArrow.Instance.ShowArrow();

            // 중앙에서 포커스시킨다.
            FocusCardOnCenter();
        }
        // 공격 카드가 아닐 경우, 아무 일도 하지 않는다. (카드가 마우스를 따라감)

        // 드래그 중임을 표시
        isDragging = true;
    }

    // 드래그 중일 때 계속 호출된다.
    public void OnMouseDrag()
    {
        if (BattleInfo.Inst.isGameOver)
        {
            return;
        }

        // 현재 마우스의 위치를 계산한다.
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        if (isTargetingCard)
        {
            // 타겟팅 카드일 경우, 화살표의 끝이 마우스를 향한다.
            CardArrow.Instance.MoveArrow(worldPosition);
        }
        else
        {
            // 논타겟팅 카드는 카드가 마우스를 부드럽게 따라다닌다.
            transform.position = Vector2.Lerp(transform.position, worldPosition, 0.06f);
        }
    }

    // 드래그가 끝날 때 호출된다.
    public void OnMouseUp()
    {
        if (BattleInfo.Inst.isGameOver)
        {
            return;
        }

        if (isTargetingCard)
        {
            // 화살표를 숨긴다.
            CardArrow.Instance.HideArrow();
        }

        // 다른 카드가 마우스 이벤트를 받게 한다.
        CardArrow.Instance.HideBlocker();

        // 드래그가 끝남을 표시
        isDragging = false;

        // 카드를 사용한다.
        UseCard();
    }
    #endregion 마우스 상호작용

    #region 카드 사용
    // 카드 발동 후 선택된 오브젝트
    public Character selectedCharacter;

    // 카드를 사용한다. (마우스가 놓아지는 시점에 호출)
    private void UseCard()
    {
        // 코스트가 모자란 경우
        if (BattleInfo.Inst.CanUseCost(cardData.cost) == false)
        {
            // 카드 발동을 취소한다.
            CancelUsingCard();

            return;
        }

        // 카드 종류에 따라 Enemy 또는 Field 레이어를 선택한다.
        LayerMask layer = CardInfo.Instance.ReturnLayer(cardData.type);

        // layer가 일치하는, 선택된 오브젝트를 가져온다.
        GameObject selectedObject = GetClickedObject(layer);

        // 오브젝트가 선택되지 않았다면
        if (selectedObject == null)
        {
            // 카드 발동을 취소한다.
            CancelUsingCard();

            return;
        }

        // 이제 공격 타겟을 정해야 한다.
        // 적 오브젝트를 선택하는 카드라면
        if (isTargetingCard)
        {
            // 적 오브젝트의 Character 스크립트를 가져오고
            selectedCharacter = selectedObject.GetComponent<Character>();
        }
        // 그 외는
        else
        {
            // Player를 가져온다.
            selectedCharacter = Player.Instance;
        }

        // 카드를 발동한다.
        CardInfo.Instance.effects[(int)cardData.type](cardData.amount, cardData.turnCount, selectedCharacter);
        // 코스트를 감소시킨다.
        BattleInfo.Inst.UseCost(cardData.cost);

        if (isTargetingCard)
        {
            // 카드를 묘지로 보낸다. 보내는 거 잡아채지 못하게 Collider도 잠깐 꺼둔다.
            cardCollider.enabled = false;
            // 일단 아래의 코드를 그대로 가져왔다. 함수화하면 좋을 듯
            moveSequence = DOTween.Sequence()
                .Append(transform.DOMove(CardManager.Inst.cardDumpPoint.position, dotweenTime))
                .Join(transform.DORotateQuaternion(Utils.QI, dotweenTime))
                .Join(transform.DOScale(Vector3.one, dotweenTime))
                .OnComplete(() => {
                    CardManager.Inst.DiscardCard(this);
                    cardCollider.enabled = true;
                }); // 애니메이션 끝나면 패에서 삭제
        }
        else
        {
            // 카드를 묘지로 보낸다. 보내는 거 잡아채지 못하게 Collider도 잠깐 꺼둔다.
            cardCollider.enabled = false;
            // 일단 아래의 코드를 그대로 가져왔다. 함수화하면 좋을 듯
            moveSequence = DOTween.Sequence()
                // 중앙으로 이동하고
                .Append(transform.DOMove(Vector3.zero, dotweenTime))
                .Join(transform.DORotateQuaternion(Utils.QI, dotweenTime))
                .Join(transform.DOScale(originPRS.scale * 1.2f, dotweenTime))
                // 1초간 정지
                .AppendInterval(focusTime)
                // 묘지로 이동한다.
                .Append(transform.DOMove(CardManager.Inst.cardDumpPoint.position, dotweenTime))
                .Join(transform.DORotateQuaternion(Utils.QI, dotweenTime))
                .Join(transform.DOScale(Vector3.one, dotweenTime))
                .OnComplete(() => {
                    CardManager.Inst.DiscardCard(this);
                    cardCollider.enabled = true;
                }); // 애니메이션 끝나면 패에서 삭제
        }
    }

    // 카드 발동을 취소한다.
    void CancelUsingCard()
    {
        MoveTransform(originPRS, true, 0.5f);
        cardOrder.SetMostFrontOrder(false);
    }
    // 클릭된(드래그 후 마우스를 뗀 순간) 오브젝트를 가져온다.
    GameObject GetClickedObject(LayerMask layer)
    {
        // 마우스 위치를 받아온다.
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // 마우스 위치에 레이캐스트를 쏘고, layer가 일치하는 오브젝트 중 가장 먼저 충돌한 오브젝트를 반환한다.
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0f, layer);

        // layer가 일치하는 오브젝트를 찾았다면
        if (hit.collider != null)
        {
            // 해당 오브젝트를 반환하고
            return hit.transform.gameObject;
        }

        // 아니라면 null을 반환한다.
        return null;
    }
    #endregion 카드 사용

    #region 애니메이션
    public void MoveTransform(PRS destPRS, bool useDotween, float dotweenTime = 0)
    {
        // 드래그 중엔 실행되지 않게 해 부자연스러운 움직임을 방지한다.
        if (isDragging == true)
        {
            return;
        }

        if (useDotween)
        {
            // moveSequence에 위치, 회전, 스케일을 조정하는 DOTween을 연결했다.
            moveSequence = DOTween.Sequence()
                .Append(transform.DOMove(destPRS.pos, dotweenTime))
                .Join(transform.DORotateQuaternion(destPRS.rot, dotweenTime))
                .Join(transform.DOScale(destPRS.scale, dotweenTime));
        }

        else
        {
            transform.position = destPRS.pos;
            transform.rotation = destPRS.rot;
            transform.localScale = destPRS.scale;
        }
    }

    // 카드를 중앙에서 강조한다.
    void FocusCardOnCenter()
    {
        MoveTransform(new PRS(CardManager.Inst.focusPos, Utils.QI, originPRS.scale * 1.2f), true, dotweenTime);

        cardOrder.SetMostFrontOrder(true);
    }
    #endregion 애니메이션
}

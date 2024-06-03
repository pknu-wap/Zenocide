using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

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
    // 카드가 버려졌는가?
    public bool isDiscarded = false;
    public PRS originPRS;

    [Header("런타임 변수")]
    // 카드 발동 후 선택된 오브젝트
    public Character[] selectedCharacter;

    [Header("이펙트")]
    ParticleSystem[] effectObject = new ParticleSystem[10];
    public Transform effectGroup;
    bool isPlaying = false;

    // DOTween 시퀀스
    public Sequence moveSequence;
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

        isTargetingCard = CardInfo.Instance.IsTargetingCard(cardData.skills);

        // CardData 안의 각 skill들의 이펙트 생성
        for(int i = 0; i < item.skills.Length; i++)
        {
            if (item.skills[i].effectPrefeb != null)
            {
                effectObject[i] = Instantiate(item.skills[i].effectPrefeb, effectGroup).GetComponent<ParticleSystem>();
            }
            else
            {
                // 이펙트가 없는 스킬은 null 처리
                effectObject[i] = (null);
            }
        }
    }
    
    // 카드를 버릴 때 오브젝트를 파괴한다
    // 이펙트 출력 도중 카드가 파괴되면 이펙트도 같이 사라진다
    // -> isPlaying으로 방지
    private void OnDestroy()
    {
        foreach(ParticleSystem particle in effectObject)
        {
            if(particle != null && !isPlaying)
            {
                Destroy(particle.gameObject);
            }
        }
    }

    #region 마우스 상호작용
    // 마우스를 카드 위에 올릴 떄 실행된다.
    void OnMouseEnter()
    {
        if (BattleInfo.Instance.isGameOver || isDiscarded)
        {
            return;
        }

        CardManager.Instance.CardMouseEnter(this);

        // 실행 중인 moveSequence가 있다면 종료한다.
        moveSequence.Kill();
    }

    // 마우스가 카드를 벗어날 떄 실행된다.
    void OnMouseExit()
    {
        if (BattleInfo.Instance.isGameOver || isDiscarded)
        {
            return;
        }

        if(isDragging == true)
        {
            return;
        }

        CardManager.Instance.CardMouseExit(this);
    }

    Vector3 arrowOffset = new Vector3(0f, 100f, 3f);

    // 드래그가 시작될 때 호출된다.
    public void OnMouseDown()
    {
        if (BattleInfo.Instance.isGameOver || isDiscarded)
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
            // 현재 마우스의 위치를 계산한다.
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            // 표시 전에 위치를 옮겨, 프레임 단위로 이상한 걸 수정
            CardArrow.Instance.MoveStartPosition(transform.position + arrowOffset);
            CardArrow.Instance.MoveArrow(worldPosition);

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
        if (BattleInfo.Instance.isGameOver || isDiscarded || isDragging == false)
        {
            return;
        }

        // 현재 마우스의 위치를 계산한다.
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        if (isTargetingCard)
        {
            CardArrow.Instance.MoveStartPosition(transform.position + arrowOffset);
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
        if (BattleInfo.Instance.isGameOver || isDiscarded || isDragging == false)
        {
            return;
        }

        if (isTargetingCard)
        {
            // 화살표를 숨긴다.
            CardArrow.Instance.HideArrow();
            moveSequence.Kill();
        }

        // 다른 카드가 마우스 이벤트를 받게 한다.
        CardArrow.Instance.HideBlocker();

        // 드래그가 끝남을 표시
        isDragging = false;
        // 카드를 사용한다.
        StartCoroutine(UseCard());
    }
    #endregion 마우스 상호작용

    #region 카드 사용
    private float skillDelay = 0.5f;

    // 카드를 사용한다. (마우스가 놓아지는 시점에 호출)
    private IEnumerator UseCard()
    {
        // 코스트가 모자란 경우
        if (BattleInfo.Instance.CanUseCost(cardData.cost) == false)
        {
            // 카드 발동을 취소한다.
            CancelUsingCard();

            yield break;
        }

        // 애니메이션이 끝났는지 검사하는 변수
        bool isAnimationDone = false;

        // 타겟팅 스킬일 때
        if (isTargetingCard)
        {
            LayerMask layer = LayerMask.GetMask("Enemy");

            // layer가 일치하는, 선택된 오브젝트를 가져온다.
            GameObject selectedObject = GetClickedObject(layer);

            // 오브젝트가 선택되지 않았다면
            if (selectedObject == null)
            {
                // 카드 발동을 취소한다.
                CancelUsingCard();

                yield break;
            }

            // 코스트를 감소시킨다.
            BattleInfo.Instance.UseCost(cardData.cost);
            // 패에서 카드를 삭제한다. (중복 삭제 방지)
            CardManager.Instance.hand.Remove(this);
            // 선택 카드를 비운다.
            CardManager.Instance.ClearSelectCard();
            // 카드를 정렬한다.
            CardManager.Instance.CardAlignment();
            // 버려졌음을 체크한다.
            isDiscarded = true;

            // 일단 아래의 코드를 그대로 가져왔다. 함수화하면 좋을 듯
            moveSequence = DOTween.Sequence()
                .Append(transform.DOMove(CardManager.Instance.cardDumpPoint.position, dotweenTime))
                .Join(transform.DORotateQuaternion(Utils.QI, dotweenTime))
                .Join(transform.DOScale(Vector3.one, dotweenTime))
                .OnComplete(() => {
                    isDiscarded = false;
                    isAnimationDone = true;
                }); // 애니메이션 끝나면 알림

            // 이제 공격 타겟을 정해야 한다.
            // 적 오브젝트의 Enemy 스크립트를 가져온다
            Enemy selectedEnemy = selectedObject.GetComponent<Enemy>();

            // 카드의 모든 효과를 발동한다.
            for (int i = 0; i < cardData.skills.Length; ++i)
            {
                // 타겟을 정한다.
                selectedCharacter = CardInfo.Instance.GetTarget(cardData.skills[i].target, selectedEnemy);

                // 해당 타겟에게 스킬을 시전한다.
                CardInfo.Instance.ActivateSkill(cardData.skills[i], selectedCharacter, Player.Instance);

                // 이펙트 출력
                if (effectObject[i] != null)
                {
                    effectObject[i].transform.position = selectedEnemy.transform.position;
                    effectObject[i].Play();
                    isPlaying = true;
                }

                // 딜레이를 주면 좀 더 자연스럽다.
                yield return new WaitForSeconds(skillDelay);
            }
        }

        // 논타겟 스킬일 때
        else
        {
            // Field 레이어를 선택한다.
            LayerMask layer = LayerMask.GetMask("Field");

            // layer가 일치하는, 선택된 오브젝트를 가져온다.
            GameObject selectedObject = GetClickedObject(layer);

            // 오브젝트가 선택되지 않았다면
            if (selectedObject == null)
            {
                // 카드 발동을 취소한다.
                CancelUsingCard();

                yield break;
            }

            // 코스트를 감소시킨다.
            BattleInfo.Instance.UseCost(cardData.cost);
            // 패에서 카드를 삭제한다. (중복 삭제 방지)
            CardManager.Instance.hand.Remove(this);
            // 선택 카드를 비운다.
            CardManager.Instance.ClearSelectCard();
            // 카드를 정렬한다.
            CardManager.Instance.CardAlignment();
            // 버려졌음을 체크한다.
            isDiscarded = true;

            // 일단 아래의 코드를 그대로 가져왔다. 함수화하면 좋을 듯
            moveSequence = DOTween.Sequence()
                // 중앙으로 이동하고
                .Append(transform.DOMove(Vector3.zero, dotweenTime))
                .Join(transform.DORotateQuaternion(Utils.QI, dotweenTime))
                .Join(transform.DOScale(originPRS.scale * 1.2f, dotweenTime))
                // 1초간 정지
                .AppendInterval(focusTime)
                // 묘지로 이동한다.
                .Append(transform.DOMove(CardManager.Instance.cardDumpPoint.position, dotweenTime))
                .Join(transform.DORotateQuaternion(Utils.QI, dotweenTime))
                .Join(transform.DOScale(Vector3.one, dotweenTime))
                .OnComplete(() => {
                    isDiscarded = false;
                    isAnimationDone = true;
                }); // 애니메이션 끝나면 알림

            // 카드의 모든 효과를 발동한다.
            for (int i = 0; i < cardData.skills.Length; ++i)
            {
                // 타겟을 정한다. 타겟팅 카드가 아니니, selectedEnemy는 없다.
                selectedCharacter = CardInfo.Instance.GetTarget(cardData.skills[i].target);

                CardInfo.Instance.ActivateSkill(cardData.skills[i], selectedCharacter, Player.Instance);

                // 이펙트 출력
                if (effectObject[i] != null)
                {
                    effectObject[i].Play();
                    isPlaying = true;
                }

                // 딜레이를 주면 좀 더 자연스럽다. -> 코루틴의 필요
                yield return new WaitForSeconds(skillDelay);
            }
        }

        // 애니메이션이 끝날 때까지 기다린다.
        while (isAnimationDone == false)
        {
            yield return null;
        }

        // 카드를 삭제한다.
        CardManager.Instance.DiscardCard(this);
    }

    // 카드 발동을 취소한다.
    public void CancelUsingCard()
    {
        MoveTransform(originPRS, true, 0.5f);
        cardOrder.SetMostFrontOrder(false);
    }

    // 우클릭으로 카드 선택을 취소한다.
    public void CancelWithRightClick()
    {
        if (BattleInfo.Instance.isGameOver || isDiscarded || isDragging == false)
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

        // 이동
        moveSequence = DOTween.Sequence()
            .Append(transform.DOMove(originPRS.pos, 0.5f))
            .Join(transform.DORotateQuaternion(originPRS.rot, 0.5f))
            .Join(transform.DOScale(originPRS.scale, 0.5f));

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
        MoveTransform(new PRS(CardManager.Instance.focusPos, Utils.QI, originPRS.scale * 1.2f), true, dotweenTime);

        cardOrder.SetMostFrontOrder(true);
    }
    #endregion 애니메이션
}

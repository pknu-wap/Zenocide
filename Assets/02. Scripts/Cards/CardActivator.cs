using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardActivator : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("카드 정보")]
    public CardData cardData;

    Vector3 originScale;
    Vector3 largeScale;

    private void Awake()
    {
        originScale = transform.localScale;
        largeScale = 1.2f * transform.localScale;
    }

    #region 마우스 오버(Mouse Hover)
    void OnMouseEnter()
    {
        // 카드의 크기를 키운다. (1.2배로)
        UpscaleCard();
    }

    void OnMouseExit()
    {
        // 카드의 크기를 줄인다. (1배로)
        DownscaleCard();
    }

    // 카드의 크기를 키운다. (1.2배로)
    void UpscaleCard()
    {
        transform.localScale = largeScale;
    }

    // 카드의 크기를 줄인다. (1배로)
    void DownscaleCard()
    {
        transform.localScale = originScale;
    }
    #endregion 마우스 오버(Mouse Hover)

    #region 마우스 클릭 및 드래그
    // 드래그가 시작될 때 호출된다.
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 공격 카드일 경우 화살표를 표시한다.

        // 공격 카드가 아닐 경우, 아무 일도 하지 않는다.
    }

    // 드래그 중일 때 계속 호출된다.
    public void OnDrag(PointerEventData eventData)
    {
        // 공격 카드일 경우, 화살표의 끝이 마우스를 향한다.
    }

    // 드래그가 끝날 때 호출된다.
    public void OnEndDrag(PointerEventData eventData)
    {
        // 카드를 사용한다.
        UseCard();
    }
    #endregion 마우스 클릭 및 드래그

    #region 카드 사용

    // 카드 발동 후 선택된 오브젝트
    public Character selectedCharacter;

    // 카드를 사용한다.
    private void UseCard()
    {
        // 카드 종류에 따라 Enemy 또는 Field 레이어를 선택한다.
        LayerMask layer = CardInfo.Instance.ReturnLayer(cardData.type);

        // layer가 일치하는, 선택된 오브젝트를 가져온다.
        GameObject selectedObject = GetClickedObject(layer);

        // 오브젝트가 선택되지 않았다면
        if (selectedObject == null)
        {
            // 카드 발동을 취소한다.

            return;
        }

        // 이제 공격 타겟을 정해야 한다.
        // 적 오브젝트를 선택하는 카드라면
        if (layer == LayerMask.GetMask("Enemy"))
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

        // 카드를 발동한다. 공격 카드일 경우 선택된 적에게 발동한다.
        CardInfo.Instance.effects[(int)cardData.type](cardData.amount, selectedCharacter);
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

    #region 카드 정리
    public virtual void RemoveCard()
    {
        // 카드를 묘지로 보낸다.
    }
    #endregion 카드 정리
}

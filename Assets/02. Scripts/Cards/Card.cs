// 김민철
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    #region 변수
    [Header("카드 정보")]
    public CardData cardData;

    [Header("카드 UI")]
    private Image spriteRenderer;
    private TextMeshProUGUI descriptionText;
    private TextMeshProUGUI nameText;
    private TextMeshProUGUI costText;
    #endregion 변수

    #region 라이프사이클
    private void Awake()
    {
        spriteRenderer = transform.GetChild(0).GetComponent<Image>();
        descriptionText = transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        nameText = transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();
        costText = transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>();

        UpdateCardInfo(cardData);
    }
    #endregion 라이프사이클

    #region 마우스 오버(Mouse Hover)
    // 마우스가 카드 위에 올라올 때 실행된다.
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        // 카드의 크기를 키운다. (1.2배로)
        UpscaleCard();
    }

    // 마우스가 카드 위에서 벗어날 때 실행된다.
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        // 카드의 크기를 줄인다. (1배로)
        DownscaleCard();
    }

    // 카드의 크기를 키운다. (1.2배로)
    void UpscaleCard()
    {
        transform.localScale = new Vector2(1.2f, 1.2f);
    }

    // 카드의 크기를 줄인다. (1배로)
    void DownscaleCard()
    {
        transform.localScale = new Vector2(1f, 1f);
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

    // 카드 발동 후 선택된 오브젝트
    public GameObject selectedObject;

    // 드래그가 끝날 때 호출된다.
    public void OnEndDrag(PointerEventData eventData)
    {
        // 기본 레이어마스크는 Field이며
        LayerMask layer = LayerMask.GetMask("Field");

        // 공격 카드일 경우 레이어마스크를 Enemy로 변경한다.
        if (cardData.type == EffectType.Attack)
        {
            layer = LayerMask.GetMask("Enemy");
        }

        // layer가 일치하는, 선택된 오브젝트를 가져온다.
        selectedObject = GetClickedObject(layer);

        // 오브젝트가 선택되지 않았다면
        if (selectedObject == null)
        {
            // 카드 발동을 취소한다.

            return;
        }

        // 카드를 발동한다. 공격 카드일 경우 선택된 적에게 발동한다.
        CardInfo.instance.effects[(int)cardData.type](cardData.amount, selectedObject);
    }

    // 클릭된(드래그 후 마우스를 뗀 순간) 오브젝트를 가져온다.
    GameObject GetClickedObject(LayerMask layer)
    {
        // 마우스 위치를 받아온다.
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // 마우스 위치에 레이캐스트를 쏘고, layer가 일치하는 오브젝트 중 가장 먼저 충돌한 오브젝트를 반환한다.
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0f, layer);

        // layer가 일치하는 오브젝트를 찾았다면
        if(hit.collider != null)
        {
            // 해당 오브젝트를 반환하고
            return hit.transform.gameObject;
        }

        // 아니라면 null을 반환한다.
        return null;
    }
    #endregion 마우스 클릭 및 드래그

    #region 정보 변경
    // 카드의 정보를 갱신한다.
    public void UpdateCardInfo(CardData data)
    {
        if (data != null)
        {
            spriteRenderer.sprite = data.sprite;
            descriptionText.text = data.description;
            nameText.text = data.name;
            costText.text = data.cost.ToString();
        }
    }
    #endregion 정보 변경

    #region 카드 정리
    public virtual void RemoveCard()
    {
        // 카드를 묘지로 보낸다.
    }
    #endregion 카드 정리
}

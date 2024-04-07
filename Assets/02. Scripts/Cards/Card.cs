// 김민철
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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

    #region 정보 변경
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

    #region 카드 효과
    public virtual void ActivateCard()
    {
        CardInfo.instance.effects[(int)cardData.effect](cardData.amount, gameObject);
    }
    #endregion 카드 효과

    public virtual void RemoveCard()
    {
        // 카드를 묘지로 보낸다.
    }
}

// 김민철
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [Header("카드 정보")]
    public CardData cardData;

    [Header("카드 UI")]
    private Image spriteRenderer;
    private TextMeshProUGUI descriptionText;
    private TextMeshProUGUI nameText;
    private TextMeshProUGUI costText;

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
        //cardEffects[cardData.effect].Invoke();
    }
    #endregion 카드 효과

    public virtual void RemoveCard()
    {
        // 카드를 묘지로 보낸다.
    }
}

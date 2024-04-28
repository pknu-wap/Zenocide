using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardInfoUpdater : MonoBehaviour
{
    #region 변수
    [Header("카드 정보")]
    public CardData cardData;

    [Header("카드 UI")]
    private SpriteRenderer spriteRenderer;
    private TMP_Text descriptionText;
    private TMP_Text nameText;
    private TMP_Text costText;
    #endregion 변수

    #region 라이프사이클
    private void Awake()
    {
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        nameText = transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>();
        descriptionText = transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>();
        costText = transform.GetChild(3).GetChild(0).GetComponent<TMP_Text>();

        UpdateCardInfo(cardData);
    }
    #endregion 라이프사이클

    #region 정보 변경
    // 카드의 정보를 갱신한다.
    public void UpdateCardInfo(CardData data)
    {
        if (data != null)
        {
            spriteRenderer.sprite = data.sprite;
            nameText.text = data.name;
            descriptionText.text = data.description;
            costText.text = data.cost.ToString();
        }
    }
    #endregion 정보 변경
}

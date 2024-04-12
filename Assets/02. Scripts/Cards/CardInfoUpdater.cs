using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardInfoUpdater : MonoBehaviour
{
    [Header("ī�� ����")]
    public CardData cardData;

    [Header("ī�� UI")]
    private Image spriteRenderer;
    private TextMeshProUGUI descriptionText;
    private TextMeshProUGUI nameText;
    private TextMeshProUGUI costText;

    #region ����������Ŭ
    private void Awake()
    {
        spriteRenderer = transform.GetChild(0).GetComponent<Image>();
        descriptionText = transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        nameText = transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();
        costText = transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>();

        UpdateCardInfo(cardData);
    }
    #endregion ����������Ŭ

    #region ���� ���� �Լ�
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
    #endregion ���� ���� �Լ�
}
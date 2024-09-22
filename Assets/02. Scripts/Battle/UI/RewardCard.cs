using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardCard : MonoBehaviour
{
    [Header("컴포넌트")]
    public CardData cardData;
    [SerializeField] Image illust;
    [SerializeField] TMP_Text nameTMP;
    [SerializeField] TMP_Text descriptionTMP;
    [SerializeField] TMP_Text costTMP;

    private void Awake()
    {
        EnrollComponent();
    }

    private void EnrollComponent()
    {
        illust = transform.GetChild(1).GetComponent<Image>();
        nameTMP = transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>();
        descriptionTMP = transform.GetChild(2).GetChild(1).GetComponent<TMP_Text>();
        costTMP = transform.GetChild(3).GetChild(0).GetComponent<TMP_Text>();
    }

    public void Setup(CardData item)
    {
        cardData = item;

        illust.sprite = cardData.sprite;
        nameTMP.text = cardData.name;
        costTMP.text = cardData.cost.ToString();
        descriptionTMP.text = cardData.description;
        SetDamageDiscription();
    }

    // 데미지를 반영해 설명 텍스트를 변경한다.
    public void SetDamageDiscription()
    {
        string tempDescription = cardData.description;

        for (int i = 0; i < cardData.skills.Length; i++)
        {
            // 총 데미지를 계산해서
            int totalDamage = cardData.skills[i].amount;
            // "damage + 해당하는 스킬의 인덱스"인 부분을 대체
            tempDescription = tempDescription.Replace("damage" + i, totalDamage.ToString());
        }

        descriptionTMP.text = tempDescription;
    }
}

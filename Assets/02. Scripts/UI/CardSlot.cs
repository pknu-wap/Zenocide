using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardSlot : MonoBehaviour
{
    public CardData cardData;
    [SerializeField] SpriteRenderer card;
    [SerializeField] SpriteRenderer illust;
    [SerializeField] TMP_Text nameTMP;
    [SerializeField] TMP_Text costTMP;
    [SerializeField] TMP_Text descriptionTMP;
    public void Setup(CardData item)
    {
        cardData = item;

        illust.sprite = cardData.sprite;
        nameTMP.text = cardData.name;
        costTMP.text = cardData.cost.ToString();
        descriptionTMP.text = cardData.description; 
    }
}

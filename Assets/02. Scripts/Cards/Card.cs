using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using static UnityEditor.Progress;

public class Card : MonoBehaviour
{
    [SerializeField] SpriteRenderer card;
    [SerializeField] SpriteRenderer illust;
    [SerializeField] TMP_Text nameTMP;
    [SerializeField] TMP_Text costTMP;
    [SerializeField] TMP_Text descriptionTMP;
    [SerializeField] Sprite cardBackground;

    public Item item;
    public PRS originPRS;

    public void Setup(Item item)
    {
        this.item = item;

        illust.sprite = this.item.sprite;
        nameTMP.text = this.item.name;
        costTMP.text = this.item.cost.ToString();
        descriptionTMP.text = this.item.description;
    }

    public void MoveTransform(PRS prs, bool useDotween, float dotweenTime = 0)
    {
        if (useDotween)
        {
            transform.DOMove(prs.pos, dotweenTime);
            transform.DORotateQuaternion(prs.rot, dotweenTime);
            transform.DOScale(prs.scale, dotweenTime);
        }
        else
        {
            {
                transform.position = prs.pos;
                transform.rotation = prs.rot;
                transform.localScale = prs.scale;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // 원래 크기 Vector3로 지정
    private Vector3 originalScale;
    // 커질 때의 크기 배율을 설정
    public float scaleMultiplier = 1.2f;

    void Start()
    {
        // 현재 카드 슬롯의 원래 크기 저장
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 카드 슬롯의 크기를 설정된 배율만큼 키움
        transform.localScale = originalScale * scaleMultiplier;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 카드 슬롯의 크기를 원래 크기로 되돌림
        transform.localScale = originalScale;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TargetLockOpener : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject targetLock;
    [SerializeField] Image illust;
    [SerializeField] protected Canvas uiCanvas;

    private void Awake()
    {
        uiCanvas = transform.parent.GetComponent<Canvas>();
        targetLock = transform.parent.parent.GetChild(0).GetChild(2).gameObject;
        illust = transform.parent.parent.GetChild(0).GetChild(0).GetComponent<Image>();
        float width = illust.rectTransform.sizeDelta.x;
        float height = illust.rectTransform.sizeDelta.y;

        targetLock.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (CardArrow.Instance.gameObject.activeSelf) {
            uiCanvas.sortingOrder = 10;
            targetLock.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        uiCanvas.sortingOrder = 0;
        targetLock.SetActive(false);
    }
}

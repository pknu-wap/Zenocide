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

        targetLock.transform.GetChild(0).localPosition = new Vector2(-width / 2, height / 2);
        targetLock.transform.GetChild(1).localPosition = new Vector2(width / 2, height / 2);
        targetLock.transform.GetChild(2).localPosition = new Vector2(-width / 2, -height / 2);
        targetLock.transform.GetChild(3).localPosition = new Vector2(width / 2, -height / 2);

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

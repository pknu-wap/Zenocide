using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatePanelOpener : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject statePanel;
    [SerializeField] Image statePanelImage;
    [SerializeField] protected Canvas uiCanvas;
    public GameObject mouseEventBlocker;

    private void Awake()
    {
        statePanel = transform.parent.GetChild(0).gameObject;
        statePanelImage = statePanel.GetComponent<Image>();
        uiCanvas = transform.parent.GetComponent<Canvas>();
        mouseEventBlocker = GameObject.Find("Mouse Event Blocker");

        statePanel.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //// DOTween을 써서 Fade 효과를 주는 경우. 현재 안 씀
        //statePanelImage.color = Color.clear;
        //statePanel.SetActive(true);

        //// 마우스를 올리면 상태창을 연다.
        //statePanelImage.DOFade(1, duration)
        //    .OnComplete(() => statePanel.SetActive(true));

        // 카드를 드래그 중일 땐 표시하지 않음
        if (!mouseEventBlocker.activeSelf)
        {
            // 강조된 캔버스의 order를 맨 앞으로 변경하고
            uiCanvas.sortingOrder = 10;
            // 패널을 활성화 시킨다.
            statePanel.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //// DOTween을 써서 Fade 효과를 주는 경우. 현재 안 씀
        //statePanelImage.color = Color.white;
        //statePanel.SetActive(true);

        //// 마우스를 내리면 상태창을 닫는다.
        //statePanelImage.DOFade(0, duration)
        //    .OnComplete(() => statePanel.SetActive(false));

        // 강조된 캔버스의 order를 뒤쪽으로 변경하고
        uiCanvas.sortingOrder = 0;
        // 패널을 비활성화 시킨다.
        statePanel.SetActive(false);
    }
}

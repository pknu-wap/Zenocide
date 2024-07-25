using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatePanelOpener : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // 스테이터스 패널
    [SerializeField] GameObject statePanel;

    // 타겟록
    GameObject targetLock;
    Image illust;

    // 캔버스
    [SerializeField] protected Canvas uiCanvas;

    // 마우스 이벤트 블로커
    public GameObject mouseEventBlocker;

    // [SerializeField] Image statePanelImage;

    private void Awake()
    {
        uiCanvas = transform.parent.GetComponent<Canvas>();

        // 스테이터스 패널
        statePanel = transform.parent.GetChild(0).gameObject;
        // statePanelImage = statePanel.GetComponent<Image>();

        statePanel.SetActive(false);

        uiCanvas = transform.parent.GetComponent<Canvas>();
        targetLock = transform.parent.parent.GetChild(0).GetChild(2).gameObject;
        illust = transform.parent.parent.GetChild(0).GetChild(0).GetComponent<Image>();
        float width = illust.rectTransform.sizeDelta.x;
        float height = illust.rectTransform.sizeDelta.y;

        targetLock.SetActive(false);

        // mouseEventBlocker = GameObject.Find("Mouse Event Blocker");
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
        if (CardManager.Instance.IsCardSelected())
        {
            // 강조된 캔버스의 order를 맨 앞으로 변경하고
            uiCanvas.sortingOrder = 10;
            // 패널을 활성화 시킨다.
            statePanel.SetActive(true);
        }

        // 타겟록
        if (CardArrow.Instance.gameObject.activeSelf)
        {
            uiCanvas.sortingOrder = 10;
            targetLock.SetActive(true);
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

        // 타겟록
        targetLock.SetActive(false);
    }
}

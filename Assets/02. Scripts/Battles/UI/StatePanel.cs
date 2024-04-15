using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatePanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject statePanel;
    [SerializeField] Image statePanelImage;

    private void Awake()
    {
        statePanel = transform.parent.GetChild(1).gameObject;
        statePanelImage = statePanel.GetComponent<Image>();

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

        statePanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //// DOTween을 써서 Fade 효과를 주는 경우. 현재 안 씀
        //statePanelImage.color = Color.white;
        //statePanel.SetActive(true);

        //// 마우스를 내리면 상태창을 닫는다.
        //statePanelImage.DOFade(0, duration)
        //    .OnComplete(() => statePanel.SetActive(false));

        statePanel.SetActive(false);
    }
}

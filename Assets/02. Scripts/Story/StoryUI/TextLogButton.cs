using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class TextLogButton : MonoBehaviour,IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    
    [Header("로그 패널 오브젝트")]
    //로그 매니저에서 로그 패널 오브젝트를 가져옴
    public GameObject logPannel ;

    [Header("로그 버튼 아이콘, 텍스트")]
    public Image logButtonIcon;
    public TMP_Text logButtonText;

    [Header("버튼 Sprite 저장 변수")]
    public Sprite[] sprites = new Sprite[2];

    //마우스가 버튼을 클릭할 경우 로그 패널 활성화
    public void OnPointerDown(PointerEventData eventData)
    {
        logPannel.SetActive(!logPannel.activeSelf);
        logButtonIcon.sprite = logPannel.activeSelf ? sprites[0] : sprites[1];
        logButtonText.color  = logPannel.activeSelf ? new Color(1, 0.92f, 0.016f, 1) : new Color(1, 1, 1, 1);
        logButtonIcon.color  = logPannel.activeSelf ? new Color(1, 0.92f, 0.016f, 1) : new Color(1, 1, 1, 1);
    }

    //마우스가 버튼 위에 있을 경우 버튼 색상 변경
    public void OnPointerEnter(PointerEventData eventData)
    {
       if(logPannel.activeSelf)
       {
            logButtonText.color  = new Color(0.5f, 0.46f, 0.008f, 1);
            logButtonIcon.color  = new Color(0.5f, 0.46f, 0.008f, 1);
            return;
       }
       logButtonIcon.color = new Color(0.5f, 0.5f, 0.5f, 1);
       logButtonText.color = new Color(0.5f, 0.5f, 0.5f, 1);
    }
    
    //마우스가 버튼에서 벗어날 경우 버튼 색상 변경
    public void OnPointerExit(PointerEventData eventData)
    {
        if(logPannel.activeSelf)
        {
            logButtonText.color  = new Color(1, 0.92f, 0.016f, 1);
            logButtonIcon.color  = new Color(1, 0.92f, 0.016f, 1);
            return;
        }
        logButtonIcon.color = new Color(1, 1, 1, 1);
        logButtonText.color = new Color(1, 1, 1, 1);
    }

}

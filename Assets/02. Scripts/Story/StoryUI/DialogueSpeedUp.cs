using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class DialogueSpeedUp : MonoBehaviour,IPointerDownHandler,IPointerEnterHandler,IPointerExitHandler
{
    private GameObject SpeedUpButton;
    private Image SpeedUpButtonIcon;
    private TMP_Text SpeedUpButtonText;
    
    [Header("버튼 Sprite 저장 변수")]
    public Sprite[] Sprites = new Sprite[2];
    
    private int SpeedStep = 1;

    void Start()
    {
        SpeedUpButton = GameObject.FindWithTag("Speed Up Button");
        SpeedUpButtonIcon = SpeedUpButton.transform.GetChild(0).GetComponent<Image>();
        SpeedUpButtonText = SpeedUpButton.transform.GetChild(1).GetComponent<TMP_Text>();
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        Color ButtonColor = new Color(1, 1, 1, 1);
        SpeedStep = SpeedStep % 2 + 1;

        switch(SpeedStep)
        {
            case 1:
                ButtonColor = new Color(1, 1, 1, 1);
                break;
            case 2:
                ButtonColor = new Color(1, 0.92f, 0.016f, 1);
                break;
        }

        SpeedUpButtonIcon.sprite = Sprites[SpeedStep - 1];
        SpeedUpButtonText.color  = ButtonColor;
        SpeedUpButtonIcon.color  = ButtonColor;
        SetSpeed();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Color ButtonColor = new Color(1, 1, 1, 1);
        
        switch(SpeedStep)
        {
            case 1:
                ButtonColor = new Color(0.5f, 0.5f, 0.5f, 1);
                break;
            case 2:
                ButtonColor = new Color(0.5f, 0.46f, 0.008f, 1);;
                break;
        }

       SpeedUpButtonIcon.color = ButtonColor;
       SpeedUpButtonText.color = ButtonColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Color ButtonColor = new Color(1, 1, 1, 1);
        
        switch(SpeedStep)
        {
            case 1:
                ButtonColor = new Color(1, 1, 1, 1);
                break;
            case 2:
                ButtonColor = new Color(1, 0.92f, 0.016f, 1);
                break;
        }

       SpeedUpButtonIcon.color = ButtonColor;
       SpeedUpButtonText.color = ButtonColor;
    }

    public void SetSpeed()
    {
        DialogueManager.Instance.DialogueSpeedy(SpeedStep);
    }
    
}

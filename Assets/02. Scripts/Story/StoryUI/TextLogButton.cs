using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class TextLogButton : MonoBehaviour,IPointerDownHandler,IPointerEnterHandler,IPointerExitHandler
{

    public static TextLogButton Instance { get; set; }
    public GameObject LogPannel;
    public GameObject LogButton;
    public Image LogButtonIcon;
    public TMP_Text LogButtonText;
    public GameObject[] LogBoxes;

    [Header("버튼 Sprite 저장 변수")]
    public Sprite[] Sprites = new Sprite[2];
    
    [Header("현재 로그 인덱스")]
    public int CurrentLog = 0;

    [Header("로그 기록 오브젝트")]
    public TMP_Text NameTMP;
    public TMP_Text TextTMP;

    [Header("로그 리스트 변수")]
    public List<string> Names = new List<string>();
    public List<string> Texts = new List<string>();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(this);
        }
        LogPannel = GameObject.FindWithTag("Log Pannel");
        LogButton = GameObject.FindWithTag("Log Button");
        LogButtonIcon = LogButton.transform.GetChild(0).GetComponent<Image>();
        LogButtonText = LogButton.transform.GetChild(1).GetComponent<TMP_Text>();
        GameObject Logs = LogPannel.transform.GetChild(2).GetChild(0).gameObject;
        LogBoxes = new GameObject[Logs.transform.childCount];
        for (int i = Logs.transform.childCount - 1; i >= 0  ; i--)
        {
            LogBoxes[i] = Logs.transform.GetChild(i).gameObject;
            LogBoxes[i].SetActive(false);
        }
        LogPannel.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        LogPannel.SetActive(!LogPannel.activeSelf);
        LogButtonIcon.sprite = LogPannel.activeSelf ? Sprites[0] : Sprites[1];
        LogButtonText.color  = LogPannel.activeSelf ? new Color(1, 0.92f, 0.016f, 1) : new Color(1, 1, 1, 1);
        LogButtonIcon.color  = LogPannel.activeSelf ? new Color(1, 0.92f, 0.016f, 1) : new Color(1, 1, 1, 1);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
       if(LogPannel.activeSelf)
       {
            LogButtonText.color  = new Color(0.5f, 0.46f, 0.008f, 1);
            LogButtonIcon.color  = new Color(0.5f, 0.46f, 0.008f, 1);
            return;
       }
       LogButtonIcon.color = new Color(0.5f, 0.5f, 0.5f, 1);
       LogButtonText.color = new Color(0.5f, 0.5f, 0.5f, 1);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(LogPannel.activeSelf)
        {
            LogButtonText.color  = new Color(1, 0.92f, 0.016f, 1);
            LogButtonIcon.color  = new Color(1, 0.92f, 0.016f, 1);
            return;
        }
        LogButtonIcon.color = new Color(1, 1, 1, 1);
        LogButtonText.color = new Color(1, 1, 1, 1);
    }

    public void ResetLogs()
    {
        for (int i = 0; i < LogBoxes.Length; i++)
        {
            LogBoxes[i].SetActive(false);
        }
        Names.Clear();
        Texts.Clear();
    }

    public void AddLog(string name, string text)
    {
        if(text == "") return;

        bool isSelect = name == "선택지" ? true : false;

        Names.Add(name);
        Texts.Add(text);

        CurrentLog = Names.Count - 1;
        LogBoxes[CurrentLog].SetActive(true);
        LogBoxes[CurrentLog].transform.GetChild(0).gameObject.SetActive(name == "" ? false : true);
        LogBoxes[CurrentLog].transform.GetChild(1).GetComponent<TMP_Text>().alignment = name == "" ? TextAlignmentOptions.TopLeft : TextAlignmentOptions.MidlineLeft;
        TextTMP = LogBoxes[CurrentLog].transform.GetChild(1).GetComponent<TMP_Text>();
        NameTMP = LogBoxes[CurrentLog].transform.GetChild(2).GetComponent<TMP_Text>();
        TextTMP.color = isSelect ? new Color(1, 0.627f, 0.478f, 1) : new Color(1, 1, 1, 1);
        NameTMP.color = isSelect ? new Color(1, 0.627f, 0.478f, 1) : new Color(1, 1, 1, 1);            


        NameTMP.text = Names[CurrentLog];
        TextTMP.text = Texts[CurrentLog];
    }

}

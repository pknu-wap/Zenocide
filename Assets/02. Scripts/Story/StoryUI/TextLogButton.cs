using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class TextLogButton : MonoBehaviour
{

    public static TextLogButton Instance { get; set; }
    public GameObject LogPannel;
    public Image LogButtonIcon;
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
        LogButtonIcon = GameObject.FindWithTag("Log Button Icon").GetComponent<Image>();
        GameObject Logs = LogPannel.transform.GetChild(2).GetChild(0).gameObject;
        LogBoxes = new GameObject[Logs.transform.childCount];
        for (int i = Logs.transform.childCount - 1; i >= 0  ; i--)
        {
            LogBoxes[i] = Logs.transform.GetChild(i).gameObject;
            LogBoxes[i].SetActive(false);
        }
        LogPannel.SetActive(false);
    }

    public void ToggleObject()
    {
        LogPannel.SetActive(!LogPannel.activeSelf);
        LogButtonIcon.sprite = LogPannel.activeSelf ? Sprites[0] : Sprites[1];
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
         
        Names.Add(name);
        Texts.Add(text);

        CurrentLog = Names.Count - 1;
        LogBoxes[CurrentLog].SetActive(true);
        LogBoxes[CurrentLog].transform.GetChild(0).gameObject.SetActive(name == "" ? false : true);
        LogBoxes[CurrentLog].transform.GetChild(1).GetComponent<TMP_Text>().alignment = name == "" ? TextAlignmentOptions.TopLeft : TextAlignmentOptions.MidlineLeft;
        TextTMP = LogBoxes[CurrentLog].transform.GetChild(1).GetComponent<TMP_Text>();
        NameTMP = LogBoxes[CurrentLog].transform.GetChild(2).GetComponent<TMP_Text>();
        NameTMP.text = Names[CurrentLog];
        TextTMP.text = Texts[CurrentLog];
    }

}

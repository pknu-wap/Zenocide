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

    [Header("최대 로그 수 저장 변수")]
    public static int MaxLog = 30;
    
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
        GameObject Logs = LogPannel.transform.GetChild(0).GetChild(0).gameObject;
        Debug.Log(Logs.transform.childCount);
        LogBoxes = new GameObject[Logs.transform.childCount];
        for (int i = Logs.transform.childCount - 1; i >= 0  ; i--)
        {
            LogBoxes[i] = Logs.transform.GetChild(i).gameObject;
            LogBoxes[i].SetActive(false);
            Debug.Log(LogBoxes[i].name);
        }
        LogPannel.SetActive(false);
    }

    public void ToggleObject()
    {
        LogPannel.SetActive(!LogPannel.activeSelf);
        LogButtonIcon.sprite = LogPannel.activeSelf ? Sprites[0] : Sprites[1];
    }

    public void AddLog(string name, string text)
    {
        Debug.Log(name + " : " + text);
        if (Names.Count >= MaxLog)
        {
            Names.RemoveAt(0);
            Texts.RemoveAt(0);
        }
        Names.Add(name);
        Texts.Add(text);

        CurrentLog = Names.Count - 1;
        LogBoxes[CurrentLog].SetActive(true);
        TextTMP = LogBoxes[CurrentLog].transform.GetChild(1).GetComponent<TMP_Text>();
        NameTMP = LogBoxes[CurrentLog].transform.GetChild(2).GetComponent<TMP_Text>();
        NameTMP.text = Names[CurrentLog];
        TextTMP.text = Texts[CurrentLog];
    }

}

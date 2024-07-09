using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class TextLogButton : MonoBehaviour
{

    public static TextLogButton Instance { get; set; }

    [Header("로그 기록 오브젝트")]
    public GameObject LogPannel;
    public GameObject[] LogBoxes;
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
        LogBoxes = GameObject.FindGameObjectsWithTag("Dialogue Log");
        foreach (GameObject Log in LogBoxes)
        {
            Log.SetActive(false);
        }
        LogPannel.SetActive(false);
    }

    public void ToggleObject()
    {
        LogPannel.SetActive(!LogPannel.activeSelf);

    }

    public void AddLog(string name, string text)
    {
        if (Names.Count >= 10)
        {
            Names.RemoveAt(0);
            Texts.RemoveAt(0);
        }
        Names.Add(name);
        Texts.Add(text);
        Debug.Log(name + " : " + text);
        
        for (int i = 0; i < Names.Count; i++)
        {
            LogBoxes[i].SetActive(true);
            TextTMP = LogBoxes[i].transform.GetChild(1).GetComponent<TMP_Text>();
            NameTMP = LogBoxes[i].transform.GetChild(2).GetComponent<TMP_Text>();
            NameTMP.text = Names[i];
            TextTMP.text = Texts[i];
        }
    }

}

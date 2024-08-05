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
    
    [Header("로그 오브젝트")]
    public GameObject LogPannel;
    public GameObject LogButton;
    
    [Header("로그 버튼 아이콘, 텍스트")]
    public Image LogButtonIcon;
    public TMP_Text LogButtonText;

    [Header("로그 박스 오브젝트")]
    public GameObject[] LogBoxes;

    [Header("버튼 Sprite 저장 변수")]
    public Sprite[] Sprites = new Sprite[2];
    
    [Header("현재 로그 인덱스")]
    public int CurrentLog = 0;

    [Header("로그 기록 오브젝트")]
    public TMP_Text NameTMP;
    public TMP_Text TextTMP;
    public TMP_Text EquipTMP;
    public TMP_Text UsedTMP;

    [Header("로그 리스트 변수")]
    public List<string> Names = new List<string>();
    public List<string> Texts = new List<string>();

    [Header("최대 저장 가능 로그 수")]
    private int MaxNum = 30;
    
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
        //30개의 로그 박스들을 저장 후 비활성화
        for (int i = Logs.transform.childCount - 1; i >= 0  ; i--)
        {
            LogBoxes[i] = Logs.transform.GetChild(i).gameObject;
            LogBoxes[i].SetActive(false);
        }
        LogPannel.SetActive(false);
    }

    //마우스가 버튼을 클릭할 경우 로그 패널 활성화
    public void OnPointerDown(PointerEventData eventData)
    {
        LogPannel.SetActive(!LogPannel.activeSelf);
        LogButtonIcon.sprite = LogPannel.activeSelf ? Sprites[0] : Sprites[1];
        LogButtonText.color  = LogPannel.activeSelf ? new Color(1, 0.92f, 0.016f, 1) : new Color(1, 1, 1, 1);
        LogButtonIcon.color  = LogPannel.activeSelf ? new Color(1, 0.92f, 0.016f, 1) : new Color(1, 1, 1, 1);
    }

    //마우스가 버튼 위에 있을 경우 버튼 색상 변경
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
    
    //마우스가 버튼에서 벗어날 경우 버튼 색상 변경
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

    //로그 초기화 함수
    public void ResetLogs()
    {
        for (int i = 0; i < LogBoxes.Length; i++)
        {
            LogBoxes[i].SetActive(false);
        }
        Names.Clear();
        Texts.Clear();
    }

    public void AddLog(Dictionary<string, object> Dialogue,int index)
    {
        const string empty = "";
        //획득 아이템
        string equipItem = Dialogue["Equip Item"].ToString();
        //획득 카드
        string equipCard = Dialogue["Equip Card"].ToString();
        //사용한 아이템
        string usedItem  = index == -1 ? empty : Dialogue["Remove Item" + index].ToString();

        //대사가 없는 경우 로그가 비어 있으므로 함수 종료
        if(Dialogue["Text"].ToString() == "") return;

        //로그가 최대 저장 가능한 로그 수를 넘어갈 경우 가장 오래된 로그 삭제
        if(Names.Count >= MaxNum)
        {
            Names.RemoveAt(0);
            Texts.RemoveAt(0);
        }

        //선택지인지 아닌지 판별
        bool isSelect = index != -1 ? true : false;
        
        //대화의 이름과 텍스트
        string name = isSelect ? "선택지" : Dialogue["Name"].ToString();
        string text = isSelect ? Dialogue["Choice" + index].ToString() :Dialogue["Text"].ToString();
        //로그 리스트에 이름과 텍스트 추가
        Names.Add(name);
        Texts.Add(text);

        //현재 로그 인덱스를 리스트의 마지막 인덱스로 설정
        CurrentLog = Names.Count - 1;
        //로그가 저장될 로그 박스 오브젝트 활성화
        LogBoxes[CurrentLog].SetActive(true);
        //로그가 나레이션일 경우 이름이 출력되는 오브젝트 부분을 비활성화
        LogBoxes[CurrentLog].transform.GetChild(0).gameObject.SetActive(name == "" ? false : true);
        //로그가 나레이션일 경우 텍스트의 정렬을 왼쪽 상단으로 설정, 아닐 경우 왼쪽 중앙으로 설정
        LogBoxes[CurrentLog].transform.GetChild(1).GetComponent<TMP_Text>().alignment = name == "" ? TextAlignmentOptions.TopLeft : TextAlignmentOptions.MidlineLeft;
        //로그의 이름과 텍스트를 설정하기 위해 오브젝트 컴포넌트 할당
        TextTMP = LogBoxes[CurrentLog].transform.GetChild(1).GetComponent<TMP_Text>();
        NameTMP = LogBoxes[CurrentLog].transform.GetChild(2).GetComponent<TMP_Text>();
        //선택지일 경우 색상을 변경
        TextTMP.color = isSelect ? new Color(1, 0.627f, 0.478f, 1) : new Color(1, 1, 1, 1);
        NameTMP.color = isSelect ? new Color(1, 0.627f, 0.478f, 1) : new Color(1, 1, 1, 1);            
        //로그의 이름과 텍스트 설정
        NameTMP.text = Names[CurrentLog];
        TextTMP.text = Texts[CurrentLog];
        //획득 아이템과 카드를 string 변수에 저장
        string EquipText = "";
        EquipText += equipItem is not empty ? equipItem + " " : "";
        EquipText += equipCard is not empty ? equipCard + " " : "";
        //획득한 아이템이나 카드가 있는 경우 오브젝트 활성화
        LogBoxes[CurrentLog].transform.GetChild(3).gameObject.SetActive(equipItem is not empty || equipCard is not empty ? true : false);
        
        //획득한 아이템이나 카드가 있는 경우 Text 오브젝트 활성화
        if(equipItem is not empty || equipCard is not empty)
        {
            EquipTMP = LogBoxes[CurrentLog].transform.GetChild(3).GetChild(0).GetComponent<TMP_Text>();
            EquipTMP.text = EquipText;
            EquipTMP.color = new Color(0, 1, 0, 1);
        }
        
        //사용한 아이템이 있는 경우 오브젝트 활성화
        LogBoxes[CurrentLog].transform.GetChild(4).gameObject.SetActive(usedItem is not empty ? true : false);
        if(usedItem is not empty)
        {
            UsedTMP = LogBoxes[CurrentLog].transform.GetChild(4).GetChild(0).GetComponent<TMP_Text>();
            UsedTMP.text = usedItem;
            UsedTMP.color = new Color(1, 0, 0, 1);
        }    
        
    }

}

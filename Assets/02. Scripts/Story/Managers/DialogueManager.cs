using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using System.Runtime.ExceptionServices;

public class DialogueManager : MonoBehaviour, IPointerDownHandler
{
    [Header("Text 오브젝트")]
    public TMP_Text dialogueText;               
    public TMP_Text dialogueName;

    [Header("선택지 TMP_Text")]               
    public TMP_Text[] choiceText = new TMP_Text[4];

    [Header("선택지 요구 아이템 안내 TMP_Text")]             
    public TMP_Text[] choiceRequireText = new TMP_Text[4];

    [Header("대화창 오브젝트")]      
    public GameObject dialogueBox;              
    public GameObject dialoguePanel;

    [Header("선택지 오브젝트")]            
    public GameObject[] choices = new GameObject[4];

    [Header("대화창 출력 완료 시 대기 표시")]          
    public GameObject waitCursor;

    [Header("CSV 데이터")]
    public List<Dictionary<string, object>> dataCSV;

    [Header("이벤트 데이터 폴더 경로")]
    public string mainEventPath          = "Assets/02. Scripts/Story/EventData SO/MainEvent";
    public string connectedMainEventPath = "Assets/02. Scripts/Story/EventData SO/ConnectedMainEvent";
    public string subEventPath           = "Assets/02. Scripts/Story/EventData SO/SubEvent";

    [Header("전체 이벤트 데이터")]
    public List<EventData> TotalEventList;

    [Header("메인 이벤트 데이터")]
    public List<EventData> MainSOs;

    [Header("서브 이벤트 데이터")]
    public List<EventData> SubSOs;

    [Header("서브 이벤트 개수 변수")]
    int subEventCount = 7;

    [Header("캐릭터 이미지 데이터")]               
    public Image dialogueImage;                 
    public Sprite[] dialogueImages;             

    [Header("Text 데이터")]
    public string[] storyText;                  
    public string[] storyName;                  
    public string[] choiceUpContent;            
    public string[] choiceDownContent;          

    [Header("대화 텍스트 출력 진행 확인 변수")]            
    private bool isTyping = false;

    [Header("대화 텍스트 출력 중단 요청 확인 변수")]              
    private bool cancelTyping = false;          

    [Header("마우스 입력 감지 변수")]
    private bool isClicked = false;

    [Header("일러스트 데이터")]
    public Dictionary<string, int> illustTable = new Dictionary<string, int>()
    {
        {"???", 0},
        {"좀비", 1},
        {"주인공",2}
    };

    [Header("표시될 일러스트 이름")]
    public string illustName; 

    // 카드 추가 함수 AddCardtoDeck  
    // 전투 시작 시 MergeDumpToDeck, SetUpDeck 선호출
    // BattleInfo.Instance.StartBattle(string[] str) <= 전투 시작 
    void Start()
    {
        // CSV 파일 읽기
        dataCSV = CSVReader.Read("DialogueScript");        
        // 선택지 비활성화
        foreach(GameObject choice in choices)
        {
            choice.SetActive(false);
        }                            
        // 이벤트 데이터 로드
        LoadSOFromAsset();

        StartCoroutine(EventProcess());                        
    }

     public void OnPointerDown(PointerEventData eventData)
    {
        isClicked = true;
    }

    // 이벤트 처리 함수
    private IEnumerator EventProcess()
    {
        for(int i = 0; i < TotalEventList.Count; i++)
        {
            EventData loadedEvent = TotalEventList[i];
            for(int j = loadedEvent.startIndex; j < loadedEvent.endIndex + 1; j++)
            {
                Debug.Log(i + "" + j);
                dialogueName.text = dataCSV[j]["Name"].ToString();
                dialogueText.text = dataCSV[j]["Text1"].ToString();
                // 캐릭터 이미지 변경
                dialogueImage.sprite = dialogueImages[illustTable[dialogueName.text]];
                DisplayDialogue(j);

                // 마우스 입력 대기
                yield return new WaitUntil(() => isClicked);
                isClicked = false;
            }
        }
            
    }

    // 대화 출력 함수
    private void DisplayDialogue(int index)
    {
        if (isTyping && !cancelTyping)
        {
            cancelTyping = true;
            return;
        }

        if (dataCSV[index]["ChoiceCount"].ToString() != "")
        {
            DisplayChoices(index);
            return;
        }
        // 대화 출력
        StartCoroutine(TypeSentence(dialogueText.text));

        // 획득 아이템이 존재 한다면 아이템 지급
        if(dataCSV[index]["EquipItem"].ToString() != "")
        {
            string equipItem = dataCSV[index]["EquipItem"].ToString();
            Items.instance.AddItem(equipItem);
            dialogueText.text = equipItem + " 을(를) 획득했습니다.";
            StartCoroutine(TypeSentence(dialogueText.text));
        }
    }

    // 텍스트 출력 효과 함수
    private IEnumerator TypeSentence(string sentence)
    {   
        // 대화 진행 시작
        isTyping = true;
        // 다음 대화로 넘어가기 전에 기다리는 커서 비활성화
        waitCursor.SetActive(false);
        // 마우스 입력 시 출력 취소 변수 초기화
        cancelTyping = false;
        dialogueText.text = "";

        foreach (char letter in sentence)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.03f);
            // 타이핑 효과 취소 시 대화를 한번에 출력
            if (cancelTyping)
            {
                dialogueText.text = sentence;
                break;
            }
        }
        // 대화 진행 종료
        isTyping = false;
        waitCursor.SetActive(true);
    }

    private void DisplayChoices(int index)
    {
        dialogueText.text = "....";
        dialogueName.text = "";
        dialoguePanel.SetActive(false);
        for(int i = 0; i < (int)dataCSV[index]["ChoiceCount"]; i++)
        {
            Debug.Log("Choice" + (i + 1));
            choices[i].SetActive(true);
            choiceText[i].text = dataCSV[index]["Choice" + (i + 1)].ToString();

            string requireItem = dataCSV[index]["RequireItem" + (i + 1)].ToString();
            if (requireItem != "")
            {
                if(Items.items.Contains(requireItem))
                {
                    choiceRequireText[i].text = "필요한 아이템: <color=green>" + requireItem + "</color>";
                }
                else
                {
                    choiceRequireText[i].text = "필요한 아이템: <color=red>" + requireItem + "</color>";
                }
            }
            choiceRequireText[i].text = "";
        }

    }

     private void LoadSOFromAsset()
    {
        string[] mainEventNames = AssetDatabase.FindAssets("t:EventData", new[] { mainEventPath });
        int mainEventCount = mainEventNames.Length;
        Array.Sort(mainEventNames);
        Array.Reverse(mainEventNames);
        
        string[] subEventNames  = AssetDatabase.FindAssets("t:EventData", new[] { subEventPath });
        Array.Sort(subEventNames);
        Array.Reverse(subEventNames);

        for (int i = 0; i < mainEventNames.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(mainEventNames[i]);
            EventData so = AssetDatabase.LoadAssetAtPath<EventData>(assetPath);
            if (so != null)
            {
                MainSOs[i] = so;
            }
        }

        for (int i = 0; i < subEventNames.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(subEventNames[i]);
            EventData so = AssetDatabase.LoadAssetAtPath<EventData>(assetPath);
            if (so != null)
            {
               SubSOs[i] = so;
            }
        }
  
        for(int i = 0; i < mainEventCount; i++)
        {
            // 메인 이벤트 리스트에서 데이터 추출
            EventData presentEvent = MainSOs[i];
            TotalEventList.Add(presentEvent);
            // 로드된 메인 이벤트에 다음 이벤트가 있다면 추가 로드
            if(presentEvent.nextEvent != null)
            {
                TotalEventList.Add(presentEvent.nextEvent);
            }

            //서브 이벤트 랜덤 로드
            for(int j = 0; j < subEventCount; j++)
            {
                EventData randomSubEvent = PickRandomElement(SubSOs);
                TotalEventList.Add(randomSubEvent);
            }

        }            

    }

    // 제네릭 함수를 사용하여 리스트에서 랜덤 요소를 뽑기
    public T PickRandomElement<T>(List<T> array)
    {
        System.Random random = new System.Random();
        int randomIndex = random.Next(array.Count);
        return array[randomIndex];
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;

public class DialogueManager : MonoBehaviour, IPointerDownHandler
{
    [Header("메인 스토리 CSV")]
    // 메인 CSV 파일을 읽어올 리스트
    private List<Dictionary<string, object>> dataMainCSV;
    [SerializeField] private TextAsset MainCSV;
    
    [Header("서브 스토리 CSV")]
    // 서브 CSV 파일을 읽어올 리스트
    private List<Dictionary<string, object>> dataSubCSV;
    [SerializeField] private TextAsset SubCSV;

    [Header("연계 스토리 CSV")]
    // 연계 CSV 파일을 읽어올 리스트
    private List<Dictionary<string, object>> dataRelationCSV;
    [SerializeField] private TextAsset RelationCSV;

    [Header("스토리 카메라 오브젝트")]
    [SerializeField] GameObject storyCamera;

    [Header("Text 오브젝트")]
    public TMP_Text dialogueText;               
    public TMP_Text dialogueName;

    [Header("선택지 TMP_Text")]               
    public TMP_Text[] choiceText = new TMP_Text[4];

    [Header("선택지 요구 아이템 안내 TMP_Text")]             
    public TMP_Text[] choiceRequireText = new TMP_Text[4];

    [Header("대화창 오브젝트")]              
    public GameObject dialoguePanel;
    public GameObject dialogueBlinder;

    [Header("선택지 오브젝트")]            
    public GameObject[] choices = new GameObject[4];

    [Header("선택지 가림막 오브젝트")]
    public GameObject[] choiceBlinders = new GameObject[4];

    [Header("선택지 관리자")]
    public SelectManager selectManager;

    [Header("대화창 출력 완료 시 대기 표시")]          
    public GameObject waitCursor;

    [Header("CSV 데이터")]
    private List<Dictionary<string, object>> dataCSV;

    [Header("이벤트 SO 데이터 폴더 경로")]
    private string mainEventPath          = "Assets/02. Scripts/Story/EventData SO/MainEvent";
    private string subEventPath           = "Assets/02. Scripts/Story/EventData SO/SubEvent";

    [Header("전체 이벤트 데이터")]
    public List<EventData> TotalEventList = new List<EventData>();

    [Header("메인 SO 이벤트 데이터")]
    private List<EventData> MainSOs = new List<EventData>();

    [Header("서브 SO 이벤트 데이터")]
    private List<EventData> SubSOs = new List<EventData>();

    [Header("서브 이벤트 개수 변수")]
    public int subEventCount = 7;

    [Header("이벤트별 ID 저장 변수")]
    public const int MainEventID = 0;
    public const int SubEventID = 1;
    public const int RelationEventID = 2;

    [Header("캐릭터 이미지 데이터")]               
    public Image[] IllustsObjects;                 
    public Sprite[] illustImages;            

    [Header("캐릭터 이미지 데이터")]               
    public Image    BackgroundCanvas;                 
    public Sprite[] BackgroundImages;      

    [Header("대화 텍스트 출력 진행 확인 변수")]            
    public bool isTyping = false;

    [Header("대화 텍스트 출력 중단 요청 확인 변수")]              
    public bool cancelTyping = false;          

    [Header("마우스 입력 감지 변수")]
    public bool isClicked = false;

    [Header("일러스트 데이터")]
    public Dictionary<string, int> illustTable = new Dictionary<string, int>()
    {
        {"???", 0},
        {"좀비", 1},
        {"주인공",2}
    };

    [Header("배경 데이터")]
    public Dictionary<string, int> backgroundTable = new Dictionary<string, int>()
    {
        {"배경1", 0},
        {"배경2", 1},
        {"배경3", 2}
    };

    void Start()
    {
        // CSV 파일 읽기
        dataMainCSV         = CSVReader.Read(MainCSV);
        dataSubCSV          = CSVReader.Read(SubCSV);
        dataRelationCSV     = CSVReader.Read(RelationCSV);    
        // 대화창 가림막 비활성화
        dialogueBlinder.SetActive(false);    
        // 선택지 비활성화
        for(int i=0; i<choices.Length; i++)
        {
            choices[i].SetActive(false);
            choiceBlinders[i].SetActive(false);
        }                            
        // 이벤트 데이터 로드
        LoadSOs();
        // 대화 이벤트 시작
        StartCoroutine(EventProcess());                        
    }

    // 이벤트 처리 함수
    private IEnumerator EventProcess()
    {
        for(int i = 0; i < TotalEventList.Count; i++)
        {
            EventData loadedEvent = TotalEventList[i];
            // 이벤트 종류에 따라 불러오는 CSV 데이터 변경
            switch(loadedEvent.eventID)
            {
                case MainEventID:
                    dataCSV = dataMainCSV;
                    break;
                case SubEventID:
                    dataCSV = dataSubCSV;
                    break;
                case RelationEventID:
                    dataCSV = dataRelationCSV;
                    break;
            }
            for(int j = loadedEvent.startIndex; j < loadedEvent.endIndex + 1; j++)
            {
                Dictionary<string, object> presentData = dataCSV[j]; 
                // 대화 출력 함수
                DisplayDialogue(presentData);

                // 마우스 입력 대기
                yield return new WaitUntil(() => isClicked);
                isClicked = false;
            }

            // 이벤트의 모든 요소 종료 이후 관련된 다음 이벤트가 존재 시 추가적으로 실행
            if(loadedEvent.nextEvent.Length != 0)
            {
                EventData relationEvent = loadedEvent.nextEvent[selectManager.result];
                for(int k = relationEvent.startIndex; k < relationEvent.endIndex + 1; k++)
                {
                Dictionary<string, object> relationCSVData = dataRelationCSV[k];
                DisplayDialogue(relationCSVData);
                yield return new WaitUntil(() => isClicked);
                isClicked = false;
                }
            }
        }
            
    }

    // 대화 출력 함수
    private void DisplayDialogue(Dictionary<string, object> csvData)
    {
        List<string> illustNames = new List<string>(){csvData["Name1"].ToString(),
                                                      csvData["Name2"].ToString(), 
                                                      csvData["Name3"].ToString()};
        
        dialogueName.text = csvData["Name1"].ToString();
        string text = csvData["Text1"].ToString();
        // 일러스트 표시 함수
        DisplayIllust(illustNames);
        // 배경 설정
        Debug.Log(csvData["Background"].ToString());
        switch(csvData["Background"].ToString())
        {
            case "":
                break;
            case "배경1":
                BackgroundCanvas.sprite = BackgroundImages[backgroundTable["배경1"]];
                break;
            case "배경2":
                BackgroundCanvas.sprite = BackgroundImages[backgroundTable["배경2"]];
                break;
        }

        if (isTyping && !cancelTyping)
        {
            cancelTyping = true;
            return;
        }
        
        // 획득 아이템이 존재 한다면 아이템 지급
        if(csvData["EquipItem"].ToString() != "")
        {
            string equipItem = csvData["EquipItem"].ToString();
            Items.Instance.AddItem(equipItem);
            text = csvData["EquipItem"].ToString() + " " + text;
        }

        // 획득 카드가 존재 한다면 카드 지급
        if(csvData["EquipCard"].ToString() != "")
        {
            string equipCard = csvData["EquipCard"].ToString();
            Debug.Log(equipCard);
            CardManager.Instance.AddCardToDeck(equipCard);
            text = csvData["EquipCard"].ToString() + " " + text;
        }
        
        // 대화 출력
        dialogueText.text = text;   
        StartCoroutine(TypeSentence(dialogueText.text));

        // 전투 시작
        if(csvData["Enemy1"].ToString() != "")
        {
            StartCoroutine(WaitToEnterBattle(csvData));
        }

        if (csvData["ChoiceCount"].ToString() != "")
        {
            StartCoroutine(DisplayChoices(csvData));
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

    private IEnumerator WaitToEnterBattle(Dictionary<string, object> csvData){
        List<string> Enemys = new List<string>();
        for(int i = 1; i < 4; i++)
        {
            if(csvData["Enemy" + i].ToString() != "")
            {
               Enemys.Add(csvData["Enemy" + i].ToString());
            }
        }
        yield return new WaitUntil(() => isClicked);
        CardManager.Instance.MergeDumpToDeck();
        CardManager.Instance.SetUpDeck();
        GameManager.Instance.StartBattle(Enemys.ToArray());   
    }

    private IEnumerator DisplayChoices(Dictionary<string, object> csvData)
    {
        // 선택지 초기화
        selectManager.ResetChoice();
        // 선택지가 마우스 입력을 감지하기 위해서 대화창 가림막 활성화
        dialogueBlinder.SetActive(true);
        for(int i = 0; i < (int)csvData["ChoiceCount"]; i++)
        {
            choices[i].SetActive(true);
            choiceText[i].text = csvData["Choice" + (i + 1)].ToString();

            string requireItem = csvData["RequireItem" + (i + 1)].ToString();
            if (requireItem != "")
            {
                if(Items.Instance.items.ContainsKey(requireItem))
                {
                    choiceRequireText[i].text = "필요한 아이템: <color=green>" + requireItem + "</color>";
                }
                else
                {
                    choiceRequireText[i].text = "필요한 아이템: <color=red>" + requireItem + "</color>";
                    choiceBlinders[i].SetActive(true);
                }
            }
        }
        yield return new WaitUntil(() => selectManager.result != -1);
        // 선택지 선택 완료 후 마우스 입력 스킵
        isClicked = true;
    }

     private void LoadSOs()
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
                MainSOs.Add(so);
            }
        }

        for (int i = 0; i < subEventNames.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(subEventNames[i]);
            EventData so = AssetDatabase.LoadAssetAtPath<EventData>(assetPath);
            if (so != null)
            {
               SubSOs.Add(so);
            }
        }
  
        for(int i = 0; i < mainEventCount; i++)
        {
            // 메인 이벤트 리스트에서 데이터 추출
            EventData presentEvent = MainSOs[i];
            TotalEventList.Add(presentEvent);

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

    public void DisplayIllust(List<string> names)
    {

        List<string> Names = new List<string>();

        foreach (string name in names)
        {
            if(name != ""){
                Names.Add(name);
            }
        }
        // 일러스트 비활성화
        IllustsObjects[0].enabled = true;
        IllustsObjects[1].enabled = true;
        IllustsObjects[2].enabled = true;
        switch(Names.Count)
        {
            case 1:
                IllustsObjects[0].sprite = illustImages[illustTable[Names[0]]];
                IllustsObjects[1].enabled = false;
                IllustsObjects[2].enabled = false;
                break;
            case 2:
                IllustsObjects[1].sprite = illustImages[illustTable[Names[0]]];
                IllustsObjects[2].sprite = illustImages[illustTable[Names[1]]];
                IllustsObjects[0].enabled = false;
                break;
            case 3:
                IllustsObjects[0].sprite = illustImages[illustTable[Names[0]]];
                IllustsObjects[1].sprite = illustImages[illustTable[Names[1]]];
                IllustsObjects[2].sprite = illustImages[illustTable[Names[2]]];
                break;
        }
    }

    // 마우스 좌클릭 감지 함수
     public void OnPointerDown(PointerEventData eventData)
    {
        isClicked = true;
    }
}

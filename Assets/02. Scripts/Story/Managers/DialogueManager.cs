using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using Unity.VisualScripting.FullSerializer;

public class DialogueManager : MonoBehaviour
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

    [Header("대화창 오브젝트")]              
    public GameObject dialoguePanel;

    [Header("대화창 출력 완료 시 대기 표시")]          
    public GameObject waitCursor;

    [Header("CSV 데이터")]
    private List<Dictionary<string, object>> dataCSV;

    [Header("이벤트 SO 데이터 폴더 경로")]
    private string EventPath = "Assets/02. Scripts/Story/EventData SO/Events";

    [Header("전체 이벤트 데이터")]
    public List<EventData> TotalEventList = new List<EventData>();

    [Header("메인 SO 이벤트 데이터")]
    private List<EventData> MainAndSubSOs = new List<EventData>();

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
    public Image    BackgroundObject;                 
    public Sprite[] BackgroundImages;      

    [Header("대화 텍스트 출력 진행 확인 변수")]            
    public bool isTyping = false;

    [Header("대화 텍스트 출력 중단 요청 확인 변수")]              
    public bool cancelTyping = false;          

    [Header("마우스 입력 감지 변수")]
    public bool isClicked = false;

    [Header("빈 String 값 저장")]
    private const string empty = "";

    [Header("대화 출력 완료 변수")]
    private bool dialoguePrintDone = true;

    public GameObject dialogButton;

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
        EnrollComponent();

        // CSV 파일 읽기
        dataMainCSV         = CSVReader.Read(MainCSV);
        dataSubCSV          = CSVReader.Read(SubCSV);
        dataRelationCSV     = CSVReader.Read(RelationCSV);

        // 대화창 활성화
        dialogButton.SetActive(true);
        // 이벤트 데이터 로드
        LoadSOs();
        // 대화 이벤트 시작
        StartCoroutine(EventProcess());                        
    }

    private void EnrollComponent()
    {

    }

    private IEnumerator EventProcess()
    {
        while(dialoguePrintDone is true)
        {
            // 전체 이벤트에서 무작위 변수 생성
            int randomIndex = UnityEngine.Random.Range(0, TotalEventList.Count);
            EventData loadedEvent = TotalEventList[randomIndex];
            //무작위로 뽑은 인덱스 삭제 후 무작위 이벤트 리스트에 추가
            TotalEventList.RemoveAt(randomIndex);
            TotalEventList.Add(MainAndSubSOs[randomIndex]);

             // 이벤트 종류에 따라 불러오는 CSV 데이터 변경
            switch(loadedEvent.eventID)
            {
                case MainEventID:
                    dataCSV = dataMainCSV;
                    break;
                case SubEventID:
                    dataCSV = dataSubCSV;
                    break;
            }
            Debug.Log(loadedEvent.eventID + " " + loadedEvent.name);
            // 이벤트 별로 SO에 저장된 시작과 끝 인덱스를 따라 대화 진행
            for(int j = loadedEvent.startIndex; j < loadedEvent.endIndex + 1; j++)
            {
                Dictionary<string, object> presentData = dataCSV[j]; 
                // 대화 출력 함수

                // 이벤트의 끝이면 이벤트 종료 후 다음 이벤트 진행을 위해 함수 종료
                if (dataCSV[j]["Name1"].ToString() == "END")
                {
                   break;
                }
                // 연계 이벤트 발생 시 연계 이벤트 우선 실행
                if (dataCSV[j]["Name1"].ToString() == "RELATION")
                {
                    int relationIndex = 0;
                    // 이전 이벤트가 선택지라면
                    if(dataCSV[j - 1]["ChoiceCount"].ToString() is not empty)
                    {
                        //선택지 결과에 따라 연계 이벤트 변경
                        //relationIndex;
                    }
                    // 연계 이벤트 로드
                    EventData relationEvent = loadedEvent.nextEvent[relationIndex];

                    for(int k = relationEvent.startIndex; k < relationEvent.endIndex + 1; k++)
                    {
                        Dictionary<string, object> relationData = dataRelationCSV[k];
                        DisplayDialogue(relationData);
                        // 마우스 입력 대기
                        while (isClicked == false)
                        {
                            yield return null;
                        }
                    }
                }
                DisplayDialogue(presentData);

                // 마우스 입력 대기
                while (isClicked == false)
                {
                    yield return null;
                }

                // 클릭이 끝나면 false로 돌린다.
                isClicked = false;
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
        switch(csvData["Background"].ToString())
        {
            case "":
                break;
            case "배경1":
                BackgroundObject.sprite = BackgroundImages[backgroundTable["배경1"]];
                break;
            case "배경2":
                BackgroundObject.sprite = BackgroundImages[backgroundTable["배경2"]];
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
            CardManager.Instance.AddCardToDeck(equipCard);
            text = csvData["EquipCard"].ToString() + " " + text;
        }
        
        // 대화 출력
        dialogueText.text = text;   
        StartCoroutine(TypeSentence(dialogueText.text));

        // 전투 시작
        if(csvData["Enemy1"].ToString() != "")
        {
            List<string> Enemys = new List<string>();
            for(int i = 1; i < 4; i++)
            {
                if(csvData["Enemy" + i].ToString() != "")
                {
                   Enemys.Add(csvData["Enemy" + i].ToString());
                }
            }
            CardManager.Instance.MergeDumpToDeck();
            CardManager.Instance.SetUpDeck();
            GameManager.Instance.StartBattle(Enemys.ToArray(), "Level 1");
            
            StartCoroutine(WaitToEnterBattle(csvData));
        }

        if (csvData["ChoiceCount"].ToString() != "")
        {
            StartCoroutine(DisplayChoices(csvData));
        }
        
    }

    private IEnumerator DisplayChoices(Dictionary<string, object> csvData)
    {
        // 선택지 띄우기 전 처리
        dialogButton.SetActive(false);

        yield return StartCoroutine(SelectManager.Instance.DisplayChoices(csvData));

        // 선택지 띄우기 후 처리
        dialogButton.SetActive(true);
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
        GameManager.Instance.StartBattle(Enemys.ToArray(), "Level 1");   
    }

     private void LoadSOs()
    {
       // 메인, 서브 이벤트 SO 데이터 이름을 불러와 저장
       string[] EventNames = AssetDatabase.FindAssets("t:EventData", new[] { EventPath });
       int mainEventCount = EventNames.Length;

       for (int i = 0; i < EventNames.Length; i++)
       {
           string assetPath = AssetDatabase.GUIDToAssetPath(EventNames[i]);
           EventData so = AssetDatabase.LoadAssetAtPath<EventData>(assetPath);
           // 무작위 이벤트 추출용 so 리스트
           MainAndSubSOs.Add(so);
           // 초기 무작위 이벤트 리스트
           TotalEventList.Add(so);
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
    public void ClickDialogButton()
    {
        isClicked = true;
    }
}

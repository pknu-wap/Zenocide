using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class DialogueManager : MonoBehaviour
{
    #region 변수
    // 싱글톤
    public static DialogueManager Instance { get; set; }

    [Header("진행 중인 이벤트")]
    public EventData currentEvent = null;

    [Header("현재 사용 중인 CSV 데이터")]
    private List<Dictionary<string, object>> dataCSV;

    [Header("상수 값")]
    private const string emptyString = "";
    private const float typeTime = 0.03f;
    // 이벤트 SO 데이터 폴더 경로
    private string EventPath = "Assets/02. Scripts/Story/EventData SO/Events";

    [Header("상태 체크")]
    // 클릭됐는지 확인한다.
    public bool isClicked = true;
    // 전투가 끝났는지 확인한다.
    public bool isBattleDone = false;
    // 게임이 엔딩에 다다랐는지 검사한다.
    private bool isGameCleared = false;

    [Header("스토리 CSV")]
    // 메인 CSV 파일을 읽어올 리스트
    private List<Dictionary<string, object>> dataMainCSV;
    [SerializeField] private TextAsset MainCSV;
    // 서브 CSV 파일을 읽어올 리스트
    private List<Dictionary<string, object>> dataSubCSV;
    [SerializeField] private TextAsset SubCSV;
    // 연계 CSV 파일을 읽어올 리스트
    private List<Dictionary<string, object>> dataRelationCSV;
    [SerializeField] private TextAsset RelationCSV;

    [Header("일러스트 데이터")]
    // 이름 - 이미지 딕셔너리
    public Dictionary<string, int> illustTable = new Dictionary<string, int>()
    {
        {"소피아", 0},
        {"좀비", 1},
        {"에단", 2},
        {"???", 3},
        {"???아이", 4},
        {"여자 신도", 5},
        {"남자 신도", 6},
        {"옷 입은 좀비", 7},
        {"교주", 8},
        {"흑화 교주", 9},
        {"인카니지 경비원", 10},
    };
    public Sprite[] illustImages;

    [Header("배경 데이터")]
    public Dictionary<string, int> backgroundTable = new Dictionary<string, int>()
    {
        {"Basement", 0},
        {"ZombieTown", 1},
        {"배경3", 2}
    };
    public Sprite[] backgroundImages;

    [Header("대화창 오브젝트")]
    // 대화창 전체 오브젝트
    public GameObject dialoguePanel;
    // 대화창 이름
    public TMP_Text dialogueName;
    // 대화창 내용
    public TMP_Text dialogueText;
    // 클릭받을 버튼
    public GameObject dialogButton;
    // 대기 커서
    public GameObject waitCursor;

    [Header("알림창 오브젝트")]
    public StoryNotification notification;

    [Header("진행 가능한 이벤트 데이터")]
    public List<EventData> processableMainEventList = new List<EventData>();
    public List<EventData> processableSubEventList = new List<EventData>();
    public EventDataList startEventList;
    // 메인 이벤트를 진행할 확률(백분률로 표기)
    public int mainRate = 30;

    [Header("딜레이 딕셔너리")]
    public Dictionary<EventData, int> delayDictionary = new Dictionary<EventData, int>();

    [Header("캐릭터 이미지 데이터")]
    public Image[] IllustsObjects;

    [Header("배경 이미지 데이터")]
    public Image    storyBackgroundObject;
    public Image    battleBackgroundObject;
    #endregion 변수

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

        // 컴포넌트 할당
        EnrollComponent();
        // Total Event List 채우기
        SetTotalEventList();
    }

    void Start()
    {
        // CSV 파일 읽기
        dataMainCSV         = CSVReader.Read(MainCSV);
        dataSubCSV          = CSVReader.Read(SubCSV);
        dataRelationCSV     = CSVReader.Read(RelationCSV);

        // 대화창 활성화
        dialogButton.SetActive(true);
    }

    // 컴포넌트를 할당한다.
    private void EnrollComponent()
    {
        storyBackgroundObject = GameObject.Find("Story BG").GetComponent<Image>();
        battleBackgroundObject = GameObject.Find("Battle BG").GetComponent<Image>();
        notification = GameObject.Find("Story Notification Panel").GetComponent<StoryNotification>();
    }

    // 시작 이벤트 리스트를 전체 이벤트 리스트에 가져온다.
    private void SetTotalEventList()
    {
        for(int i = 0; i < startEventList.list.Length; ++i)
        {
            if (startEventList.list[i].eventID == EventType.Main)
            {
                processableMainEventList.Add(startEventList.list[i]);
            }

            else if (startEventList.list[i].eventID == EventType.Sub)
            {
                processableSubEventList.Add(startEventList.list[i]);
            }
        }
    }

    private EventData GetRandomEvent()
    {
        // 확률 계산 (0~99)
        int randomNumber = Random.Range(0, 100);
        // 이벤트를 가져올 리스트
        List<EventData> processableEventList;

        Debug.Log(randomNumber);

        // 메인 스토리가 선택되면
        if (randomNumber < mainRate)
        {
            processableEventList = processableMainEventList;
        }
        else
        {
            processableEventList = processableSubEventList;
        }

        // 랜덤한 숫자 하나를 고르고
        int randomIndex = Random.Range(0, processableEventList.Count);

        // 해당 이벤트를 리스트에서 가져와 넣는다. (삭제)
        EventData selectedEvent = processableEventList[randomIndex];
        processableEventList.RemoveAt(randomIndex);

        return selectedEvent;
    }

    // 랜덤 이벤트를 선택해 진행한다.
    public IEnumerator ProcessRandomEvent()
    {
        // 게임이 끝나지 않았다면 무한 반복
        while (isGameCleared == false)
        {
            // 현재 이벤트가 없다면
            if(currentEvent == null)
            {
                // 랜덤한 이벤트를 가져온다.
                currentEvent = GetRandomEvent();
            }

            // 이벤트를 진행한다.
            yield return StartCoroutine(ProcessEvent(currentEvent));
        }

        // 끝나면 엔딩.
    }

    // 이벤트를 진행한다.
    private IEnumerator ProcessEvent(EventData loadedEvent)
    {
        // null이 들어오면 바로 종료한다.
        if (loadedEvent == null)
        {
            yield break;
        }

        // 이벤트가 들어있는 CSV 오브젝트를 찾는다.
        switch (loadedEvent.eventID)
        {
            case EventType.Main:
                dataCSV = dataMainCSV;
                break;
            case EventType.Sub:
                dataCSV = dataSubCSV;
                break;
            case EventType.Relation:
                dataCSV = dataRelationCSV;
                break;
        }

        // 첫 문장은 바로 띄운다.
        isClicked = true;

        // 이벤트 진행
        for (int i = loadedEvent.startIndex; i <= loadedEvent.endIndex; ++i)
        {
            // 클릭을 기다린다.
            while (isClicked == false)
            {
                yield return null;
            }
            // 클릭이 감지되면, 재사용을 위해 원상태로 돌린다.
            isClicked = false;

            // 이벤트의 끝이면 함수를 종료한다.
            if (dataCSV[i]["Name"].ToString() == "END")
            {
                // 추가할 이벤트가 있다면
                for (int j = 0; j < loadedEvent.addEvent.Length; ++j)
                {
                    // 딜레이가 있는 경우
                    if (loadedEvent.addEvent[j].delay > 0)
                    {
                        // 딜레이 딕셔너리에 추가한다.
                        delayDictionary.Add(loadedEvent.addEvent[j], loadedEvent.addEvent[j].delay);
                    }
                    else
                    {
                        // 딜레이가 0이라면 바로 processableEventList에 추가 (함수 내에서 타입 검사)
                        AddEventToList(loadedEvent.addEvent[j]);
                    }
                }

                // 딕셔너리에 있는 이벤트들을 처리한다.
                ProcessDelay(loadedEvent);

                // 바로 이어질 이벤트가 있다면 거기로 이동한다. 없으면 null이 된다.
                currentEvent = loadedEvent.nextEvent;

                // 현재 이벤트를 종료한다. (ProcessRandomEvent로 이동)
                yield break;
            }

            // 대화창을 갱신한다. 이 이후의 조건문은, 대화창을 변경하며 실행되는 애들이다.
            yield return StartCoroutine(DisplayDialogue(dataCSV[i]));

            // 선택지가 나타나면 선택지 이벤트를 실행한다.
            if (dataCSV[i]["Choice Count"].ToString() is not emptyString)
            {
                // 선택이 끝날 때까지 대기
                yield return DisplayChoices(dataCSV[i]);

                // 고른 선택지 이벤트 로드
                int result = SelectManager.Instance.result;

                // 사용한 아이템을 제거한다.
                string requiredItem = dataCSV[i]["Remove Item" + (result + 1)].ToString();
                if (requiredItem is not emptyString)
                {
                    Items.Instance.RemoveItem(requiredItem);

                    notification.ShowRemoveItemMessage(requiredItem);
                }

                // 선택된 이벤트를 캐싱해둔다.
                EventData relationEvent = loadedEvent.relationEvent[result];

                // 선택된 이벤트가 null일 경우
                if(relationEvent == null)
                {
                    // 기존 이벤트를 이어서 진행한다.
                    isClicked = true;

                    continue;
                }

                // 그 외엔 선택지 이벤트로 교체하고
                currentEvent = relationEvent;

                // 종료 (ProcessRandomEvent로 이동)
                yield break;
            }

            // 획득 아이템이 존재 한다면 아이템 지급
            if (dataCSV[i]["Equip Item"].ToString() is not emptyString)
            {
                string equipItem = dataCSV[i]["Equip Item"].ToString();
                if(equipItem == "직장에서 챙긴 물건들")
                {
                    Items.Instance.GainJobItem();
                }
                else
                {
                    Items.Instance.AddItem(equipItem);
                }

                notification.ShowGetItemMessage(equipItem);
            }

            // 획득 카드가 존재 한다면 카드 지급
            if (dataCSV[i]["Equip Card"].ToString() is not emptyString)
            {
                string equipCard = dataCSV[i]["Equip Card"].ToString();

                if(equipCard == "직업")
                {
                    CardManager.Instance.GainJobCard();
                }
                else
                {
                    CardManager.Instance.AddCardToDeck(equipCard);
                }

                notification.ShowGetCardMessage(equipCard);
            }

            // 전투가 있다면 시작한다.
            if (dataCSV[i]["Enemy1"].ToString() is not emptyString)
            {
                // 배틀이 끝나지 않았음을 체크
                isBattleDone = false;

                // 이름들을 배열로 받아온다.
                string[] enemies = new string[4];
                for (int j = 0; j < enemies.Length; ++j)
                {
                    // Enemy는 1, 2, 3, 4가 존재
                    enemies[j] = dataCSV[i]["Enemy" + (j + 1)].ToString();
                }

                // 보상 카드 리스트를 불러온다.
                string rewardCardList = dataCSV[i]["Reward Card List"].ToString();

                // 준비가 끝나면 클릭을 기다린다.
                isClicked = false;
                while (isClicked == false)
                {
                    yield return null;
                }
                isClicked = false;

                //클릭되면 배틀을 시작한다.
                GameManager.Instance.StartBattle(enemies, rewardCardList);

                // 전투가 끝날 때까지 기다린다.
                while (isBattleDone == false)
                {
                    yield return null;
                }

                // 끝나면 다음 줄로 바로 이동한다.
                isClicked = true;
            }
        }

        // 이벤트가 종료되지 않고 endIndex를 벗어난 경우 에러를 띄운다.
        Debug.LogError("End Index가 틀렸습니다.");
    }

    // 대화 출력 함수
    private IEnumerator DisplayDialogue(Dictionary<string, object> csvData)
    {
        // 일러스트 배열로 만들어두고
        string[] illustNames = new string[] {
            csvData["Left Image"].ToString(),
            csvData["Center Image"].ToString(),
            csvData["Right Image"].ToString()
        };
        // 일러스트 표시
        DisplayIllust(illustNames);

        // 배경 설정
        if (csvData["Background"].ToString() is not emptyString)
        {
            // 스토리와 전투를 함께 바꾼다.
            storyBackgroundObject.sprite = backgroundImages[backgroundTable[csvData["Background"].ToString()]];
            battleBackgroundObject.sprite = backgroundImages[backgroundTable[csvData["Background"].ToString()]];
        }

        // 이름 변경 (비어있어도 상관 X)
        dialogueName.text = csvData["Name"].ToString();

        // 내용을 받아오고 (비어있어도 상관 X)
        string sentence = csvData["Text"].ToString();
        // 타이핑 출력
        yield return StartCoroutine(TypeSentence(sentence));
    }

    // 일러스트를 띄운다.
    public void DisplayIllust(string[] illustNames)
    {
        // names엔 ""를 포함해 3개가 들어온다.

        // 적용한다.
        for (int i = 0; i < IllustsObjects.Length; ++i)
        {
            // 비었거나, 리스트에 없다면 비활성화
            if (illustNames[i] == "" || illustTable.ContainsKey(illustNames[i]) == false)
            {
                IllustsObjects[i].gameObject.SetActive(false);
            }

            // 있다면 바꾸고 활성화
            else
            {
                IllustsObjects[i].sprite = illustImages[illustTable[illustNames[i]]];
                IllustsObjects[i].gameObject.SetActive(true);
            }
        }
    }

    // 텍스트 출력 효과 함수
    private IEnumerator TypeSentence(string sentence)
    {
        // 다음 대화로 넘어가기 전에 기다리는 커서 비활성화
        waitCursor.SetActive(false);
        // 대화창을 비운다.
        dialogueText.text = "";

        // 출력 취소(캔슬)를 위한 변수 초기화
        isClicked = false;
        for(int i = 0; i < sentence.Length; ++i)
        {
            // 타이핑 효과 취소 시
            if (isClicked == true)
            {
                // 대화를 한 번에 출력
                dialogueText.text = sentence;
                // 클릭 감지를 했으니, 다시 원상태로 돌린다.
                isClicked = false;
                break;
            }

            // 출력할 글자
            string letter = "";
            letter += sentence[i];

            // 만약 html 태그를 만난다면, 한 번에 출력을 위해 letter에 전부 담는다.
            if (letter == "<")
            {
                ++i;
                while (sentence[i] != '>')
                {
                    letter += sentence[i];
                    ++i;
                }
                if (sentence[i] == '>')
                {
                    letter += sentence[i];
                }
            }

            // 한 글자 추가 후 잠시 기다린다.
            dialogueText.text += letter;
            yield return new WaitForSeconds(typeTime);
        }

        // 출력이 끝나면 출력 커서를 띄운다.
        waitCursor.SetActive(true);
    }

    // 선택지를 띄운다.
    private IEnumerator DisplayChoices(Dictionary<string, object> csvData)
    {
        // 선택지 띄우기 전 처리
        // 선택지를 제외한 입력을 받지 않는다.
        dialogButton.SetActive(false);

        // 선택이 끝날 때까지 대기
        yield return StartCoroutine(SelectManager.Instance.DisplayChoices(csvData));

        // 선택지 띄우기 후 처리
        // 다시 기존 입력을 받기 시작한다.
        dialogButton.SetActive(true);

        // 사용한 아이템 제거는 이 함수를 호출한 곳으로 이동했습니다.
    }

    // 리스트에 이벤트를 추가한다.
    private void AddEventToList(EventData eventData)
    {
        // 딜레이 딕셔너리에 추가한다.
        if (eventData.eventID == EventType.Main)
        {
            processableMainEventList.Add(eventData);
        }
        else if (eventData.eventID == EventType.Sub)
        {
            processableSubEventList.Add(eventData);
        }
        else
        {
            Debug.LogError("선택지 이벤트가 processable Event List에 추가되었습니다.");
        }
    }
    
    // 마우스 좌클릭 감지 함수
    public void ClickDialogButton()
    {
        isClicked = true;
    }

    public void ProcessDelay(EventData loadedData)
    {
        // 릴레이션 이벤트일 때는 딜레이 적용 안함
        if (loadedData.eventID == EventType.Relation)
        {
            return;
        }

        // 메인 리스트 딜레이 감소
        List<EventData> events = delayDictionary.Keys.ToList();

        for (int i = 0; i < delayDictionary.Count; i++)
        {
            // 딜레이 감소
            delayDictionary[events[i]] -= 1;

            // 딜레이 만큼 기다렸다면
            if (delayDictionary[events[i]] <= 0)
            {
                // 리스트에 삽입 (이때 타입에 맞게 삽입된다.)
                AddEventToList(events[i]);

                // 딜레이 딕셔너리에서는 삭제
                delayDictionary.Remove(events[i]);
            }
        }
    }
    #region Legacy
    // Legacy
    /*    private IEnumerator EventProcess()
        {
            while (dialoguePrintDone is true)
            {
                // 전체 이벤트에서 무작위 변수 생성
                int randomIndex = Random.Range(0, totalEventList.Count);
                EventData loadedEvent = totalEventList[randomIndex];
                //무작위로 뽑은 인덱스 삭제 후 무작위 이벤트 리스트에 추가
                totalEventList.RemoveAt(randomIndex);
                totalEventList.Add(mainAndSubSOs[randomIndex]);

                // 이벤트 종류에 따라 불러오는 CSV 데이터 변경
                switch (loadedEvent.eventID)
                {
                    case EventType.Main:
                        dataCSV = dataMainCSV;
                        break;
                    case EventType.Sub:
                        dataCSV = dataSubCSV;
                        break;
                }
                Debug.Log(loadedEvent.eventID + " " + loadedEvent.name);
                // 이벤트 별로 SO에 저장된 시작과 끝 인덱스를 따라 대화 진행
                for (int j = loadedEvent.startIndex; j < loadedEvent.endIndex + 1; j++)
                {
                    Dictionary<string, object> presentData = dataCSV[j];
                    // 대화 출력 함수

                    // 이벤트의 끝이면 이벤트 종료 후 다음 이벤트 진행을 위해 함수 종료
                    if (dataCSV[j]["Name"].ToString() == "END")
                    {
                        break;
                    }
                    // 연계 이벤트 발생 시 연계 이벤트 우선 실행
                    if (dataCSV[j]["Name"].ToString() == "RELATION")
                    {
                        int relationIndex = 0;
                        // 이전 이벤트가 선택지라면
                        if (SelectManager.Instance.result != -1)
                        {
                            relationIndex = SelectManager.Instance.result;
                        }
                        // 연계 이벤트 로드
                        EventData relationEvent = loadedEvent.relationEvent[relationIndex];

                        for (int k = relationEvent.startIndex; k < relationEvent.endIndex + 1; k++)
                        {
                            Debug.Log(relationEvent.eventID + " " + relationEvent.name);
                            Dictionary<string, object> relationData = dataRelationCSV[k];
                            isClicked = false;
                            while (isClicked == false)
                            {
                                yield return StartCoroutine(DisplayDialogue(relationData));
                            }
                            isClicked = false;
                        }
                        continue;
                    }

                    isClicked = false;
                    // 마우스 입력 대기
                    while (isClicked == false)
                    {
                        yield return StartCoroutine(DisplayDialogue(presentData)); ;
                    }

                    // 클릭이 끝나면 false로 돌린다.
                    isClicked = false;
                }
            }
        }*/

    // Legacy
    /*private IEnumerator DisplayDialogue(Dictionary<string, object> csvData)
    {
        List<string> illustNames = new List<string>(){
            csvData["Left Image"].ToString(),
            csvData["Center Image"].ToString(), 
            csvData["Right Image"].ToString()
        };
        
        dialogueName.text = csvData["Name"].ToString();
        string text = csvData["Text"].ToString();
        // 일러스트 표시 함수
        DisplayIllust(illustNames);
        // 배경 설정
        if(csvData["Background"].ToString() is not emptyString)
        {
            storyBackgroundObject.sprite = backgroundImages[backgroundTable[csvData["Background"].ToString()]];
            //BattleBackgroundObject.sprite = BackgroundImages[backgroundTable[csvData["Background"].ToString()]]; 
        }
                
         // 획득 아이템이 존재 한다면 아이템 지급
        if(csvData["EquipItem"].ToString() != "")
        {
            string equipItem = csvData["EquipItem"].ToString();
            Items.Instance.AddItem(equipItem);
        }

        // 획득 카드가 존재 한다면 카드 지급
        if(csvData["EquipCard"].ToString() != "")
        {
            string equipCard = csvData["EquipCard"].ToString();
            CardManager.Instance.AddCardToDeck(equipCard);
        }

        // 대화 출력
        dialogueText.text = text;
        isClicked = false;
        yield return StartCoroutine(TypeSentence(dialogueText.text));
        // 전투 시작
        if(csvData["Enemy1"].ToString() is not emptyString)
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
        
    }*/

    // Legacy
    /*private IEnumerator TypeSentence(string sentence)
    {   
        // 다음 대화로 넘어가기 전에 기다리는 커서 비활성화
        waitCursor.SetActive(false);
        // 마우스 입력 시 출력 취소 변수 초기화
        dialogueText.text = "";

        foreach (char letter in sentence)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.03f);
            // 타이핑 효과 취소 시 대화를 한번에 출력
            if (isClicked)
            {
                dialogueText.text = sentence;
                break;
            }
        }
        waitCursor.SetActive(true);
        yield return new WaitUntil(() => isClicked);
    }*/

    // Legacy
    /*    private IEnumerator WaitToEnterBattle(Dictionary<string, object> csvData){
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
        }*/

    // Legacy
    /*    public void DisplayIllust(List<string> names)
        {

            List<string> validNames = names.Where(name => illustTable.ContainsKey(name)).ToList();

            // Disable all illustrations initially
            foreach (Image illustObject in IllustsObjects)
            {
                illustObject.enabled = false;
            }

            for (int i = 0; i < validNames.Count && i < IllustsObjects.Length; i++)
            {
                IllustsObjects[i].sprite = illustImages[illustTable[validNames[i]]];
                IllustsObjects[i].enabled = true;
            }
        }*/

    // 제네릭 함수를 사용하여 리스트에서 랜덤 요소를 뽑기
/*    public T PickRandomElement<T>(List<T> array)
    {
        System.Random random = new System.Random();
        int randomIndex = random.Next(array.Count);
        return array[randomIndex];
    }*/
    #endregion Legacy
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class TutorialManager : MonoBehaviour
{
    // 싱글톤
    public static TutorialManager Instance { get; private set; }

    [Header("진행 중인 이벤트")]
    public EventData currentEvent = null;
    private Coroutine processEvent = null;

    [Header("CSV 데이터")]
    public List<Dictionary<string, object>> dataCSV;
    [SerializeField] private TextAsset tutorailCSV;

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
        {"배경3", 2},
        {"배경4", 3}
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

    [Header("딜레이 딕셔너리")]
    public Dictionary<EventData, int> delayDictionary = new Dictionary<EventData, int>();

    [Header("캐릭터 이미지 데이터")]
    public Image[] IllustsObjects;

    [Header("대화 배속 조절 변수")]
    public int dialogueSpeed = 1;

    [Header("화면 전환 속도 변수")]
    public float fadeSpeed = 1.0f;

    [Header("배경 이미지 데이터")]
    public Image storyBackgroundObject;
    public Image battleBackgroundObject;

    [Header("스킵")]
    public GameObject skipButton;
    public bool isSkip;

    [Header("튜토리얼 이미지")]
    [SerializeField] GameObject[] tutorialPanels;

    [Header("튜토리얼 이벤트")]
    [SerializeField] private EventData[] tutorialEvent;
    Dictionary<string, int> jobTable = new Dictionary<string, int>()
    {
        {"군인", 0 },
        {"의사", 1 },
        {"경찰", 2 },
        {"건설", 3 },
    };

    void Awake()
    {
        Instance = this;

        // 컴포넌트 할당
        EnrollComponent();

        for (int i=0; i<tutorialPanels.Length; i++)
        {
            tutorialPanels[i].SetActive(false);
        }
    }

    void Start()
    {
        // CSV 파일 읽기
        dataCSV = CSVReader.Read(tutorailCSV);

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

    public IEnumerator ProcessEvent(EventData loadedEvent)
    {
        // null이 들어오면 바로 종료한다.
        if (loadedEvent == null)
        {
            yield break;
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

            // 스킵 요청이 들어오면 스킵한다.
            if (isSkip == true)
            {
                isSkip = false;
                break;
            }

            // 클릭이 감지되면, 재사용을 위해 원상태로 돌리고 진행한다.
            isClicked = false;

            // 이벤트의 끝이면
            if (dataCSV[i]["Name"].ToString() == "END")
            {
                // 함수를 종료한다.
                EndEvent(loadedEvent);

                // 현재 이벤트를 종료한다. (DialogueManager로 이동)
                break;
            }

            // 대화창을 갱신한다. 이 이후의 조건문은, 대화창을 변경한 후 실행되는 애들이다.
            yield return StartCoroutine(DisplayDialogue(dataCSV[i]));

            // 선택지가 나타나면 선택지 이벤트를 실행한다.
            if (dataCSV[i]["Choice Count"].ToString() is not emptyString)
            {
                #region 선택지 표시 및 대기
                // 선택지를 띄우고, 선택할 때까지 대기
                yield return DisplayChoices(dataCSV[i]);

                // 선택지가 표시 중일 때도 스킵 요청이 들어오면 스킵
                if (isSkip == true)
                {
                    isSkip = false;
                    yield break;
                }
                #endregion 선택지 표시 및 대기

                #region 선택한 이벤트로 이동
                // 고른 선택지 번호 확인하고
                int result = SelectManager.Instance.result;

                // 선택된 이벤트를 캐싱해둔다.
                EventData relationEvent = loadedEvent.relationEvent[result];

                // 선택된 이벤트가 null일 경우
                if (relationEvent == null)
                {
                    // 기존 이벤트를 이어서 진행한다.
                    isClicked = true;

                    // 다음 줄로 바로 이동한다.
                    continue;
                }

                // 그 외엔 선택지 이벤트로 교체한다.
                currentEvent = relationEvent;
                #endregion 선택한 이벤트로 이동

                #region 사용한 아이템 제거
                // 사용한 아이템을 확인한다.
                string requiredItem = dataCSV[i]["Remove Item" + (result + 1)].ToString();

                // 해당 아이템을 제거한다.
                RemoveUsedItem(requiredItem);
                #endregion 사용한 아이템 제거

                yield break;
            }

            // 획득 아이템이 존재 한다면 아이템 지급
            if (dataCSV[i]["Equip Item"].ToString() is not emptyString)
            {
                string equipItem = dataCSV[i]["Equip Item"].ToString();

                EquipItem(equipItem);
            }

            // 획득 카드가 존재 한다면 카드 지급
            if (dataCSV[i]["Equip Card"].ToString() is not emptyString)
            {
                string equipCard = dataCSV[i]["Equip Card"].ToString();

                EquipCard(equipCard);
            }

            // 전투가 있다면 시작한다.
            if (dataCSV[i]["Enemy1"].ToString() is not emptyString)
            {
                // 이름들을 배열로 받아온다.
                string[] enemies = GetEnemiesName(dataCSV[i]);

                // 보상 카드 리스트를 불러온다.
                string rewardCardList = dataCSV[i]["Reward Card List"].ToString();

                // 전투를 시작한다.
                yield return StartCoroutine(StartBattleTutorial(enemies, rewardCardList));
            }
        }

        // 튜토리얼 이벤트 종료 후에 다른 이벤트로 넘어감
        StartCoroutine(DialogueManager.Instance.ProcessRandomEvent());
    }

    public void StartTutorial()
    {
        // 튜토리얼 이벤트 삽입하고
        currentEvent = tutorialEvent[jobTable[Player.Instance.job]];

        // 튜토리얼 시작
        StartCoroutine(TutorialManager.Instance.ProcessEvent(currentEvent));
        GameManager.Instance.SwitchToStoryScene();
    }

    // dialogue mgr, game mgr, turn manager의 StartBattle을 편집
    IEnumerator StartBattleTutorial(string[] enemies, string rewardCardList)
    {
        // 배틀이 끝나지 않았음을 체크
        isBattleDone = false;

        // 준비가 끝나면 클릭을 기다린다.
        isClicked = false;
        while (isClicked == false)
        {
            yield return null;
        }
        isClicked = false;

        //클릭되면 배틀을 시작한다.
        #region battle
        if (enemies.Length > 4)
        {
            Debug.LogError("적의 숫자가 너무 많습니다. (최대 4)");
        }

        // 적 생성 및 정보 갱신, 추후 분리 예정
        GameManager.Instance.EnrollEnemies(enemies);

        // 적 정렬
        GameManager.Instance.AlignEnemies(enemies);

        // 플레이어 초기화
        Player.Instance.ResetState();

        // 보상 카드 리스트 등록
        GameManager.Instance.rewardCardList = CardInfo.Instance.GetRewardCardListData(rewardCardList);

        // 덱 셔플
        CardManager.Instance.ShuffleDeck();

        // 배틀 카메라로 전환
        GameManager.Instance.SwitchToBattleScene();

        // isGameOver를 false로 변경
        BattleInfo.Instance.isGameOver = false;

        // 게임 세팅
        TurnManager.Instance.GameSetup();

        // 로딩을 시작한다.
        TurnManager.Instance.isLoading = true;

        for (int i = 0; i < tutorialPanels.Length; i++)
        {
            // 튜토리얼 이미지를 띄운다.
            tutorialPanels[i].SetActive(true);

            // 한 번에 다 넘어가지 않게 텀 주기
            yield return new WaitForSeconds(1f);

            // 임의의 키를 누를 때까지 대기
            while (Input.anyKeyDown == false && Input.GetKeyDown(KeyCode.Escape) == false)
            {
                yield return null;
            }

            tutorialPanels[i].SetActive(false);
        }

        // 플레이어 턴을 시작하고, 끝날 때까지 기다린다.
        yield return StartCoroutine(TurnManager.Instance.StartPlayerTurnCo());

        // 로딩을 종료한다.
        TurnManager.Instance.isLoading = false;
        #endregion battle

        // 끝나면 다음 줄로 바로 이동한다.
        isClicked = true;
    }

    public void SkipEvent()
    {
        isSkip = true;
        currentEvent = null;
        skipButton.SetActive(false);

        // 다이얼로그가 출력 중이면
        if (waitCursor.activeSelf == false)
        {
            // 전부 출력
            isClicked = true;
        }

        // 대화 기록을 삭제한다.
        LogManager.Instance.ResetLogs();

        // 다음 이벤트로 넘어간다.
        isClicked = true;
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
        //대화 로그를 저장하는 함수 호출
        LogManager.Instance.AddLog(csvData, -1);
        // 타이핑 출력
        yield return StartCoroutine(TypeSentence(sentence));
    }

    // 일러스트를 띄운다.
    private void DisplayIllust(string[] illustNames)
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
        for (int i = 0; i < sentence.Length; ++i)
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
            //typeTime: 0.03f/dialogueSpeed: 1로 dialogueSpeed 변수를 typeTime에 나누어 출력 속도를 조절한다. ex) 기본 값: 0.03f/1 = 0.03f 2배속: 0.03f/2 = 0.015f
            yield return new WaitForSeconds(typeTime / dialogueSpeed);
        }

        // 출력이 끝나면 출력 커서를 띄운다.
        waitCursor.SetActive(true);
    }

    // 이벤트를 종료한다.
    private void EndEvent(EventData loadedEvent)
    {
        // 다음 이벤트가 들어올 수 있게 초기화한다.
        currentEvent = null;

        // 현재 이벤트의 대화 기록을 삭제한다.
        LogManager.Instance.ResetLogs();
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
    }

    // 사용한 아이템을 제거한다.
    private void RemoveUsedItem(string item)
    {
        // 사용한 아이템을 제거한다.
        if (item is not emptyString)
        {
            Items.Instance.RemoveItems(item);

            notification.ShowRemoveItemMessage(item);
        }
    }

    // 아이템을 획득한다.
    private void EquipItem(string equipItem)
    {
        // 따로 분리하지 않고, 그대로 준다.
        Items.Instance.AddItems(equipItem);

        notification.ShowGetItemMessage(equipItem);
    }

    // 카드를 획득한다.
    private void EquipCard(string equipCard)
    {
        CardManager.Instance.AddCardsToDeck(equipCard);

        notification.ShowGetCardMessage(equipCard);
    }

    // 마우스 좌클릭 감지 함수
    public void ClickDialogButton()
    {
        isClicked = true;
    }

    // 해당 열에 적힌 적들의 이름을 받아온다.
    private string[] GetEnemiesName(Dictionary<string, object> csvData)
    {
        string[] enemies = new string[4];
        for (int j = 0; j < enemies.Length; ++j)
        {
            // Enemy는 1, 2, 3, 4가 존재
            enemies[j] = csvData["Enemy" + (j + 1)].ToString();
        }
        return enemies;
    }

    //대화 배속 조절 함수
    public void DialogueSpeedy(int speed)
    {
        dialogueSpeed = speed;
    }
}
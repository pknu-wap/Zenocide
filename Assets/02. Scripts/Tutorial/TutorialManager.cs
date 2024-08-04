using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

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

    [Header("상수")]
    public const string emptyString = "";

    public bool isPanelShow = false;

    void Awake()
    {
        Instance = this;

        for(int i=0; i<tutorialPanels.Length; i++)
        {
            tutorialPanels[i].SetActive(false);
        }
    }

    public IEnumerator ProcessEvent(EventData loadedEvent)
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
                DialogueManager.Instance.dataCSV = DialogueManager.Instance.dataMainCSV;
                break;
            case EventType.Sub:
                DialogueManager.Instance.dataCSV = DialogueManager.Instance.dataSubCSV;
                break;
            case EventType.Relation:
                DialogueManager.Instance.dataCSV = DialogueManager.Instance.dataRelationCSV;
                break;
        }

        // 첫 문장은 바로 띄운다.
        DialogueManager.Instance.isClicked = true;

        // 이벤트 진행
        for (int i = loadedEvent.startIndex; i <= loadedEvent.endIndex; ++i)
        {
            // 클릭을 기다린다.
            while (DialogueManager.Instance.isClicked == false)
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
            DialogueManager.Instance.isClicked = false;

            // 이벤트의 끝이면
            if (DialogueManager.Instance.dataCSV[i]["Name"].ToString() == "END")
            {
                // 함수를 종료한다.
                DialogueManager.Instance.EndEvent(loadedEvent);

                // 딜레이를 감소시킨다.
                DialogueManager.Instance.ProcessDelay(loadedEvent);

                // 현재 이벤트를 종료한다. (ProcessRandomEvent로 이동)
                yield break;
            }

            // 대화창을 갱신한다. 이 이후의 조건문은, 대화창을 변경한 후 실행되는 애들이다.
            yield return StartCoroutine(DialogueManager.Instance.DisplayDialogue(DialogueManager.Instance.dataCSV[i]));

            // 선택지가 나타나면 선택지 이벤트를 실행한다.
            if (DialogueManager.Instance.dataCSV[i]["Choice Count"].ToString() is not emptyString)
            {
                #region 선택지 표시 및 대기
                // 선택지를 띄우고, 선택할 때까지 대기
                yield return DialogueManager.Instance.DisplayChoices(DialogueManager.Instance.dataCSV[i]);

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
                    DialogueManager.Instance.isClicked = true;

                    // 다음 줄로 바로 이동한다.
                    continue;
                }

                // 그 외엔 선택지 이벤트로 교체한다.
                DialogueManager.Instance.currentEvent = relationEvent;
                #endregion 선택한 이벤트로 이동

                #region 사용한 아이템 제거
                // 사용한 아이템을 확인한다.
                string requiredItem = DialogueManager.Instance.dataCSV[i]["Remove Item" + (result + 1)].ToString();

                // 해당 아이템을 제거한다.
                DialogueManager.Instance.RemoveUsedItem(requiredItem);
                #endregion 사용한 아이템 제거

                yield break;
            }

            // 획득 아이템이 존재 한다면 아이템 지급
            if (DialogueManager.Instance.dataCSV[i]["Equip Item"].ToString() is not emptyString)
            {
                string equipItem = DialogueManager.Instance.dataCSV[i]["Equip Item"].ToString();

                DialogueManager.Instance.EquipItem(equipItem);
            }

            // 획득 카드가 존재 한다면 카드 지급
            if (DialogueManager.Instance.dataCSV[i]["Equip Card"].ToString() is not emptyString)
            {
                string equipCard = DialogueManager.Instance.dataCSV[i]["Equip Card"].ToString();

                DialogueManager.Instance.EquipCard(equipCard);
            }

            // 전투가 있다면 시작한다.
            if (DialogueManager.Instance.dataCSV[i]["Enemy1"].ToString() is not emptyString)
            {
                // 이름들을 배열로 받아온다.
                string[] enemies = DialogueManager.Instance.GetEnemiesName(DialogueManager.Instance.dataCSV[i]);

                // 보상 카드 리스트를 불러온다.
                string rewardCardList = DialogueManager.Instance.dataCSV[i]["Reward Card List"].ToString();

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
        DialogueManager.Instance.currentEvent = tutorialEvent[jobTable[Player.Instance.job]];

        // 튜토리얼 시작
        StartCoroutine(TutorialManager.Instance.ProcessEvent(DialogueManager.Instance.currentEvent));
        GameManager.Instance.SwitchToStoryScene();
    }

    // dialogue mgr, game mgr, turn manager의 StartBattle을 편집
    IEnumerator StartBattleTutorial(string[] enemies, string rewardCardList)
    {
        // 배틀이 끝나지 않았음을 체크
        DialogueManager.Instance.isBattleDone = false;

        // 준비가 끝나면 클릭을 기다린다.
        DialogueManager.Instance.isClicked = false;
        while (DialogueManager.Instance.isClicked == false)
        {
            yield return null;
        }
        DialogueManager.Instance.isClicked = false;

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

        // 배틀 카메라로 전환
        GameManager.Instance.SwitchToBattleScene();

        // isGameOver를 false로 변경
        BattleInfo.Instance.isGameOver = false;

        // 게임 세팅
        TurnManager.Instance.GameSetup();

        // 로딩을 시작한다.
        TurnManager.Instance.isLoading = true;

        // 카드 상호작용을 막는다
        isPanelShow = true;

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

        isPanelShow = false;

        // 플레이어 턴을 시작하고, 끝날 때까지 기다린다.
        yield return StartCoroutine(TurnManager.Instance.StartPlayerTurnCo());

        // 로딩을 종료한다.
        TurnManager.Instance.isLoading = false;
        #endregion battle

        // 끝나면 다음 줄로 바로 이동한다.
        DialogueManager.Instance.isClicked = true;
    }

    // 게임을 시작한다.
    private IEnumerator StartBattleTutorialCo()
    {
        // 게임 세팅
        TurnManager.Instance.GameSetup();

        // 로딩을 시작한다.
        TurnManager.Instance.isLoading = true;

        // 카드 상호작용을 막는다
        isPanelShow = true;

        // 플레이어 턴을 시작하고, 끝날 때까지 기다린다.
        yield return StartCoroutine(TurnManager.Instance.StartPlayerTurnCo());

        for (int i=0; i<tutorialPanels.Length; i++) {
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

        isPanelShow = false;

        // 로딩을 종료한다.
        TurnManager.Instance.isLoading = false;
    }

    public void SkipEvent()
    {
        isSkip = true;
        DialogueManager.Instance.currentEvent = null;
        skipButton.SetActive(false);

        // 다이얼로그가 출력 중이면
        if (DialogueManager.Instance.waitCursor.activeSelf == false)
        {
            // 전부 출력
            DialogueManager.Instance.isClicked = true;
        }

        // 다음 이벤트로 넘어간다.
        DialogueManager.Instance.isClicked = true;
    }
}
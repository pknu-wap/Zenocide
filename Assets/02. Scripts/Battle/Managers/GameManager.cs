using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 치트, UI, 랭킹, 게임오버
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private void Awake() => Instance = this;

    [Header("컴포넌트 및 오브젝트")]
    [SerializeField] private GameObject rewardPanel;
    [SerializeField] NotificationPanel notificationPanel;
    [SerializeField] Transform enemiesParent;
    [SerializeField] public Enemy[] enemies;
    [SerializeField] CardList rewardCardList;

    // 전투 시작 시 실행할 이벤트
    public UnityEvent onStartBattle;

    void Start()
    {
        // 컴포넌트를 미리 할당한다.
        EnrollComponent();

        // 시작은 스토리
        // SwitchToStoryScene();
        // 시작은 배틀
        TestStartBattle();

        // 변수를 찾아 등록한다.
        EnrollComponent();
        SetDefaultState();
    }

    void Update()
    {
#if UNITY_EDITOR
        InputCheatKey();
#endif
    }

    void InputCheatKey()
    {
        if (Input.GetKeyDown(KeyCode.S) && TurnManager.Instance.myTurn)
        {
            // 1장 드로우
            StartCoroutine(CardManager.Instance.AddCardToHand(1));
        }

    }

    // 컴포넌트를 찾아 등록한다.
    private void EnrollComponent()
    {
        rewardPanel = GameObject.Find("Reward Panel");
        // 알림 패널 할당
        notificationPanel = GameObject.Find("Notification Panel").GetComponent<NotificationPanel>();

        // Enemy 프리팹들을 미리 등록해둔다.
        enemies = enemiesParent.GetComponentsInChildren<Enemy>();

        // 카메라 할당
        storyCamera = GameObject.Find("Story Camera");
        battleCamera = GameObject.Find("Battle Camera");
        tutorialCamera = GameObject.Find("Tutorial Camera");
    }

    // 초기 상태를 지정한다.
    private void SetDefaultState()
    {
        rewardPanel.SetActive(false);
    }

    /// <summary>
    /// 전투를 시작한다.
    /// </summary>
    /// <param name="enemyNames">전투 시작 시 생성할 적 ID</param>
    public void StartBattle(string[] enemyNames, string rewardCardListName)
    {
        if(enemyNames.Length > 4)
        {
            Debug.LogError("적의 숫자가 너무 많습니다. (최대 4)");
        }

        // 적 생성 및 정보 갱신, 추후 분리 예정
        EnrollEnemies(enemyNames);

        // 적, 플레이어의 시작 이벤트 호출
        onStartBattle.Invoke();

        // 보상 카드 리스트 등록
        rewardCardList = CardInfo.Instance.GetRewardCardListData(rewardCardListName);

        // 배틀 카메라로 전환
        SwitchToBattleScene();

        // TurnManager를 통해 게임 시작
        StartCoroutine(TurnManager.Instance.StartGameCo());
    }

    public void FinishTutorial()
    {
        // 다시 처음으로 돌리고
        DiaryManager.Instance.currentDialogIndex = 0;
        DiaryManager.Instance.currentPageIndex = 0;

        // 스토리 시작
        tutorialCamera.SetActive(false);
        StartStory();
    }

    // 스토리를 시작한다.
    public void StartStory()
    {
        SwitchToStoryScene();
    }

    // StartBattle 함수를 테스트하는 함수
    public void TestStartBattle()
    {
        StartBattle(new string[] { "NormalZombie", "NormalZombie" }, "Level 1");
    }

    // 모든 적 정보를 등록, 소환한다
    public void EnrollEnemies(string[] enemyNames)
    {
        // enemyNames로 받은 값들은
        int i = 0;
        for(; i < enemyNames.Length; ++i)
        {
            // 데이터를 갱신하고 활성화한다.
            enemies[i].UpdateEnemyData(EnemyInfo.Instance.GetEnemyData(enemyNames[i]));
            enemies[i].gameObject.SetActive(true);
        }

        // enemyNames가 없는, 빈 껍데기들은
        for(; i < enemies.Length; ++i)
        {
            // 비활성화한다.
            enemies[i].gameObject.SetActive(false);
        }
    }

    public void WinBattle()
    {
        Notification("승리");
        StartCoroutine(GiveRewardCard());
    }

    #region 보상 지급
    [SerializeField] private int selectedRewardIndex = -1;

    // 보상 카드를 지급한다.
    public IEnumerator GiveRewardCard()
    {
        // 변수 초기화
        selectedRewardIndex = -1;
        CardData[] rewardCards = new CardData[3];


        // 보상 카드 선택
        //Random.Range(0, );

        // UI 최신화


        // 패널 열기
        rewardPanel.SetActive(true);

        // 선택될 때까지 대기
        while(selectedRewardIndex == -1)
        {
            yield return null;
        }

        Debug.Log("선택: " + selectedRewardIndex);
        // 선택되면 덱에 선택한 카드를 추가한다.
        CardManager.Instance.AddCardToDeck(rewardCards[selectedRewardIndex]);

        // 패널을 닫는다.
        rewardPanel.SetActive(false);
    }

    public void SelectReward(int index)
    {
        selectedRewardIndex = index;
    }
    #endregion 보상 지급

    public void Notification(string message)
    {
        notificationPanel.Show(message);
    }

    #region 카메라 전환
    [SerializeField] GameObject storyCamera;
    [SerializeField] GameObject battleCamera;
    [SerializeField] GameObject tutorialCamera;

    private void SwitchToStoryScene()
    {
        storyCamera.SetActive(true);
        battleCamera.SetActive(false);
    }

    private void SwitchToBattleScene()
    {
        storyCamera.SetActive(false);
        battleCamera.SetActive(true);
    }
    #endregion 카메라 전환
}

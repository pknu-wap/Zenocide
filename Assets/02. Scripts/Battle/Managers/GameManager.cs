using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 치트, UI, 랭킹, 게임오버
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("컴포넌트 및 오브젝트")]
    [SerializeField] private GameObject rewardPanel;
    [SerializeField] private RewardCard[] rewardCards;
    [SerializeField] NotificationPanel notificationPanel;
    [SerializeField] Transform enemiesParent;
    [SerializeField] public Enemy[] enemies;
    [SerializeField] CardList rewardCardList;
    [SerializeField] StatusHP storyHP;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] int enemyLeft;
    [SerializeField] int enemyRight;

    [Header("블로커")]
    [SerializeField] private GameObject storyScene;
    [SerializeField] private GameObject battleScene;
    [SerializeField] private GameObject tutorialScene;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("두 개임");
            Destroy(gameObject);
        }

        // 컴포넌트를 미리 할당한다.
        EnrollComponent();
    }

    void Start()
    {
        // 시작은 튜토리얼
        SwitchToTutorialScene();
        // 시작은 스토리
        // SwitchToStoryScene();
        // 시작은 배틀
        // TestStartBattle();

        SetDefaultState();
    }

    // 컴포넌트를 찾아 등록한다.
    private void EnrollComponent()
    {
        // 리워드 오브젝트 할당
        rewardPanel = GameObject.Find("Reward Panel");
        rewardCards = rewardPanel.GetComponentsInChildren<RewardCard>();
        // 알림 패널 할당
        notificationPanel = GameObject.Find("Battle Notification Panel").GetComponent<NotificationPanel>();

        // Enemy 프리팹들을 미리 등록해둔다.
        enemies = enemiesParent.GetComponentsInChildren<Enemy>();

        // 씬 할당
        storyScene = GameObject.Find("Story Scene");
        battleScene = GameObject.Find("Battle Scene");
        tutorialScene = GameObject.Find("Tutorial Scene");

        // 스토리 씬의 HP 바 할당
        storyHP = GameObject.Find("Player HP").GetComponent<StatusHP>();

        // 게임 오버 패널 할당
        gameOverPanel = GameObject.Find("GameOver Panel");
    }

    // 초기 상태를 지정한다.
    private void SetDefaultState()
    {
        rewardPanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    // 스토리를 시작한다.
    public void StartStory()
    {
        StartCoroutine(DialogueManager.Instance.ProcessRandomEvent());
        SwitchToStoryScene();
    }

    /// <summary>
    /// 전투를 시작한다.
    /// </summary>
    /// <param name="enemyNames">전투 시작 시 생성할 적 ID</param>
    public void StartBattle(string[] enemyNames, string rewardCardListName)
    {
        if (enemyNames.Length > 4)
        {
            Debug.LogError("적의 숫자가 너무 많습니다. (최대 4)");
        }

        // 적 생성 및 정보 갱신, 추후 분리 예정
        EnrollEnemies(enemyNames);

        // 적 정렬
        AlignEnemies(enemyNames);

        // 플레이어 초기화
        Player.Instance.ResetState();

        // 보상 카드 리스트 등록
        rewardCardList = CardInfo.Instance.GetRewardCardListData(rewardCardListName);

        // 덱을 셔플 한다.
        CardManager.Instance.ShuffleDeck();

        // 배틀 카메라로 전환
        SwitchToBattleScene();

        // isGameOver를 false로 변경
        BattleInfo.Instance.isGameOver = false;

        // TurnManager를 통해 게임 시작
        StartCoroutine(TurnManager.Instance.StartGameCo());
    }

    public IEnumerator WinBattle()
    {
        // 승리를 띄우고, 완료될 때까지 기다린 후 보상을 지급한다.
        //Notification("승리");
        yield return StartCoroutine(notificationPanel.Show("승리", true));
        // 리워드 지급이 완료되길 기다린다.
        yield return StartCoroutine(GiveRewardCard());

        // 스토리의 HP 바를 갱신한다.
        storyHP.UpdateHPUI();
        // 배틀이 끝났음을 알린다.
        DialogueManager.Instance.isBattleDone = true;
        // 스토리 씬으로 넘어간다.
        SwitchToStoryScene();

        // 묘지와 핸드를 덱으로 다시 넣고 정렬한다.
        CardManager.Instance.ResetDeck();
        CardManager.Instance.SortDeck();

        // UI도 갱신 (덱이 켜진 채 진입한 경우를 고려)
        CardInventory.instance.UpdateAllCardSlot();
    }

    public IEnumerator GameOver()
    {
        // 슬로우 모션 연출
        Time.timeScale = 0.5f;

        yield return new WaitForSecondsRealtime(3f);

        Time.timeScale = 1f;

        gameOverPanel.SetActive(true);
    }

    public void FinishTutorial()
    {
        // 다시 처음으로 돌리고
        DiaryManager.Instance.currentDialogIndex = 0;
        DiaryManager.Instance.currentPageIndex = 0;

        // 스토리 시작
        tutorialScene.SetActive(false);
        StartStory();
    }

    // 모든 적 정보를 등록, 소환한다
    public void EnrollEnemies(string[] enemyNames)
    {
        // enemyNames로 받은 값들은
        for(int i = 0; i < enemyNames.Length; ++i)
        {
            // 데이터를 갱신하고 활성화한다. null은 Enemy가 처리한다.
            EnemyData enemyData = EnemyInfo.Instance.GetEnemyData(enemyNames[i]);
            enemies[i].EnrollEnemy(enemyData);
        }
    }

    public void AlignEnemies(string[] enemyNames)
    {
        int enemyCount = 0;
        int interval;

        for (int i = 0; i < enemyNames.Length; i++)
        {
            if (enemyNames[i] != "")
            {
                enemyCount++;
            }
        }

        if(enemyCount == 4)
        {
            interval = (Mathf.Abs(enemyLeft) + Mathf.Abs(enemyRight)) / (enemyCount - 1);
            for (int i = 0; i < enemyCount; i++)
            {
                enemies[i].transform.position = new Vector3(enemyLeft + i * interval, 0, 0);
            }
        }
        else
        {
            interval = (Mathf.Abs(enemyLeft) + Mathf.Abs(enemyRight)) / (2 * enemyCount);
            for (int i = 0; i < enemyCount; i++)
            {
                if(i == 0)
                {
                    enemies[i].transform.position = new Vector3(enemyLeft + interval, 0, 0);
                }
                else
                {
                    enemies[i].transform.position = new Vector3(enemyLeft + (2 * i + 1)  * interval, 0, 0);
                }
            }
        }
    }

    #region 보상 지급
    [SerializeField] private int selectedRewardIndex = -1;

    // 보상 카드를 지급한다.
    public IEnumerator GiveRewardCard()
    {
        // 변수 초기화
        selectedRewardIndex = -1;
        // 카드 데이터 배열을, 새로운 리스트로 만든다. (값 복사)
        List<CardData> cardList = rewardCardList.items.ToList();

        // UI 최신화
        for(int i = 0; i < rewardCards.Length; ++i)
        {
            // 보상 카드 선택
            int randomIndex = Random.Range(0, cardList.Count);

            rewardCards[i].Setup(cardList[randomIndex]);
            cardList.RemoveAt(randomIndex);
        }

        // 패널 열기
        rewardPanel.SetActive(true);

        // 선택될 때까지 대기
        while(selectedRewardIndex == -1)
        {
            yield return null;
        }

        // 선택되면 덱에 선택한 카드를 추가한다.
        CardManager.Instance.AddCardToDeck(rewardCards[selectedRewardIndex].cardData);

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
    private void SwitchToStoryScene()
    {
        storyScene.SetActive(true);
        battleScene.SetActive(false);
    }

    private void SwitchToBattleScene()
    {
        storyScene.SetActive(false);
        battleScene.SetActive(true);
    }

    public void SwitchToTutorialScene()
    {
        tutorialScene.SetActive(true);
        storyScene.SetActive(false);
        battleScene.SetActive(false);
    }
    #endregion 카메라 전환
}

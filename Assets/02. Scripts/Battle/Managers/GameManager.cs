using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 치트, UI, 랭킹, 게임오버
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private void Awake() => Instance = this;

    [SerializeField] NotificationPanel notificationPanel;
    [SerializeField] Transform enemiesParent;
    [SerializeField] public Enemy[] enemies;

    // 전투 시작 시 실행할 이벤트
    public UnityEvent onStartBattle;

    void Start()
    {
        // 컴포넌트를 미리 할당한다.
        EnrollComponent();

        // 시작은 스토리
        SwitchToStoryScene();
    }

    private void EnrollComponent()
    {
        // 알림 패널 할당
        notificationPanel = GameObject.Find("Notification Panel").GetComponent<NotificationPanel>();

        // Enemy 프리팹들을 미리 등록해둔다.
        enemies = enemiesParent.GetComponentsInChildren<Enemy>();

        // 카메라 할당
        storyCamera = GameObject.Find("Story Camera");
        battleCamera = GameObject.Find("Battle Camera");
        tutorialCamera = GameObject.Find("Tutorial Camera");
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

    /// <summary>
    /// 전투를 시작한다.
    /// </summary>
    /// <param name="enemyNames">전투 시작 시 생성할 적 ID</param>
    public void StartBattle(string[] enemyNames)
    {
        if(enemyNames.Length > 4)
        {
            Debug.LogError("적의 숫자가 너무 많습니다. (최대 4)");
        }

        // 적 생성 및 정보 갱신, 추후 분리 예정
        EnrollEnemies(enemyNames);

        // 적, 플레이어의 시작 이벤트 호출
        onStartBattle.Invoke();

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
        StartBattle(new string[] { "NormalZombie", "NormalZombie" });
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

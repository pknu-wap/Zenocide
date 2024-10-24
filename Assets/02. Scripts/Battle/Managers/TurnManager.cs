using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }
    private void Awake() => Instance = this;

    [Header("Develop")]
    [SerializeField] [Tooltip("시작 턴 모드를 정합니다")] ETurnMode eTurnMode;
    [SerializeField] [Tooltip("드로우 카드 개수를 정합니다")] int drawCardCount;

    [Header("Properties")]
    public bool isLoading; // 게임 끝나면 isLoading을 true로 하면 카드와 엔티티 클릭방지
    public bool myTurn;

    [Header("Audio Clips")]
    public AudioClip turnEndButton;

    enum ETurnMode { Random, My, Other }

    // 턴 이벤트
    public UnityEvent onStartPlayerTurn;    // 플레이어 턴이 시작할 때
    public UnityEvent onStartEnemyTurn;     // 적 턴이 시작할 때
    public UnityEvent onEndPlayerTurn;      // 플레이어 턴이 끝날 때
    public UnityEvent onEndEnemyTurn;       // 적 턴이 끝날 때

    public void GameSetup()
    {
        switch (eTurnMode)
        {
            case ETurnMode.Random:
                myTurn = Random.Range(0, 2) == 0;
                break;
            case ETurnMode.My:
                myTurn = true;
                break;
            case ETurnMode.Other:
                myTurn = false;
                break;
        }
    }

    // 게임을 시작한다.
    public IEnumerator StartGameCo()
    {
        // 게임 세팅
        GameSetup();

        // 로딩을 시작한다.
        isLoading = true;

        // 플레이어 턴을 시작하고, 끝날 때까지 기다린다.
        yield return StartCoroutine(StartPlayerTurnCo());

        // 로딩을 종료한다.
        isLoading = false;
    }

    // 플레이어 턴을 시작한다.
    public IEnumerator StartPlayerTurnCo()
    {
        // 턴 시작 UI 출력, 이 부분도 추후 수정해야 합니다.
        GameManager.Instance.Notification("나의 턴");

        // 플레이어 턴 시작 시 이벤트 호출
        onStartPlayerTurn.Invoke();

        // 드로우 카드 수만큼 드로우, 끝날 때까지 대기
        yield return StartCoroutine(CardManager.Instance.AddCardToHand(drawCardCount));
    }

    // 적 턴을 시작한다.
    IEnumerator StartEnemyTurnCo()
    {
        yield break;
    }

    public void EndTurn()
    {
        SoundManager.Instance.Play(turnEndButton);

        if (BattleInfo.Instance.isGameOver)
        {
            return;
        }

        if (isLoading == true)
        {
            return;
        }

        // 상호작용을 막늗나.
        isLoading = true;

        StartCoroutine(EndTurnCo());
    }

    public IEnumerator EndTurnCo()
    {
        // 내 턴일 때 호출
        if (myTurn)
        {
            // 카드 전부 버리기
            yield return StartCoroutine(CardManager.Instance.DiscardHandCo());

            onEndPlayerTurn.Invoke();
            onStartEnemyTurn.Invoke();

            // 적 턴 직전, 플레이어가 죽으면 코루틴을 끝낸다.
            if (BattleInfo.Instance.isGameOver)
            {
                yield break;
            }

            // 적 턴으로 변경
            myTurn = false;

            // 적 턴 종료
            yield return StartCoroutine(EndTurnCo());

            // 적 턴 종료 후에도, 플레이어가 죽으면 코루틴을 끝낸다.
            if (BattleInfo.Instance.isGameOver)
            {
                yield break;
            }

            // 플레이어 턴 시작
            yield return StartCoroutine(StartPlayerTurnCo());

            isLoading = false;
        }
        // 적 턴일 때 호출
        else
        {
            #region 적 공격
            // 적이 랜덤한 순서로 스킬을 사용함
            // 더 깔끔한 알고리즘이 있지 않을까
            int enemyIndex = Random.Range(0, 4);
            int count = 0;
            bool[] hasAttack = new bool[4]; // 변수 이름이 부적절한 거 같다.
            Array.Fill(hasAttack, false);

            while (count < 4)
            // 적 목록을 값 복사한다. (적이 죽으면 리스트에서 사라지므로)
            //List<Enemy> enemies = BattleInfo.Instance.remainingEnemies.ToList();
            //for (int i = 0; i < enemies.Count; ++i)
            {
                Enemy enemy = GameManager.Instance.enemies[enemyIndex];

                // 공격한 적은 체크
                if (!hasAttack[enemyIndex])
                {
                    count++;
                    hasAttack[enemyIndex] = true;
                }
                else
                {
                    enemyIndex = Random.Range(0, 4);
                    continue;
                }

                // 비활성화 된 적은 패스
                if (!enemy.gameObject.activeSelf)
                {
                    enemyIndex = Random.Range(0, 4);
                    continue;
                }

                // 실제 스킬 사용
                //enemies[i].EndEnemyTurn();
                enemy.EndEnemyTurn();

                // 스킬 모션
                yield return StartCoroutine(enemy.SkillMotion());

                //enemyIndex = Random.Range(0, 4);
            }
            #endregion 적 공격

            // 플레이어 턴으로 변경
            myTurn = true;
        }
    }
}

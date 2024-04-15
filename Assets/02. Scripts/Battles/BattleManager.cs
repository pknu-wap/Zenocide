using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 턴 관리, 게임 오버, 게임 중단 등을 다루는 클래스
public class BattleManager : MonoBehaviour
{
    #region 싱글톤
    public static BattleManager Instance { get; set; }
    private static BattleManager instance;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(gameObject);
        }
    }
    #endregion 싱글톤

    #region 턴 관리
    public bool isPlayerTurn = true;

    public UnityEvent onStartPlayerTurn;    // 플레이어 턴이 시작할 때
    public UnityEvent onStartEnemyTurn;     // 적 턴이 시작할 때
    public UnityEvent onEndPlayerTurn;      // 플레이어 턴이 끝날 때
    public UnityEvent onEndEnemyTurn;       // 적 턴이 끝날 때

    public void ToggleTurn()
    {
        // 턴을 변경한다.
        if (isPlayerTurn)
        {
            onEndPlayerTurn.Invoke();
            onStartEnemyTurn.Invoke();
            isPlayerTurn = false;
            Debug.Log("적 턴");

            // 적 턴이 끝나면 자동으로 플레이어 턴으로 바꾼다.
            ToggleTurn();
        }
        else
        {
            onEndEnemyTurn.Invoke();
            onStartPlayerTurn.Invoke();
            isPlayerTurn = true;
            Debug.Log("플레이어 턴");
        }
    }

    public void StartTurn(bool isPlayerTurn)
    {
        // 플레이어의 턴을 시작한다.
        if (isPlayerTurn)
        {

        }

        // 적의 턴을 시작한다.
        else
        {

        }
    }

    public void EndTurn(bool isPlayerTurn)
    {

    }
    #endregion 턴 관리

    public void GameOver()
    {
        Debug.Log("게임 오버");
        // 게임의 모든 기능을 정지한다. -> 입력을 막음, 더미 콜라이더 생성?
        // 게임 오버 메시지를 출력한다.
        // 부활이 가능할 경우, 부활 팝업을 띄운다. <- 넣을지 말지도 회의 필요

        // 게임을 리셋시킨다. 혹은 처음부터 다시 시작한다. -> 어떻게 시작할지를 정하자.
    }
}

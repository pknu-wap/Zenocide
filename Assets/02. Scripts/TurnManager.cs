using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Inst { get; private set; }
    private void Awake() => Inst = this;

    [Header("Develop")]
    [SerializeField] [Tooltip("시작 턴 모드를 정합니다")] ETurnMode eTurnMode;
    [SerializeField] [Tooltip("드로우 카드 개수를 정합니다")] int drawCardCount;

    [Header("Properties")]
    public bool isLoading; // 게임 끝나면 isLoading을 true로 하면 카드와 엔티티 클릭방지
    public bool myTurn;

    enum ETurnMode { Random, My, Other }
    WaitForSeconds delay03 = new WaitForSeconds(0.3f);
    WaitForSeconds delay07 = new WaitForSeconds(0.7f);

    public static Action<bool> OnAddCard;

    void GameSetup()
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

    public IEnumerator StartGameCo()
    {
        GameSetup();
        isLoading = true;

        // 드로우 카드 수만큼 드로우
        for (int i = 0; i < drawCardCount; i++)
        {
/*            yield return delay05;
            OnAddCard?.Invoke(false);*/
            yield return delay03;
            OnAddCard?.Invoke(true);
        }
    }

    IEnumerator StartTurnCo()
    {
        isLoading = true;

        // 턴 시작 UI 출력
        if (myTurn)
            GameManager.Inst.Notification("나의 턴");

        // 덱이 없으면 묘지를 셔플해서 새로 생성
        if(CardManager.Inst.deck.Count == 0)
            CardManager.Inst.ResetDeck();

        // 드로우 카드 수만큼 드로우
        for (int i = 0; i < drawCardCount; i++)
        {
            yield return delay03;
            OnAddCard?.Invoke(myTurn);
        }
        yield return delay07;

        isLoading = false;
    }

    public void EndTurn()
    {
        if(myTurn)
            StartCoroutine(CardManager.Inst.DiscardHandCo());
        myTurn = !myTurn;
        StartCoroutine(StartTurnCo());
    }
}

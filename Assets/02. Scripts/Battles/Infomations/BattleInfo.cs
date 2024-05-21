// 김민철
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class BattleInfo : MonoBehaviour
{
    public static BattleInfo Inst { get; private set; }
    void Awake() {
        Inst = this;

        remainingEnemies = new List<Enemy>();
    }

    private void Start()
    {
        ResetCost();
        TurnManager.Inst.onStartPlayerTurn.AddListener(ResetCost);
    }

    [Header("전투 정보")]
    // 남은 적 오브젝트 배열
    public List<Enemy> remainingEnemies;
    // 게임 오버 여부
    public bool isGameOver = false;
    // 최대 코스트
    public int maxCost = 5;
    // 현재 코스트
    public int currentCost = 5;

    [Header("플레이어 능력치")]
    // 추가 공격력. 데미지 계산 식에 적용
    public int bonusAttackStat;
    // 추가 데미지. 데미지 계산 후, 추가로 들어가는 고정 데미지
    public int bonusDamage;
    // 추가 방어력. 데미지 계산 식에 적용
    public int bonusArmor;

    [Header("컴포넌트")]
    public TMP_Text costText;

    // 모든 값을 초기화한다.
    public void ResetBattleInfo()
    {
        // 남은 적 숫자를 적 숫자에 맞춰 초기화한다.
        bonusAttackStat = 0;
        bonusDamage = 0;
        bonusArmor = 0;
    }

    // 적 오브젝트 등록한다. 스토리 씬과 연계되면 삭제될 수도 있다.
    public void EnrollEnemy(Enemy enemy)
    {
        remainingEnemies.Add(enemy);
    }

    // 적 오브젝트를 삭제한다.
    public void DisenrollEnemy(Enemy enemy)
    {
        // 리스트가 이미 비어있다면 리턴한다. (예외 처리)
        if(remainingEnemies.Any() == false)
        {
            return;
        }

        // 리스트에서 enemy를 삭제한다.
        remainingEnemies.Remove(enemy);

        // 삭제 후 리스트가 비어있다면
        if(remainingEnemies.Any() == false)
        {
            // 게임을 종료한다.
            isGameOver = true;

            GameManager.Inst.Notification("승리");

            // 이후 스토리로 복귀한다.
        }
    }

    // cost로 카드 사용이 가능한지 알려준다.
    public bool CanUseCost(int cost)
    {
        return currentCost - cost >= 0;
    }


    // 코스트를 줄인다. 성공 시 true, 실패 시 false를 반환한다.
    public bool UseCost(int cost)
    {
        if(currentCost - cost < 0)
        {
            return false;
        }

        currentCost -= cost;
        UpdateCostText();

        return true;
    }

    // 코스트를 maxCost로 복구시킨다.
    public void ResetCost()
    {
        currentCost = maxCost;
        UpdateCostText();
    }

    void UpdateCostText()
    {
        costText.text = currentCost + "/" + maxCost;
    }
}

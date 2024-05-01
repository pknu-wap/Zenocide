// 김민철
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleInfo : MonoBehaviour
{
    public static BattleInfo Inst { get; private set; }
    void Awake() => Inst = this;

    [Header("전투 정보")]
    // 남은 적 숫자
    public int remainingEnemies;
    // 게임 오버 여부
    public bool isGameOver = false;

    [Header("플레이어 능력치")]
    // 추가 공격력. 데미지 계산 식에 적용
    public int bonusAttackStat;
    // 추가 데미지. 데미지 계산 후, 추가로 들어가는 고정 데미지
    public int bonusDamage;
    // 추가 방어력. 데미지 계산 식에 적용
    public int bonusArmor;

    // 모든 값을 초기화한다.
    public void ResetBattleInfo()
    {
        // 남은 적 숫자를 적 숫자에 맞춰 초기화한다.
        bonusAttackStat = 0;
        bonusDamage = 0;
        bonusArmor = 0;
    }

    // 적 오브젝트 숫자를 1 줄인다.
    public void IncreaseEnemyCount()
    {
        ++remainingEnemies;
    }

    // 적 오브젝트 숫자를 1 줄인다.
    public void DecreaseEnemyCount()
    {
        --remainingEnemies;

        if(remainingEnemies <= 0)
        {
            isGameOver = true;

            GameManager.Inst.Notification("승리");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleInfo : MonoBehaviour
{
    // 추가 공격력. 데미지 계산 식에 적용
    public static int bonusAttackStat;
    // 추가 데미지. 데미지 계산 후, 추가로 들어가는 고정 데미지
    public static int bonusDamage;
    // 추가 방어력. 데미지 계산 식에 적용
    public static int bonusArmor;

    // 모든 값을 초기화한다.
    public void ResetBattleInfo()
    {
        bonusAttackStat = 0;
        bonusDamage = 0;
        bonusArmor = 0;
    }
}

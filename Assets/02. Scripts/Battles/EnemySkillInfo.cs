using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum EnemySkillType
{
    Attack,         // 공격
    Shield,         // 실드 생성
    Heal,           // 체력 회복
    Cleanse,        // 디버프 제거
    Buff,           // 버프
    Debuff          // 디버프
};

public enum EnemyTarget
{
    Player, // 플레이어
    Self    // 자기 자신
};

public class EnemySkillInfo : MonoBehaviour
{
    #region 싱글톤
    public static EnemySkillInfo Instance { get; set; }
    private static EnemySkillInfo instance;

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

        // 타입별 타겟을 배열에 등록
        EnrollTargetDict();
        // 카드 효과를 델리게이트 배열에 모두 등록
        EnrollAllEffects();
    }
    #endregion 싱글톤

    private EnemyTarget[] targetDict;

    #region 정보 검색
    private void EnrollTargetDict()
    {
        targetDict = new EnemyTarget[Enum.GetValues(typeof(EnemySkillType)).Length];

        targetDict[(int)EnemySkillType.Attack] = EnemyTarget.Player;
        targetDict[(int)EnemySkillType.Shield] = EnemyTarget.Self;
        targetDict[(int)EnemySkillType.Heal] = EnemyTarget.Self;
        targetDict[(int)EnemySkillType.Cleanse] = EnemyTarget.Self;
        targetDict[(int)EnemySkillType.Buff] = EnemyTarget.Self;
        targetDict[(int)EnemySkillType.Debuff] = EnemyTarget.Player;
    }

    public Character ReturnTarget(EnemySkillType type, Character caller)
    {
        if (targetDict[(int)type] == EnemyTarget.Player)
        {
            return Player.Instance;
        }
        else
        {
            return caller;
        }
    }
    #endregion 정보 검색

    #region 스킬 효과
    // 카드 효과 함수들을 담아둘 델리게이트
    public delegate void EnemySkillEffects(int amount, Character target);
    // 델리게이트 배열, EnemySkillType에 맞는 함수를 매칭한다.
    public EnemySkillEffects[] effects;

    // 사용 예시
    // EnemySkillInfo.Instance.effects[(int)효과 종류](효과량, 대상);
    // EnemySkillInfo.Instance.effects[(int)EnemySkillType.Buff](5, target);

    // 모든 효과를 effects 배열에 등록한다.
    void EnrollAllEffects()
    {
        effects = new EnemySkillEffects[Enum.GetValues(typeof(EnemySkillType)).Length];

        // 카드 효과를 배열에 등록
        effects[(int)EnemySkillType.Attack] += Attack;
        effects[(int)EnemySkillType.Shield] += Shield;
        effects[(int)EnemySkillType.Heal] += Heal;
        effects[(int)EnemySkillType.Cleanse] += Cleanse;
        effects[(int)EnemySkillType.Buff] += Buff;
        effects[(int)EnemySkillType.Debuff] += Debuff;
    }

    // target이 null인 경우는 Card의 OnEndDrag에서 검사했으므로, 검사하지 않는다.
    public void Attack(int amount, Character target)
    {
        target.DecreaseHP(amount);
        Debug.Log(target.GetHP());
    }

    public void Shield(int amount, Character target)
    {
        Debug.Log("Shield");
    }

    public void Heal(int amount, Character target)
    {
        Debug.Log("Heal");
    }

    public void Cleanse(int amount, Character target)
    {
        Debug.Log("Cleanse");
    }

    public void RestoreCost(int amount, Character target)
    {
        Debug.Log("RestoreCost");
    }

    public void Draw(int amount, Character target)
    {
        Debug.Log("Draw");
    }

    public void Buff(int amount, Character target)
    {
        Debug.Log("Buff");
    }

    public void Debuff(int amount, Character target)
    {
        Debug.Log("Debuff");
    }
    #endregion 카드 효과
}

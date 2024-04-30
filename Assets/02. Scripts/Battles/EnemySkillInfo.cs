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
    Debuff,         // 디버프
    Bleed,          // 출혈
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

    #region 정보 검색
    private EnemyTarget[] targetDict;

    private void EnrollTargetDict()
    {
        targetDict = new EnemyTarget[Enum.GetValues(typeof(EnemySkillType)).Length];

        targetDict[(int)EnemySkillType.Attack] = EnemyTarget.Player;
        targetDict[(int)EnemySkillType.Shield] = EnemyTarget.Self;
        targetDict[(int)EnemySkillType.Heal] = EnemyTarget.Self;
        targetDict[(int)EnemySkillType.Cleanse] = EnemyTarget.Self;
        targetDict[(int)EnemySkillType.Buff] = EnemyTarget.Self;
        targetDict[(int)EnemySkillType.Debuff] = EnemyTarget.Player;
        targetDict[(int)EnemySkillType.Bleed] = EnemyTarget.Player;
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
    public delegate void EnemySkillEffects(int amount, int turnCount, Character target);
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
        effects[(int)EnemySkillType.Bleed] += Bleed;
    }

    // target이 null인 경우는 Card의 OnEndDrag에서 검사했으므로, 검사하지 않는다.
    // {target}에게 {amount}만큼 데미지를 입힌다.
    public void Attack(int amount, int turnCount, Character target)
    {
        target.DecreaseHP(amount);
    }

    // {Shield}만큼 보호막을 얻는다. 보호막은 체력 대신 소모된다.
    public void Shield(int amount, int turnCount, Character target)
    {
        Debug.Log("Shield");
    }

    // {amount}만큼 체력을 회복한다.
    public void Heal(int amount, int turnCount, Character target)
    {
        Debug.Log("Heal");
    }

    // 모든 디버프를 정화한다.
    public void Cleanse(int amount, int turnCount, Character target)
    {
        Debug.Log("Cleanse");
    }

    // 코스트를 {amount} 회복한다.
    public void RestoreCost(int amount, int turnCount, Character target)
    {
        Debug.Log("RestoreCost");
    }

    // 카드를 {amount} 개 뽑는다.
    public void Draw(int amount, int turnCount, Character target)
    {
        Debug.Log("Draw");
    }

    // 버프(삭제 예정)
    public void Buff(int amount, int turnCount, Character target)
    {
        Debug.Log("Buff");
    }

    // 디버프(삭제 예정)
    public void Debuff(int amount, int turnCount, Character target)
    {
        Debug.Log("Debuff");
    }

    // {turnCount} 턴 간 {amount}의 데미지를 입힌다.
    public void Bleed(int amount, int turnCount, Character target)
    {
        target.EnrollBleed(new BleedEffect(amount, turnCount));
    }
    #endregion 카드 효과
}

// 김민철
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class CardInfo : MonoBehaviour
{
    #region 싱글톤
    public static CardInfo Instance { get; set; }
    private static CardInfo instance;

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

        // 카드 효과를 델리게이트에 모두 등록
        EnrollAllSkills();
        EnrollLayerDict();
    }
    #endregion 싱글톤

    #region 카드 UI 데이터
    // 통합하고 싶다...
    public Sprite[] skillIcons;
    public Sprite[] debuffIcons;
    #endregion 카드 UI 데이터

    #region 정보 검색
    // 카드 타입을 넣으면 레이어를 뱉어주는 배열
    private LayerMask[] layerDict;

    // 배열에 타입 - 레이어 정보를 등록한다.
    private void EnrollLayerDict()
    {
        layerDict = new LayerMask[Enum.GetValues(typeof(SkillType)).Length];

        layerDict[(int)SkillType.Attack] = LayerMask.GetMask("Enemy");
        layerDict[(int)SkillType.Shield] = LayerMask.GetMask("Field");
        layerDict[(int)SkillType.Heal] = LayerMask.GetMask("Field");
        layerDict[(int)SkillType.Cleanse] = LayerMask.GetMask("Field");
        layerDict[(int)SkillType.RestoreCost] = LayerMask.GetMask("Field");
        layerDict[(int)SkillType.Draw] = LayerMask.GetMask("Field");
        layerDict[(int)SkillType.Buff] = LayerMask.GetMask("Field");
        layerDict[(int)SkillType.Debuff] = LayerMask.GetMask("Enemy");
        layerDict[(int)SkillType.Bleed] = LayerMask.GetMask("Enemy");
    }

    // 타입에 맞는 레이어를 반환한다.
    public LayerMask ReturnLayer(SkillType type)
    {
        return layerDict[(int)type];
    }

    // 카드가 타겟팅 카드인지 알려준다.
    public bool IsTargetingCard(Skill[] data)
    {
        // 카드가 타겟팅 스킬을 갖고 있다면 true를 리턴한다.
        for(int i = 0; i < data.Length; ++i)
        {
            if (data[i].target == SkillTarget.Enemy)
            {
                return true;
            }
        }

        // 그 외엔 false를 리턴한다.
        return false;
    }

    public SkillTextInfo skillTexts;

    public SkillText GetSkillText(Skill skill)
    {
        SkillText skillText = new SkillText();

        // 스킬 이름을 저장한다.
        skillText.name = skillTexts.text[(int)skill.type].name;

        // 스킬 설명을 효과량, 턴 수와 함께 저장한다.
        if(skill.turnCount != 0)
        {
            // 턴 수가 0이 아니라면 설명에 추가한다.
            skillText.description = skill.turnCount + "턴 간 ";
        }

        if (skill.amount != 0)
        {
            // 효과량이 0이 아니라면 추가한다.
            skillText.description = skill.amount + "의 ";
        }
        // 타입에 맞는 설명을 추가한다.
        skillText.description += skillTexts.text[(int)skill.type].description;

        return skillText;
    }
    #endregion 정보 검색

    #region 카드 효과
    // 카드 효과 함수들을 담아둘 델리게이트
    // amount는 카드의 효과량, turnCount는 지속될 턴의 수, target은 적용 대상이다.
    // 예를 들어 Bleed의 변수가 6, 3, Player.Instance라면, 플레이어에게 3턴간, 매 턴 6의 데미지를 준다.
    public delegate void CardSkill(int amount, int turnCount, Character target);
    // 델리게이트 배열, EffectType에 맞는 함수를 매칭한다.
    public CardSkill[] effects;

    // 카드를 사용할 때 호출한다.
    public void UseSkill(Skill skill, Character[] target)
    {
        for(int i = 0; i < target.Length; ++i)
        {
            // 모든 타겟에게 skill을 사용한다.
            ActivateSkill(skill, target[i]);
            // 딜레이를 주면 좀 더 자연스럽다.
        }
    }

    // 모든 효과를 effects 배열에 등록한다.
    void EnrollAllSkills()
    {
        effects = new CardSkill[Enum.GetValues(typeof(SkillType)).Length];

        // 카드 효과를 배열에 등록
        effects[(int)SkillType.Attack] += Attack;
        effects[(int)SkillType.Shield] += Shield;
        effects[(int)SkillType.Heal] += Heal;
        effects[(int)SkillType.Cleanse] += Cleanse;
        effects[(int)SkillType.RestoreCost] += RestoreCost;
        effects[(int)SkillType.Draw] += Draw;
        effects[(int)SkillType.Buff] += Buff;
        effects[(int)SkillType.Debuff] += Debuff;
        effects[(int)SkillType.Bleed] += Bleed;
    }

    /// 카드에 맞는 효과를 발동시킨다.
    public void ActivateSkill(Skill skill, Character selectedCharacter)
    {
        // selectedCharacter는 타겟팅 스킬에만 사용되며, 나머지 스킬에선 무시된다.
        effects[(int)skill.type](skill.amount, skill.turnCount, selectedCharacter);
    }

    // target이 null인 경우는 Card의 OnEndDrag에서 검사했으므로, 검사하지 않는다.
    public void Attack(int amount, int turnCount, Character target)
    {
        target.DecreaseHP(amount);
    }

    public void Shield(int amount, int turnCount, Character target)
    {
        Debug.Log("Shield");
    }

    public void Heal(int amount, int turnCount, Character target)
    {
        Debug.Log("Heal");
    }

    public void Cleanse(int amount, int turnCount, Character target)
    {
        Debug.Log("Cleanse");
    }

    public void RestoreCost(int amount, int turnCount, Character target)
    {
        Debug.Log("RestoreCost");
    }

    public void Draw(int amount, int turnCount, Character target)
    {
        Debug.Log("Draw");
    }

    public void Buff(int amount, int turnCount, Character target)
    {
        Debug.Log("Buff");
    }

    public void Debuff(int amount, int turnCount, Character target)
    {
        Debug.Log("Debuff");
    }

    public void Bleed(int amount, int turnCount, Character target)
    {
        target.EnrollBleed(new BleedEffect(SkillType.Bleed, amount, turnCount));
    }
    #endregion 카드 효과
}

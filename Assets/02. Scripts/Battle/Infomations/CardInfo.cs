// 김민철
using UnityEngine;
using System;
using System.Collections;

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
        layerDict[(int)SkillType.Bleed] = LayerMask.GetMask("Enemy");
        layerDict[(int)SkillType.AddExtraDamage] = LayerMask.GetMask("Field");
    }

    // 타입에 맞는 레이어를 반환한다.
    public LayerMask ReturnLayer(SkillType type)
    {
        return layerDict[(int)type];
    }

    // 타겟을 반환한다.
    public Character[] GetTarget(SkillTarget target, Enemy selectedEnemy)
    {
        // 각 경우에 맞는 값을 반환한다. 근데 전부 배열로 만들고 반환해서, 나중에 리팩토링 한 번 해야 겠다.
        if (target == SkillTarget.Player)
        {
            return new Character[] { Player.Instance };
        }

        else if (target == SkillTarget.Enemy)
        {
            return new Character[] { selectedEnemy };
        }

        else if (target == SkillTarget.AllEnemy)
        {
            return BattleInfo.Instance.remainingEnemies.ToArray();
        }

        return null;
    }


    // 타겟을 반환한다. 논타겟 스킬일 때 호출 가능하다.
    public Character[] GetTarget(SkillTarget target)
    {
        // 타겟이 지정되면 null을 반환한다.
        if (target == SkillTarget.Enemy)
        {
            return null;
        }

        // 각 경우에 맞는 값을 반환한다. 근데 전부 배열로 만들고 반환해서, 나중에 리팩토링 한 번 해야 겠다.
        else if (target == SkillTarget.Player)
        {
            return new Character[] { Player.Instance };
        }

        else if (target == SkillTarget.AllEnemy)
        {
            return BattleInfo.Instance.remainingEnemies.ToArray();
        }

        return null;
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
        effects[(int)SkillType.Bleed] += Bleed;
        effects[(int)SkillType.AddExtraDamage] += AddExtraDamage;
    }


    // 모든 타겟에게 스킬을 사용한다.
    public void ActivateSkill(Skill skill, Character[] target)
    {
        for (int i = 0; i < target.Length; ++i)
        {
            // 모든 타겟에게 skill을 사용한다.
            effects[(int)skill.type](skill.amount, skill.turnCount, target[i]);
        }
    }

    // target이 null인 경우는 Card의 OnEndDrag에서 검사했으므로, 검사하지 않는다.
    public void Attack(int amount, int turnCount, Character target)
    {
        // 타겟의 체력을 감소시킨다.
        target.DecreaseHP(amount + BattleInfo.Instance.bonusDamage);
    }

    public void Shield(int amount, int turnCount, Character target)
    {
        Debug.Log("Shield");
    }

    public void Heal(int amount, int turnCount, Character target)
    {
        // 타겟의 체력을 회복시킨다.
        target.IncreaseHP(amount);
    }

    public void Cleanse(int amount, int turnCount, Character target)
    {
        // 타겟의 디버프를 모두 제거한다.
        target.CleanseDebuff();
    }

    public void RestoreCost(int amount, int turnCount, Character target)
    {
        // target이 Player가 아니라면 종료한다.
        if (target.GetType() != typeof(Player))
        {
            return;
        }

        // 코스트를 회복시킨다.
        BattleInfo.Instance.RestoreCost(amount);
    }

    public void Draw(int amount, int turnCount, Character target)
    {
        // target이 Player가 아니라면 종료한다.
        if (target.GetType() != typeof(Player))
        {
            return;
        }

        // 카드를 뽑는다.
        StartCoroutine(TurnManager.Instance.DrawCard(amount));
    }

    public void Bleed(int amount, int turnCount, Character target)
    {
        target.EnrollBleed(new BleedEffect(SkillType.Bleed, amount, turnCount));
    }

    public void AddExtraDamage(int amount, int turnCount, Character target)
    {
        BattleInfo.Instance.GetBonusDamage(amount);
    }
    #endregion 카드 효과
}

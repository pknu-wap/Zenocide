using System.Collections.Generic;
using UnityEngine;

public class DebuffInfo
{
    public static Dictionary<SkillType, string> debuffNameDict = new Dictionary<SkillType, string>
    {
        { SkillType.Bleed, "출혈" },
        { SkillType.Burn, "화상" },
        { SkillType.Heal, "지속 회복" },
        { SkillType.ExtraDamage, "추가 데미지" },
        { SkillType.ExtraBleedDamage, "추가 출혈 데미지" },
        { SkillType.Silence, "침묵" },
        { SkillType.SilenceStack, "침묵 스택" },
    };
    public static Dictionary<SkillType, string> debuffDescriptionDict = new Dictionary<SkillType, string>
    {
        { SkillType.Bleed, "피해를 입습니다." },
        { SkillType.Burn, "피해를 받습니다." },
        { SkillType.Heal, "체력을 회복합니다." },
        { SkillType.ExtraDamage, "추가 데미지를 입힙니다." },
        { SkillType.ExtraBleedDamage, "추가 출혈 데미지를 입힙니다." },
        { SkillType.Silence, "..." },
        { SkillType.SilenceStack, "2 이상인 경우 다음 턴에 침묵됩니다." },
    };

    public static SkillText GetSkillText(BuffEffect buff)
    {
        SkillText skillText = new SkillText();

        // 스킬 이름을 저장한다.
        skillText.name = debuffNameDict[buff.type];

        // 스킬 설명을 효과량, 턴 수와 함께 저장한다.
        if (buff.remainingTurns != 0 && buff.type != SkillType.Silence && buff.type != SkillType.SilenceStack)
        {
            // 턴 수가 0이 아니라면 설명에 추가한다.
            skillText.description += (buff.remainingTurns + "턴 간 ");
        }

        if (buff.amount != 0)
        {
            // 효과량이 0이 아니라면 추가한다.
            skillText.description += (buff.amount + "의 ");
        }
        // 타입에 맞는 설명을 추가한다.
        skillText.description += debuffDescriptionDict[buff.type];

        return skillText;
    }
}

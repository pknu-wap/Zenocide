using System.Collections;
using System.Collections.Generic;

public class DebuffInfo
{
    public static Dictionary<SkillType, string> debuffNameDict = new Dictionary<SkillType, string>
    {
        { SkillType.Bleed, "출혈" }
    };
    public static Dictionary<SkillType, string> debuffDescriptionDict = new Dictionary<SkillType, string>
    {
        { SkillType.Bleed, "턴 간 2의 피해를 입습니다." }
    };
}

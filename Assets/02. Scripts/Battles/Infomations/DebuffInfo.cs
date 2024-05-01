using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffInfo
{
    public static Dictionary<EnemySkillType, string> debuffNameDict = new Dictionary<EnemySkillType, string>
    {
        { EnemySkillType.Bleed, "출혈" }
    };
    public static Dictionary<EnemySkillType, string> debuffDescriptionDict = new Dictionary<EnemySkillType, string>
    {
        { EnemySkillType.Bleed, "턴 간 2의 피해를 입습니다." }
    };
}

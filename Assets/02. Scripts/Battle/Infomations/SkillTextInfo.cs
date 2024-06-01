using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 각 스킬별 이름과 설명을 담아두는 클래스 (적의 다음 행동을 띄울 때 사용)
[System.Serializable]
public class SkillText
{
    public string name;
    public string description;
}

[CreateAssetMenu(fileName = "SkillInfo", menuName = "Scriptable Object/Skill Info")]
public class SkillTextInfo : ScriptableObject
{
    public SkillText[] text;
}

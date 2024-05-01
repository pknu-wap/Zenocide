// 김민철
using UnityEngine;

[System.Serializable]
public class Skill
{
    [Header("정보")]
    // 스킬 이름
    public string skillName;
    // 스킬 설명
    public string description;

    // 카드 효과의 종류
    public EnemySkillType type;
    // 카드 효과에 적용되는 수치
    public int amount;
    // 효과가 지속되는 턴 수
    public int turnCount;
}

[CreateAssetMenu(fileName = "EnemySkillData", menuName = "Scriptable Object/Enemy Skill Data")]
public class SkillData : ScriptableObject
{
    public Skill[] skills;
}

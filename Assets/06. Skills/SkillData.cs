// ±ËπŒ√∂
using UnityEngine;

[System.Serializable]
public class Skill
{
    [Header("¡§∫∏")]
    public string skillName;
    public string description;

    public EffectType type;
    public int amount;
}

[CreateAssetMenu(fileName = "EnemySkillData", menuName = "Scriptable Object/Enemy Skill Data")]
public class SkillData : ScriptableObject
{
    public Skill[] skills;
}

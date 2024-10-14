// 김민철
using UnityEngine;

// 카드의 종류
public enum SkillType
{
    Attack = 0,                 // 공격
    Shield = 1,                 // 실드 생성
    Heal = 2,                   // 체력 회복
    Cleanse = 3,                // 디버프 제거
    RestoreCost = 4,            // 코스트 회복
    Draw = 5,                   // 카드 드로우
    Bleed = 6,                  // 출혈 데미지 (한 턴)
    Burn = 7,                   // 화상 데미지 (한 턴)
    ExtraDamage = 8,         // 추가 데미지 제공 (한 턴)
    LingeringHeal = 9,          // 지속 회복
    LingeringBleed = 10,         // 출혈 (지속)
    LingeringBurn = 11,          // 화상 (지속)
    LingeringExtraDamage = 12,   // 추가 데미지 제공 (지속)
    ModifyCost = 13,            // 코스트 조정
    Drain = 14,                 // 흡혈
    ExtraBleedDamage = 15,   // 출혈 추가데미지 (한 턴)
    LingeringExtraBleedDamage = 16,   // 출혈 추가데미지 (지속)
    Silence = 17,                // 침묵, 아무 행동도 하지 않음
}

public enum SkillTarget
{
    Player,         // 플레이어
    Enemy,          // 적
    AllEnemy,       // 모든 적
}

// 카드가 갖고 있는 스킬의 종류
[System.Serializable]   // 이걸 붙여줘야 Inspector에 나타난다.
public class Skill
{
    [Header("카드 성능")]
    // 카드 효과의 종류
    public SkillType type;
    // 스킬을 적용받을 타겟
    public SkillTarget target;
    // 카드 효과에 적용되는 수치
    public int amount;
    // 효과가 지속되는 턴 수
    public int turnCount;
    // 스킬 이펙트
    public ParticleSystem effectPrefeb;
    // 스킬 효과음
    public AudioClip sound;
}

// 유니티 에디터, Project 뷰의 Create 메뉴에 아래 항목을 추가한다.
[CreateAssetMenu(fileName = "Card Data", menuName = "Scriptable Object/Card Data", order = 0)]
public class CardData : ScriptableObject
{
    [Header("카드 모양")]
    // 카드 이름
    public new string name;
    // 카드 설명
    [TextArea(3, 5)]
    public string description;
    // 카드 일러스트
    public Sprite sprite;

    [Header("카드 성능")]
    // 카드 스킬
    public Skill[] skills;
    // 카드 발동에 필요한 코스트
    public int cost;
}
